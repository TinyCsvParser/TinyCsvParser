// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;

namespace TinyCsvParser
{
    #region Csv Parser

    public readonly record struct CsvOptions(
        char Delimiter,
        char QuoteChar,
        char EscapeChar,
        Encoding? Encoding = null,
        bool SkipHeader = false
    )
    {
        public static CsvOptions Default => new(';', '"', '\\', System.Text.Encoding.UTF8, false);

        public static CsvOptions Rfc4180 => new(';', '"', '"', System.Text.Encoding.UTF8, false);
    }

    public class CsvParser<TEntity> where TEntity : class, new()
    {
        private readonly CsvOptions _options;
        private readonly ICsvMapping<TEntity> _mapping;

        public CsvParser(CsvOptions options, ICsvMapping<TEntity> mapping)
        {
            _options = options;
            _mapping = mapping;
        }

        public IEnumerable<CsvMappingResult<TEntity>> ReadFromFile(string filePath)
        {
            return new CsvReaderEnumerable(
                () => new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read),
                _options,
                _mapping,
                shouldDisposeStream: true);
        }

        public IEnumerable<CsvMappingResult<TEntity>> ReadFromStream(Stream stream)
        {
            return new CsvReaderEnumerable(() => stream, _options, _mapping, shouldDisposeStream: false);
        }

        public IEnumerable<CsvMappingResult<TEntity>> ReadFromString(string csvContent)
        {
            var encoding = _options.Encoding ?? Encoding.UTF8;
            return new CsvReaderEnumerable(
                () => new MemoryStream(encoding.GetBytes(csvContent)),
                _options,
                _mapping,
                shouldDisposeStream: true);
        }

        private class CsvReaderEnumerable : IEnumerable<CsvMappingResult<TEntity>>
        {
            private readonly Func<Stream> _streamFactory;
            private readonly CsvOptions _options;
            private readonly ICsvMapping<TEntity> _mapping;
            private readonly bool _shouldDisposeStream;

            public CsvReaderEnumerable(Func<Stream> streamFactory, CsvOptions options, ICsvMapping<TEntity> mapping, bool shouldDisposeStream)
            {
                _streamFactory = streamFactory;
                _options = options;
                _mapping = mapping;
                _shouldDisposeStream = shouldDisposeStream;
            }

            public IEnumerator<CsvMappingResult<TEntity>> GetEnumerator()
            {
                return new CsvReaderEnumerator(_streamFactory(), _options, _mapping, _shouldDisposeStream);
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        private class CsvReaderEnumerator : IEnumerator<CsvMappingResult<TEntity>>
        {
            private readonly Stream _stream;
            private readonly CsvOptions _options;
            private readonly ICsvMapping<TEntity> _mapping;
            private readonly bool _shouldDisposeStream;

            private SpanBasedCsvReader? _reader;
            private CsvMappingResult<TEntity> _current;
            private readonly long[] _rangesBuffer;

            public CsvReaderEnumerator(Stream stream, CsvOptions options, ICsvMapping<TEntity> mapping, bool shouldDisposeStream)
            {
                _stream = stream;
                _options = options;
                _mapping = mapping;
                _shouldDisposeStream = shouldDisposeStream;
                _rangesBuffer = new long[1024];
                _current = default;
            }

            public CsvMappingResult<TEntity> Current => _current;

            object IEnumerator.Current => _current;

            public bool MoveNext()
            {
                if (_reader == null)
                {
                    _reader = new SpanBasedCsvReader(_stream, _options);

                    if (_mapping is IHeaderBinder binder && binder.NeedsHeaderResolution)
                    {
                        if (_reader.TryGetNextRecord(out var headerLine))
                        {
                            Span<long> ranges = _rangesBuffer.AsSpan();

                            int count = CsvParserEngine.SplitLine(headerLine, _options, ranges);

                            var headerRow = new CsvRow(headerLine, ranges.Slice(0, count), _options);

                            binder.BindHeaders(ref headerRow);
                        }
                        else
                        {
                            throw new InvalidOperationException("Could not read CSV header for mapping initialization.");
                        }
                    }
                    else if (_options.SkipHeader)
                    {
                        // Regular header skip if no resolution is needed
                        _reader.TryGetNextRecord(out _);
                    }
                }

                if (_reader.TryGetNextRecord(out ReadOnlySpan<char> line))
                {
                    Span<long> rangesSpan = _rangesBuffer.AsSpan();

                    int columnsFound = CsvParserEngine.SplitLine(line, _options, rangesSpan);

                    var row = new CsvRow(line, rangesSpan.Slice(0, columnsFound), _options);

                    _current = _mapping.Map(ref row);

                    return true;
                }

                return false;
            }

            public void Reset() => throw new NotSupportedException("CSV Streams cannot be reset.");

            public void Dispose()
            {
                _reader?.Dispose();

                if (_shouldDisposeStream)
                {
                    _stream.Dispose();
                }

                _reader = null;
            }
        }
    }

    public static class CsvParserEngine
    {
        public static int SplitLine(ReadOnlySpan<char> line, CsvOptions options, Span<long> rangesBuffer)
        {
            int rangeCount = 0;
            int currentIdx = 0;
            int length = line.Length;

            char delimiter = options.Delimiter;
            char quote = options.QuoteChar;
            char escape = options.EscapeChar;

            while (currentIdx < length && rangeCount < rangesBuffer.Length)
            {
                int fieldStart = currentIdx;
                bool isQuoted = false;
                bool needsUnescape = false;

                if (line[currentIdx] == quote)
                {
                    isQuoted = true;
                    currentIdx++;

                    while (currentIdx < length)
                    {
                        char c = line[currentIdx];

                        if (c == escape)
                        {
                            if (escape == quote)
                            {
                                if (currentIdx + 1 < length && line[currentIdx + 1] == quote)
                                {
                                    needsUnescape = true;
                                    currentIdx += 2;
                                    continue;
                                }
                                else
                                {
                                    currentIdx++;
                                    break;
                                }
                            }
                            else
                            {
                                if (currentIdx + 1 < length)
                                {
                                    needsUnescape = true;
                                    currentIdx += 2;
                                    continue;
                                }
                                else
                                {
                                    currentIdx++;
                                    break;
                                }
                            }
                        }
                        else if (c == quote)
                        {
                            currentIdx++;
                            break;
                        }

                        currentIdx++;
                    }
                }

                while (currentIdx < length && line[currentIdx] != delimiter)
                {
                    currentIdx++;
                }

                int fieldLen = currentIdx - fieldStart;

                rangesBuffer[rangeCount++] = CsvRow.Pack(fieldStart, fieldLen, isQuoted, needsUnescape);

                if (currentIdx < length) currentIdx++;
            }

            if (length > 0 && line[length - 1] == delimiter && rangeCount < rangesBuffer.Length)
            {
                rangesBuffer[rangeCount++] = CsvRow.Pack(length, 0, false, false);
            }

            return rangeCount;
        }
    }

    public class SpanBasedCsvReader : IDisposable
    {
        private readonly StreamReader _reader;
        private char[]? _buffer;
        private int _charsFilled;
        private int _currentIdx;
        private bool _isEndOfStream;
        private readonly CsvOptions _options;

        private const int BufferSize = 32 * 1024;

        public SpanBasedCsvReader(Stream stream, CsvOptions options)
        {
            _reader = new StreamReader(stream, options.Encoding ?? Encoding.UTF8, true, BufferSize, leaveOpen: true);
            _buffer = ArrayPool<char>.Shared.Rent(BufferSize);
            _options = options;
        }

        public bool TryGetNextRecord(out ReadOnlySpan<char> recordSpan)
        {
            recordSpan = default;
            if (_buffer == null) return false;

            while (true)
            {
                if (TryFindEndOfRecord(out int lengthOfData, out int lengthOfDelimiter))
                {
                    recordSpan = new ReadOnlySpan<char>(_buffer, _currentIdx, lengthOfData);
                    _currentIdx += lengthOfData + lengthOfDelimiter;
                    return true;
                }

                if (_isEndOfStream)
                {
                    if (_currentIdx < _charsFilled)
                    {
                        recordSpan = new ReadOnlySpan<char>(_buffer, _currentIdx, _charsFilled - _currentIdx);
                        _currentIdx = _charsFilled;
                        return true;
                    }
                    return false;
                }

                RefillBuffer();
            }
        }

        private bool TryFindEndOfRecord(out int dataLength, out int delimiterLength)
        {
            dataLength = 0;
            delimiterLength = 0;

            if (_buffer == null) return false;

            bool inQuotes = false;
            char quote = _options.QuoteChar;
            char escape = _options.EscapeChar;
            bool sameEscapeQuote = quote == escape;

            int count = _charsFilled - _currentIdx;
            var span = new ReadOnlySpan<char>(_buffer, _currentIdx, count);

            for (int i = 0; i < span.Length; i++)
            {
                char c = span[i];

                if (c == quote)
                {
                    bool isEscaped = false;

                    if (!sameEscapeQuote && i > 0 && span[i - 1] == escape)
                    {
                        isEscaped = true;
                    }

                    if (!isEscaped)
                    {
                        inQuotes = !inQuotes;
                    }
                }

                if (!inQuotes)
                {
                    if (c == '\n')
                    {
                        dataLength = i;
                        delimiterLength = 1;
                        return true;
                    }
                    else if (c == '\r')
                    {
                        if (i + 1 >= span.Length)
                        {
                            if (!_isEndOfStream)
                            {
                                return false;
                            }
                        }

                        if (i + 1 < span.Length && span[i + 1] == '\n')
                        {
                            dataLength = i;
                            delimiterLength = 2;
                            return true;
                        }
                        else
                        {
                            dataLength = i;
                            delimiterLength = 1;
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private void RefillBuffer()
        {
            if (_buffer == null) return;

            int remaining = _charsFilled - _currentIdx;

            if (remaining > 0)
            {
                Array.Copy(_buffer, _currentIdx, _buffer, 0, remaining);
            }

            _currentIdx = 0;
            _charsFilled = remaining;

            int spaceLeft = _buffer.Length - _charsFilled;
            if (spaceLeft == 0)
            {
                char[] newBuffer = ArrayPool<char>.Shared.Rent(_buffer.Length * 2);

                Array.Copy(_buffer, 0, newBuffer, 0, _charsFilled);
                ArrayPool<char>.Shared.Return(_buffer);

                _buffer = newBuffer;
                spaceLeft = _buffer.Length - _charsFilled;
            }

            int read = _reader.Read(_buffer, _charsFilled, spaceLeft);

            _charsFilled += read;

            if (read == 0)
            {
                _isEndOfStream = true;
            }
        }

        public void Dispose()
        {
            if (_buffer != null)
            {
                ArrayPool<char>.Shared.Return(_buffer);
                _buffer = null;
            }
            _reader.Dispose();
        }
    }

    public ref struct CsvRow
    {
        private readonly ReadOnlySpan<char> _line;
        private readonly ReadOnlySpan<long> _packedInfo;
        private readonly CsvOptions _options;

        private const long IsQuotedMask = 1L << 62;
        private const long NeedsUnescapeMask = 1L << 63;

        public CsvRow(ReadOnlySpan<char> line, ReadOnlySpan<long> packedInfo, CsvOptions options)
        {
            _line = line;
            _packedInfo = packedInfo;
            _options = options;
        }

        public int Count => _packedInfo.Length;

        public string GetString(int index)
        {
            if (index >= _packedInfo.Length) return string.Empty;

            Unpack(_packedInfo[index], out int start, out int length, out bool isQuoted, out bool needsUnescape);
            var raw = _line.Slice(start, length);

            if (!isQuoted)
            {
                return raw.ToString();
            }

            if (!needsUnescape)
            {
                if (raw.Length < 2) return string.Empty;
                return raw.Slice(1, raw.Length - 2).ToString();
            }

            return UnescapeAndCreateString(raw);
        }

        public ReadOnlySpan<char> GetSpan(int index)
        {
            if (index >= _packedInfo.Length) return ReadOnlySpan<char>.Empty;
            Unpack(_packedInfo[index], out int start, out int length, out bool isQuoted, out _);

            var raw = _line.Slice(start, length);
            if (isQuoted && raw.Length >= 2)
            {
                return raw.Slice(1, raw.Length - 2);
            }
            return raw;
        }

        private string UnescapeAndCreateString(ReadOnlySpan<char> rawQuoted)
        {
            var content = rawQuoted.Slice(1, rawQuoted.Length - 2);

            Span<char> buffer = content.Length <= 512
                ? stackalloc char[content.Length]
                : new char[content.Length];

            int destIdx = 0;
            int srcIdx = 0;
            char escape = _options.EscapeChar;
            char quote = _options.QuoteChar;

            while (srcIdx < content.Length)
            {
                char c = content[srcIdx];

                if (c == escape)
                {
                    if (escape == quote)
                    {
                        if (srcIdx + 1 < content.Length && content[srcIdx + 1] == quote)
                        {
                            buffer[destIdx++] = quote;
                            srcIdx += 2;
                            continue;
                        }
                    }
                    else
                    {
                        if (srcIdx + 1 < content.Length)
                        {
                            buffer[destIdx++] = content[srcIdx + 1];
                            srcIdx += 2;
                            continue;
                        }
                    }
                }

                buffer[destIdx++] = c;
                srcIdx++;
            }

            return new string(buffer.Slice(0, destIdx));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Unpack(long packed, out int start, out int length, out bool isQuoted, out bool needsUnescape)
        {
            isQuoted = (packed & IsQuotedMask) != 0;
            needsUnescape = (packed & NeedsUnescapeMask) != 0;
            start = (int)((packed >> 32) & 0x3FFFFFFF);
            length = (int)(packed & 0xFFFFFFFF);
        }

        public static long Pack(int start, int length, bool isQuoted, bool needsUnescape)
        {
            long info = ((long)start) << 32 | (uint)length;
            if (isQuoted) info |= IsQuotedMask;
            if (needsUnescape) info |= NeedsUnescapeMask;
            return info;
        }
    }

    /// <summary>
    /// Represents the result of a mapping operation.
    /// </summary>
    public readonly struct CsvMappingResult<TEntity>
    {
        private readonly object? _value;
        private readonly int _index;

        private CsvMappingResult(object? value, int index)
        {
            _value = value;
            _index = index;
        }

        public bool IsSuccess => _index == 0;

        public TEntity Result => _index == 0
            ? (TEntity)_value!
            : throw new InvalidOperationException($"Cannot access 'Result' on a failed mapping. Error: {_value}");

        public CsvMappingError Error => _index == 1
            ? (CsvMappingError)_value!
            : throw new InvalidOperationException("Cannot access 'Error' on a successful mapping.");

        public static implicit operator CsvMappingResult<TEntity>(TEntity success) => new(success, 0);
        public static implicit operator CsvMappingResult<TEntity>(CsvMappingError error) => new(error, 1);

        public TResult Match<TResult>(Func<TEntity, TResult> onSuccess, Func<CsvMappingError, TResult> onFailure)
        {
            return _index == 0 ? onSuccess((TEntity)_value!) : onFailure((CsvMappingError)_value!);
        }
    }

    /// <summary>
    /// Represents an error that occurred during mapping.
    /// </summary>
    public class CsvMappingError
    {
        public int ColumnIndex { get; set; }
        public string Value { get; set; } = string.Empty;
        public override string ToString() => $"Error at Column {ColumnIndex}: {Value}";
    }

    /// <summary>
    /// The core interface for the parser to execute mapping logic.
    /// </summary>
    public interface ICsvMapping<TEntity>
    {
        CsvMappingResult<TEntity> Map(ref CsvRow row);
    }

    /// <summary>
    /// Extended interface for mappings that require header resolution.
    /// </summary>
    public interface IHeaderBinder
    {
        bool NeedsHeaderResolution { get; }
        void BindHeaders(ref CsvRow headerRow);
    }

    /// <summary>
    /// Base class for CSV mapping definitions.
    /// </summary>
    public abstract class CsvMapping<TEntity> : ICsvMapping<TEntity>, IHeaderBinder
        where TEntity : class, new()
    {
        private readonly ITypeConverterProvider _typeConverterProvider;
        private readonly List<PropertyMapping> _propertyMappings = new();

        protected CsvMapping() : this(new TypeConverterProvider()) { }

        protected CsvMapping(ITypeConverterProvider typeConverterProvider)
        {
            _typeConverterProvider = typeConverterProvider;
        }

        public bool NeedsHeaderResolution => _propertyMappings.Any(m => m.ColumnIndex == -1);

        public void BindHeaders(ref CsvRow headerRow)
        {
            var headerMap = new Dictionary<string, int>(headerRow.Count, StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < headerRow.Count; i++)
            {
                headerMap[headerRow.GetString(i)] = i;
            }

            foreach (var mapping in _propertyMappings)
            {
                if (mapping.ColumnIndex == -1 && mapping.ColumnName != null)
                {
                    if (headerMap.TryGetValue(mapping.ColumnName, out int index))
                    {
                        mapping.ColumnIndex = index;
                    }
                    else
                    {
                        throw new InvalidOperationException($"The column '{mapping.ColumnName}' was not found in the CSV header.");
                    }
                }
            }
        }

        public CsvMappingResult<TEntity> Map(ref CsvRow row)
        {
            if (NeedsHeaderResolution)
            {
                throw new InvalidOperationException("Mapping contains unresolved header names. Call BindHeaders() first.");
            }

            var entity = new TEntity();
            foreach (var mapping in _propertyMappings)
            {
                if (mapping.ColumnIndex < 0 || mapping.ColumnIndex >= row.Count)
                {
                    return new CsvMappingError { ColumnIndex = mapping.ColumnIndex, Value = "Index Out Of Range" };
                }

                if (!mapping.TryMap(ref row, entity))
                {
                    return new CsvMappingError { ColumnIndex = mapping.ColumnIndex, Value = $"Conversion failed: {row.GetString(mapping.ColumnIndex)}" };
                }
            }
            return entity;
        }

        /// <summary>
        /// Maps a property by its column index using the default converter.
        /// </summary>
        public void MapProperty<TProperty>(int columnIndex, Expression<Func<TEntity, TProperty>> property)
        {
            MapProperty(columnIndex, property, _typeConverterProvider.Resolve<TProperty>());
        }

        /// <summary>
        /// Maps a property by its column index using a specific converter.
        /// </summary>
        public void MapProperty<TProperty>(int columnIndex, Expression<Func<TEntity, TProperty>> property, ITypeConverter<TProperty> converter)
        {
            var setter = CreateSetter(property);
            _propertyMappings.Add(new PropertyMapping<TProperty>(columnIndex, null, converter, setter));
        }

        /// <summary>
        /// Maps a property by its header column name using the default converter.
        /// </summary>
        public void MapProperty<TProperty>(string columnName, Expression<Func<TEntity, TProperty>> property)
        {
            MapProperty(columnName, property, _typeConverterProvider.Resolve<TProperty>());
        }

        /// <summary>
        /// Maps a property by its header column name using a specific converter.
        /// </summary>
        public void MapProperty<TProperty>(string columnName, Expression<Func<TEntity, TProperty>> property, ITypeConverter<TProperty> converter)
        {
            var setter = CreateSetter(property);
            _propertyMappings.Add(new PropertyMapping<TProperty>(-1, columnName, converter, setter));
        }

        private abstract class PropertyMapping
        {
            public int ColumnIndex;
            public string? ColumnName;
            public abstract bool TryMap(ref CsvRow row, TEntity entity);
        }

        private class PropertyMapping<TProperty> : PropertyMapping
        {
            private readonly ITypeConverter<TProperty> _converter;
            private readonly Action<TEntity, TProperty> _setter;

            public PropertyMapping(int index, string? name, ITypeConverter<TProperty> converter, Action<TEntity, TProperty> setter)
            {
                ColumnIndex = index;
                ColumnName = name;
                _converter = converter;
                _setter = setter;
            }

            public override bool TryMap(ref CsvRow row, TEntity entity)
            {
                var span = row.GetSpan(ColumnIndex);
                if (_converter.TryConvert(span, out TProperty? value))
                {
                    _setter(entity, value!);
                    return true;
                }
                return false;
            }
        }

        private static Action<TEntity, TProperty> CreateSetter<TProperty>(Expression<Func<TEntity, TProperty>> property)
        {
            ParameterExpression valueParam = Expression.Parameter(typeof(TProperty), "value");
            BinaryExpression assign = Expression.Assign(property.Body, valueParam);
            return Expression.Lambda<Action<TEntity, TProperty>>(assign, property.Parameters[0], valueParam).Compile();
        }
    }

    #endregion

    #region Converters

    public interface ITypeConverterProvider
    {
        ITypeConverter<T> Resolve<T>();
    }

    public class TypeConverterProvider : ITypeConverterProvider
    {
        private readonly Dictionary<Type, ITypeConverter> _converters = new Dictionary<Type, ITypeConverter>();

        public TypeConverterProvider()
        {
            AddStandardConverters();
        }

        private void AddStandardConverters()
        {
            Register(new SByteConverter());
            Register(new ByteConverter());
            Register(new Int16Converter());
            Register(new UInt16Converter());
            Register(new Int32Converter());
            Register(new UInt32Converter());
            Register(new Int64Converter());
            Register(new UInt64Converter());
            Register(new SingleConverter());
            Register(new DoubleConverter());
            Register(new DecimalConverter());
            Register(new BoolConverter());
            Register(new StringConverter());
            Register(new GuidConverter());
            Register(new TimeSpanConverter());
            Register(new DateTimeConverter());

            Register(new NullableSByteConverter());
            Register(new NullableByteConverter());
            Register(new NullableInt16Converter());
            Register(new NullableUInt16Converter());
            Register(new NullableInt32Converter());
            Register(new NullableUInt32Converter());
            Register(new NullableInt64Converter());
            Register(new NullableUInt64Converter());
            Register(new NullableSingleConverter());
            Register(new NullableDoubleConverter());
            Register(new NullableDecimalConverter());
            Register(new NullableBoolConverter());
            Register(new NullableGuidConverter());
            Register(new NullableTimeSpanConverter());
            Register(new NullableDateTimeConverter());
        }

        public TypeConverterProvider Register<T>(ITypeConverter<T> converter)
        {
            _converters[typeof(T)] = converter;
            return this;
        }

        public ITypeConverter<T> Resolve<T>()
        {
            var type = typeof(T);

            if (_converters.TryGetValue(type, out var converter))
            {
                return (ITypeConverter<T>)converter;
            }

            throw new NotSupportedException($"No TypeConverter for Type '{type.FullName}'.");
        }
    }

    public interface ITypeConverter
    {
    }

    public interface ITypeConverter<TTargetType> : ITypeConverter
    {
        bool TryConvert(ReadOnlySpan<char> value, out TTargetType result);
        Type TargetType { get; }
    }


    public abstract class NonNullableConverter<TTargetType> : ITypeConverter<TTargetType>
    {
        public Type TargetType => typeof(TTargetType);

        public bool TryConvert(ReadOnlySpan<char> value, out TTargetType result)
        {
            if (value.IsEmpty || value.IsWhiteSpace())
            {
                result = default!;
                return false;
            }
            return InternalConvert(value, out result);
        }

        protected abstract bool InternalConvert(ReadOnlySpan<char> value, out TTargetType result);
    }

    public abstract class NullableConverter<TTargetType> : ITypeConverter<TTargetType>
    {
        public Type TargetType => typeof(TTargetType);

        public bool TryConvert(ReadOnlySpan<char> value, out TTargetType result)
        {
            if (value.IsEmpty || value.IsWhiteSpace())
            {
                result = default!;
                return true;
            }
            return InternalConvert(value, out result);
        }

        protected abstract bool InternalConvert(ReadOnlySpan<char> value, out TTargetType result);
    }

    public class EnumConverter<TEnum> : NonNullableConverter<TEnum> where TEnum : struct, Enum
    {
        private readonly (string Name, TEnum Value)[] _enumMap;
        private readonly StringComparison _comparison;

        public EnumConverter() 
            : this(StringComparison.OrdinalIgnoreCase) { }

        public EnumConverter(StringComparison comparison)
        {
            _comparison = comparison;

            var names = Enum.GetNames(typeof(TEnum));
            var values = (TEnum[])Enum.GetValues(typeof(TEnum));

            _enumMap = new (string, TEnum)[names.Length];

            for (int i = 0; i < names.Length; i++)
            {
                _enumMap[i] = (names[i], values[i]);
            }
        }

        public EnumConverter(IDictionary<string, TEnum> customMap, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            _comparison = comparison;
            _enumMap = new (string, TEnum)[customMap.Count];
            int i = 0;
            foreach (var kvp in customMap)
            {
                _enumMap[i++] = (kvp.Key, kvp.Value);
            }
        }

        protected override bool InternalConvert(ReadOnlySpan<char> value, out TEnum result)
        {
            for (int i = 0; i < _enumMap.Length; i++)
            {
                if (value.Equals(_enumMap[i].Name.AsSpan(), _comparison))
                {
                    result = _enumMap[i].Value;
                    return true;
                }
            }

            if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int numericValue))
            {
                result = Unsafe.As<int, TEnum>(ref numericValue);
                return true;
            }

            result = default;
            return false;
        }
    }

    public class NullableEnumConverter<TEnum> : NullableConverter<TEnum?> where TEnum : struct, Enum
    {
        private readonly EnumConverter<TEnum> _baseConverter;

        public NullableEnumConverter() : this(StringComparison.OrdinalIgnoreCase) { }

        public NullableEnumConverter(StringComparison comparison)
        {
            _baseConverter = new EnumConverter<TEnum>(comparison);
        }

        public NullableEnumConverter(IDictionary<string, TEnum> customMap, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            _baseConverter = new EnumConverter<TEnum>(customMap, comparison);
        }

        protected override bool InternalConvert(ReadOnlySpan<char> value, out TEnum? result)
        {
            if (_baseConverter.TryConvert(value, out TEnum tempResult))
            {
                result = tempResult;
                return true;
            }
            result = null;
            return false;
        }
    }

    public class BoolConverter : NonNullableConverter<bool>
    {
        private readonly string _trueValue;
        private readonly string _falseValue;
        private readonly StringComparison _stringComparison;

        public BoolConverter()
            : this("true", "false", StringComparison.OrdinalIgnoreCase)
        {
        }

        public BoolConverter(string trueValue, string falseValue, StringComparison stringComparison)
        {
            _trueValue = trueValue;
            _falseValue = falseValue;
            _stringComparison = stringComparison;
        }

        protected override bool InternalConvert(ReadOnlySpan<char> value, out bool result)
        {
            if (value.Equals(_trueValue.AsSpan(), _stringComparison))
            {
                result = true;
                return true;
            }

            if (value.Equals(_falseValue.AsSpan(), _stringComparison))
            {
                result = false;
                return true;
            }

            result = false;
            return false;
        }
    }

    public class NullableBoolConverter : NullableConverter<bool?>
    {
        private readonly string _trueValue;
        private readonly string _falseValue;
        private readonly StringComparison _stringComparison;

        public NullableBoolConverter()
            : this("true", "false", StringComparison.OrdinalIgnoreCase)
        {
        }

        public NullableBoolConverter(string trueValue, string falseValue, StringComparison stringComparison)
        {
            _trueValue = trueValue;
            _falseValue = falseValue;
            _stringComparison = stringComparison;
        }

        protected override bool InternalConvert(ReadOnlySpan<char> value, out bool? result)
        {
            if (value.Equals(_trueValue.AsSpan(), _stringComparison))
            {
                result = true;
                return true;
            }

            if (value.Equals(_falseValue.AsSpan(), _stringComparison))
            {
                result = false;
                return true;
            }

            result = null;
            return false;
        }
    }

    public class ByteConverter : NonNullableConverter<byte>
    {
        private readonly IFormatProvider _formatProvider;
        private readonly NumberStyles _numberStyles;

        public ByteConverter() : this(CultureInfo.InvariantCulture) { }
        public ByteConverter(IFormatProvider formatProvider) : this(formatProvider, NumberStyles.Integer) { }
        public ByteConverter(IFormatProvider formatProvider, NumberStyles numberStyles)
        {
            _formatProvider = formatProvider;
            _numberStyles = numberStyles;
        }

        protected override bool InternalConvert(ReadOnlySpan<char> value, out byte result)
        {
            return byte.TryParse(value, _numberStyles, _formatProvider, out result);
        }
    }

    public class NullableByteConverter : NullableConverter<byte?>
    {
        private readonly IFormatProvider _formatProvider;
        private readonly NumberStyles _numberStyles;

        public NullableByteConverter() : this(CultureInfo.InvariantCulture) { }
        public NullableByteConverter(IFormatProvider formatProvider) : this(formatProvider, NumberStyles.Integer) { }
        public NullableByteConverter(IFormatProvider formatProvider, NumberStyles numberStyles)
        {
            _formatProvider = formatProvider;
            _numberStyles = numberStyles;
        }

        protected override bool InternalConvert(ReadOnlySpan<char> value, out byte? result)
        {
            if (byte.TryParse(value, _numberStyles, _formatProvider, out byte tempResult))
            {
                result = tempResult;
                return true;
            }
            result = null;
            return false;
        }
    }

    public class SByteConverter : NonNullableConverter<sbyte>
    {
        private readonly IFormatProvider _formatProvider;
        private readonly NumberStyles _numberStyles;

        public SByteConverter() : this(CultureInfo.InvariantCulture) { }
        public SByteConverter(IFormatProvider formatProvider) : this(formatProvider, NumberStyles.Integer) { }
        public SByteConverter(IFormatProvider formatProvider, NumberStyles numberStyles)
        {
            _formatProvider = formatProvider;
            _numberStyles = numberStyles;
        }

        protected override bool InternalConvert(ReadOnlySpan<char> value, out sbyte result)
        {
            return sbyte.TryParse(value, _numberStyles, _formatProvider, out result);
        }
    }

    public class NullableSByteConverter : NullableConverter<sbyte?>
    {
        private readonly IFormatProvider _formatProvider;
        private readonly NumberStyles _numberStyles;

        public NullableSByteConverter() : this(CultureInfo.InvariantCulture) { }
        public NullableSByteConverter(IFormatProvider formatProvider) : this(formatProvider, NumberStyles.Integer) { }
        public NullableSByteConverter(IFormatProvider formatProvider, NumberStyles numberStyles)
        {
            _formatProvider = formatProvider;
            _numberStyles = numberStyles;
        }

        protected override bool InternalConvert(ReadOnlySpan<char> value, out sbyte? result)
        {
            if (sbyte.TryParse(value, _numberStyles, _formatProvider, out sbyte tempResult))
            {
                result = tempResult;
                return true;
            }
            result = null;
            return false;
        }
    }

    public class Int16Converter : NonNullableConverter<short>
    {
        private readonly IFormatProvider _formatProvider;
        private readonly NumberStyles _numberStyles;

        public Int16Converter() : this(CultureInfo.InvariantCulture) { }
        public Int16Converter(IFormatProvider formatProvider) : this(formatProvider, NumberStyles.Integer) { }
        public Int16Converter(IFormatProvider formatProvider, NumberStyles numberStyles)
        {
            _formatProvider = formatProvider;
            _numberStyles = numberStyles;
        }

        protected override bool InternalConvert(ReadOnlySpan<char> value, out short result)
        {
            return short.TryParse(value, _numberStyles, _formatProvider, out result);
        }
    }

    public class NullableInt16Converter : NullableConverter<short?>
    {
        private readonly IFormatProvider _formatProvider;
        private readonly NumberStyles _numberStyles;

        public NullableInt16Converter() : this(CultureInfo.InvariantCulture) { }
        public NullableInt16Converter(IFormatProvider formatProvider) : this(formatProvider, NumberStyles.Integer) { }
        public NullableInt16Converter(IFormatProvider formatProvider, NumberStyles numberStyles)
        {
            _formatProvider = formatProvider;
            _numberStyles = numberStyles;
        }

        protected override bool InternalConvert(ReadOnlySpan<char> value, out short? result)
        {
            if (short.TryParse(value, _numberStyles, _formatProvider, out short tempResult))
            {
                result = tempResult;
                return true;
            }
            result = null;
            return false;
        }
    }

    public class Int32Converter : NonNullableConverter<int>
    {
        private readonly IFormatProvider _formatProvider;
        private readonly NumberStyles _numberStyles;

        public Int32Converter() : this(CultureInfo.InvariantCulture) { }
        public Int32Converter(IFormatProvider formatProvider) : this(formatProvider, NumberStyles.Integer) { }
        public Int32Converter(IFormatProvider formatProvider, NumberStyles numberStyles)
        {
            _formatProvider = formatProvider;
            _numberStyles = numberStyles;
        }

        protected override bool InternalConvert(ReadOnlySpan<char> value, out int result)
        {
            return int.TryParse(value, _numberStyles, _formatProvider, out result);
        }
    }

    public class NullableInt32Converter : NullableConverter<int?>
    {
        private readonly IFormatProvider _formatProvider;
        private readonly NumberStyles _numberStyles;

        public NullableInt32Converter() : this(CultureInfo.InvariantCulture) { }
        public NullableInt32Converter(IFormatProvider formatProvider) : this(formatProvider, NumberStyles.Integer) { }
        public NullableInt32Converter(IFormatProvider formatProvider, NumberStyles numberStyles)
        {
            _formatProvider = formatProvider;
            _numberStyles = numberStyles;
        }

        protected override bool InternalConvert(ReadOnlySpan<char> value, out int? result)
        {
            if (int.TryParse(value, _numberStyles, _formatProvider, out int tempResult))
            {
                result = tempResult;
                return true;
            }
            result = null;
            return false;
        }
    }

    public class Int64Converter : NonNullableConverter<long>
    {
        private readonly IFormatProvider _formatProvider;
        private readonly NumberStyles _numberStyles;

        public Int64Converter() : this(CultureInfo.InvariantCulture) { }
        public Int64Converter(IFormatProvider formatProvider) : this(formatProvider, NumberStyles.Integer) { }
        public Int64Converter(IFormatProvider formatProvider, NumberStyles numberStyles)
        {
            _formatProvider = formatProvider;
            _numberStyles = numberStyles;
        }

        protected override bool InternalConvert(ReadOnlySpan<char> value, out long result)
        {
            return long.TryParse(value, _numberStyles, _formatProvider, out result);
        }
    }

    public class NullableInt64Converter : NullableConverter<long?>
    {
        private readonly IFormatProvider _formatProvider;
        private readonly NumberStyles _numberStyles;

        public NullableInt64Converter() : this(CultureInfo.InvariantCulture) { }
        public NullableInt64Converter(IFormatProvider formatProvider) : this(formatProvider, NumberStyles.Integer) { }
        public NullableInt64Converter(IFormatProvider formatProvider, NumberStyles numberStyles)
        {
            _formatProvider = formatProvider;
            _numberStyles = numberStyles;
        }

        protected override bool InternalConvert(ReadOnlySpan<char> value, out long? result)
        {
            if (long.TryParse(value, _numberStyles, _formatProvider, out long tempResult))
            {
                result = tempResult;
                return true;
            }
            result = null;
            return false;
        }
    }

    public class UInt16Converter : NonNullableConverter<ushort>
    {
        private readonly IFormatProvider _formatProvider;
        private readonly NumberStyles _numberStyles;

        public UInt16Converter() : this(CultureInfo.InvariantCulture) { }
        public UInt16Converter(IFormatProvider formatProvider) : this(formatProvider, NumberStyles.Integer) { }
        public UInt16Converter(IFormatProvider formatProvider, NumberStyles numberStyles)
        {
            _formatProvider = formatProvider;
            _numberStyles = numberStyles;
        }

        protected override bool InternalConvert(ReadOnlySpan<char> value, out ushort result)
        {
            return ushort.TryParse(value, _numberStyles, _formatProvider, out result);
        }
    }

    public class NullableUInt16Converter : NullableConverter<ushort?>
    {
        private readonly IFormatProvider _formatProvider;
        private readonly NumberStyles _numberStyles;

        public NullableUInt16Converter() : this(CultureInfo.InvariantCulture) { }
        public NullableUInt16Converter(IFormatProvider formatProvider) : this(formatProvider, NumberStyles.Integer) { }
        public NullableUInt16Converter(IFormatProvider formatProvider, NumberStyles numberStyles)
        {
            _formatProvider = formatProvider;
            _numberStyles = numberStyles;
        }

        protected override bool InternalConvert(ReadOnlySpan<char> value, out ushort? result)
        {
            if (ushort.TryParse(value, _numberStyles, _formatProvider, out ushort tempResult))
            {
                result = tempResult;
                return true;
            }
            result = null;
            return false;
        }
    }

    public class UInt32Converter : NonNullableConverter<uint>
    {
        private readonly IFormatProvider _formatProvider;
        private readonly NumberStyles _numberStyles;

        public UInt32Converter() : this(CultureInfo.InvariantCulture) { }
        public UInt32Converter(IFormatProvider formatProvider) : this(formatProvider, NumberStyles.Integer) { }
        public UInt32Converter(IFormatProvider formatProvider, NumberStyles numberStyles)
        {
            _formatProvider = formatProvider;
            _numberStyles = numberStyles;
        }

        protected override bool InternalConvert(ReadOnlySpan<char> value, out uint result)
        {
            return uint.TryParse(value, _numberStyles, _formatProvider, out result);
        }
    }

    public class NullableUInt32Converter : NullableConverter<uint?>
    {
        private readonly IFormatProvider _formatProvider;
        private readonly NumberStyles _numberStyles;

        public NullableUInt32Converter() : this(CultureInfo.InvariantCulture) { }
        public NullableUInt32Converter(IFormatProvider formatProvider) : this(formatProvider, NumberStyles.Integer) { }
        public NullableUInt32Converter(IFormatProvider formatProvider, NumberStyles numberStyles)
        {
            _formatProvider = formatProvider;
            _numberStyles = numberStyles;
        }

        protected override bool InternalConvert(ReadOnlySpan<char> value, out uint? result)
        {
            if (uint.TryParse(value, _numberStyles, _formatProvider, out uint tempResult))
            {
                result = tempResult;
                return true;
            }
            result = null;
            return false;
        }
    }

    public class UInt64Converter : NonNullableConverter<ulong>
    {
        private readonly IFormatProvider _formatProvider;
        private readonly NumberStyles _numberStyles;

        public UInt64Converter() : this(CultureInfo.InvariantCulture) { }
        public UInt64Converter(IFormatProvider formatProvider) : this(formatProvider, NumberStyles.Integer) { }
        public UInt64Converter(IFormatProvider formatProvider, NumberStyles numberStyles)
        {
            _formatProvider = formatProvider;
            _numberStyles = numberStyles;
        }

        protected override bool InternalConvert(ReadOnlySpan<char> value, out ulong result)
        {
            return ulong.TryParse(value, _numberStyles, _formatProvider, out result);
        }
    }

    public class NullableUInt64Converter : NullableConverter<ulong?>
    {
        private readonly IFormatProvider _formatProvider;
        private readonly NumberStyles _numberStyles;

        public NullableUInt64Converter() : this(CultureInfo.InvariantCulture) { }
        public NullableUInt64Converter(IFormatProvider formatProvider) : this(formatProvider, NumberStyles.Integer) { }
        public NullableUInt64Converter(IFormatProvider formatProvider, NumberStyles numberStyles)
        {
            _formatProvider = formatProvider;
            _numberStyles = numberStyles;
        }

        protected override bool InternalConvert(ReadOnlySpan<char> value, out ulong? result)
        {
            if (ulong.TryParse(value, _numberStyles, _formatProvider, out ulong tempResult))
            {
                result = tempResult;
                return true;
            }
            result = null;
            return false;
        }
    }

    public class SingleConverter : NonNullableConverter<float>
    {
        private readonly IFormatProvider _formatProvider;
        private readonly NumberStyles _numberStyles;

        public SingleConverter() : this(CultureInfo.InvariantCulture) { }
        public SingleConverter(IFormatProvider formatProvider) : this(formatProvider, NumberStyles.Float | NumberStyles.AllowThousands) { }
        public SingleConverter(IFormatProvider formatProvider, NumberStyles numberStyles)
        {
            _formatProvider = formatProvider;
            _numberStyles = numberStyles;
        }

        protected override bool InternalConvert(ReadOnlySpan<char> value, out float result)
        {
            return float.TryParse(value, _numberStyles, _formatProvider, out result);
        }
    }

    public class NullableSingleConverter : NullableConverter<float?>
    {
        private readonly IFormatProvider _formatProvider;
        private readonly NumberStyles _numberStyles;

        public NullableSingleConverter() : this(CultureInfo.InvariantCulture) { }
        public NullableSingleConverter(IFormatProvider formatProvider) : this(formatProvider, NumberStyles.Float | NumberStyles.AllowThousands) { }
        public NullableSingleConverter(IFormatProvider formatProvider, NumberStyles numberStyles)
        {
            _formatProvider = formatProvider;
            _numberStyles = numberStyles;
        }

        protected override bool InternalConvert(ReadOnlySpan<char> value, out float? result)
        {
            if (float.TryParse(value, _numberStyles, _formatProvider, out float tempResult))
            {
                result = tempResult;
                return true;
            }
            result = null;
            return false;
        }
    }

    public class DoubleConverter : NonNullableConverter<double>
    {
        private readonly IFormatProvider _formatProvider;
        private readonly NumberStyles _numberStyles;

        public DoubleConverter() : this(CultureInfo.InvariantCulture) { }
        public DoubleConverter(IFormatProvider formatProvider) : this(formatProvider, NumberStyles.Float | NumberStyles.AllowThousands) { }
        public DoubleConverter(IFormatProvider formatProvider, NumberStyles numberStyles)
        {
            _formatProvider = formatProvider;
            _numberStyles = numberStyles;
        }

        protected override bool InternalConvert(ReadOnlySpan<char> value, out double result)
        {
            return double.TryParse(value, _numberStyles, _formatProvider, out result);
        }
    }

    public class NullableDoubleConverter : NullableConverter<double?>
    {
        private readonly IFormatProvider _formatProvider;
        private readonly NumberStyles _numberStyles;

        public NullableDoubleConverter() : this(CultureInfo.InvariantCulture) { }
        public NullableDoubleConverter(IFormatProvider formatProvider) : this(formatProvider, NumberStyles.Float | NumberStyles.AllowThousands) { }
        public NullableDoubleConverter(IFormatProvider formatProvider, NumberStyles numberStyles)
        {
            _formatProvider = formatProvider;
            _numberStyles = numberStyles;
        }

        protected override bool InternalConvert(ReadOnlySpan<char> value, out double? result)
        {
            if (double.TryParse(value, _numberStyles, _formatProvider, out double tempResult))
            {
                result = tempResult;
                return true;
            }
            result = null;
            return false;
        }
    }

    public class DecimalConverter : NonNullableConverter<decimal>
    {
        private readonly IFormatProvider _formatProvider;
        private readonly NumberStyles _numberStyles;

        public DecimalConverter() : this(CultureInfo.InvariantCulture) { }
        public DecimalConverter(IFormatProvider formatProvider) : this(formatProvider, NumberStyles.Number) { }
        public DecimalConverter(IFormatProvider formatProvider, NumberStyles numberStyles)
        {
            _formatProvider = formatProvider;
            _numberStyles = numberStyles;
        }

        protected override bool InternalConvert(ReadOnlySpan<char> value, out decimal result)
        {
            return decimal.TryParse(value, _numberStyles, _formatProvider, out result);
        }
    }

    public class NullableDecimalConverter : NullableConverter<decimal?>
    {
        private readonly IFormatProvider _formatProvider;
        private readonly NumberStyles _numberStyles;

        public NullableDecimalConverter() : this(CultureInfo.InvariantCulture) { }
        public NullableDecimalConverter(IFormatProvider formatProvider) : this(formatProvider, NumberStyles.Number) { }
        public NullableDecimalConverter(IFormatProvider formatProvider, NumberStyles numberStyles)
        {
            _formatProvider = formatProvider;
            _numberStyles = numberStyles;
        }

        protected override bool InternalConvert(ReadOnlySpan<char> value, out decimal? result)
        {
            if (decimal.TryParse(value, _numberStyles, _formatProvider, out decimal tempResult))
            {
                result = tempResult;
                return true;
            }
            result = null;
            return false;
        }
    }

    public class StringConverter : ITypeConverter<string>
    {
        public Type TargetType => typeof(string);

        public bool TryConvert(ReadOnlySpan<char> value, out string result)
        {
            result = value.ToString();
            return true;
        }
    }

    public class GuidConverter : NonNullableConverter<Guid>
    {
        private readonly string _format;
        public GuidConverter() : this(string.Empty) { }
        public GuidConverter(string format) { _format = format; }

        protected override bool InternalConvert(ReadOnlySpan<char> value, out Guid result)
        {
            if (string.IsNullOrEmpty(_format)) return Guid.TryParse(value, out result);
            return Guid.TryParseExact(value, _format.AsSpan(), out result);
        }
    }

    public class NullableGuidConverter : NullableConverter<Guid?>
    {
        private readonly string _format;
        public NullableGuidConverter() : this(string.Empty) { }
        public NullableGuidConverter(string format) { _format = format; }

        protected override bool InternalConvert(ReadOnlySpan<char> value, out Guid? result)
        {
            if (string.IsNullOrEmpty(_format))
            {
                if (Guid.TryParse(value, out Guid tempResult)) { result = tempResult; return true; }
            }
            else
            {
                if (Guid.TryParseExact(value, _format.AsSpan(), out Guid tempResult)) { result = tempResult; return true; }
            }
            result = null; return false;
        }
    }

    public class TimeSpanConverter : NonNullableConverter<TimeSpan>
    {
        private readonly string _format;
        private readonly IFormatProvider _formatProvider;
        private readonly TimeSpanStyles _styles;

        public TimeSpanConverter() : this(string.Empty) { }
        public TimeSpanConverter(string format) : this(format, CultureInfo.InvariantCulture) { }
        public TimeSpanConverter(string format, IFormatProvider formatProvider) : this(format, formatProvider, TimeSpanStyles.None) { }
        public TimeSpanConverter(string format, IFormatProvider formatProvider, TimeSpanStyles styles)
        {
            _format = format; _formatProvider = formatProvider; _styles = styles;
        }

        protected override bool InternalConvert(ReadOnlySpan<char> value, out TimeSpan result)
        {
            if (string.IsNullOrEmpty(_format)) return TimeSpan.TryParse(value, _formatProvider, out result);
            return TimeSpan.TryParseExact(value, _format.AsSpan(), _formatProvider, _styles, out result);
        }
    }

    public class NullableTimeSpanConverter : NullableConverter<TimeSpan?>
    {
        private readonly string _format;
        private readonly IFormatProvider _formatProvider;
        private readonly TimeSpanStyles _styles;

        public NullableTimeSpanConverter() : this(string.Empty) { }
        public NullableTimeSpanConverter(string format) : this(format, CultureInfo.InvariantCulture) { }
        public NullableTimeSpanConverter(string format, IFormatProvider formatProvider) : this(format, formatProvider, TimeSpanStyles.None) { }
        public NullableTimeSpanConverter(string format, IFormatProvider formatProvider, TimeSpanStyles styles)
        {
            _format = format; _formatProvider = formatProvider; _styles = styles;
        }

        protected override bool InternalConvert(ReadOnlySpan<char> value, out TimeSpan? result)
        {
            if (string.IsNullOrEmpty(_format))
            {
                if (TimeSpan.TryParse(value, _formatProvider, out TimeSpan tempResult)) { result = tempResult; return true; }
            }
            else
            {
                if (TimeSpan.TryParseExact(value, _format.AsSpan(), _formatProvider, _styles, out TimeSpan tempResult)) { result = tempResult; return true; }
            }
            result = null; return false;
        }
    }

    public class DateTimeConverter : NonNullableConverter<DateTime>
    {
        private readonly string _dateTimeFormat;
        private readonly IFormatProvider _formatProvider;
        private readonly DateTimeStyles _dateTimeStyles;

        public DateTimeConverter() : this(string.Empty) { }
        public DateTimeConverter(string dateTimeFormat) : this(dateTimeFormat, CultureInfo.InvariantCulture) { }
        public DateTimeConverter(string dateTimeFormat, IFormatProvider formatProvider) : this(dateTimeFormat, formatProvider, DateTimeStyles.None) { }
        public DateTimeConverter(string dateTimeFormat, IFormatProvider formatProvider, DateTimeStyles dateTimeStyles)
        {
            _dateTimeFormat = dateTimeFormat; _formatProvider = formatProvider; _dateTimeStyles = dateTimeStyles;
        }

        protected override bool InternalConvert(ReadOnlySpan<char> value, out DateTime result)
        {
            if (string.IsNullOrWhiteSpace(_dateTimeFormat)) return DateTime.TryParse(value, _formatProvider, _dateTimeStyles, out result);
            return DateTime.TryParseExact(value, _dateTimeFormat.AsSpan(), _formatProvider, _dateTimeStyles, out result);
        }
    }

    public class NullableDateTimeConverter : NullableConverter<DateTime?>
    {
        private readonly string _dateTimeFormat;
        private readonly IFormatProvider _formatProvider;
        private readonly DateTimeStyles _dateTimeStyles;

        public NullableDateTimeConverter() : this(string.Empty) { }
        public NullableDateTimeConverter(string dateTimeFormat) : this(dateTimeFormat, CultureInfo.InvariantCulture) { }
        public NullableDateTimeConverter(string dateTimeFormat, IFormatProvider formatProvider) : this(dateTimeFormat, formatProvider, DateTimeStyles.None) { }
        public NullableDateTimeConverter(string dateTimeFormat, IFormatProvider formatProvider, DateTimeStyles dateTimeStyles)
        {
            _dateTimeFormat = dateTimeFormat; _formatProvider = formatProvider; _dateTimeStyles = dateTimeStyles;
        }

        protected override bool InternalConvert(ReadOnlySpan<char> value, out DateTime? result)
        {
            if (string.IsNullOrWhiteSpace(_dateTimeFormat))
            {
                if (DateTime.TryParse(value, _formatProvider, _dateTimeStyles, out DateTime tempResult)) { result = tempResult; return true; }
            }
            else
            {
                if (DateTime.TryParseExact(value, _dateTimeFormat.AsSpan(), _formatProvider, _dateTimeStyles, out DateTime tempResult)) { result = tempResult; return true; }
            }
            result = null; return false;
        }
    }

    #endregion

    #region Extensions
    
    internal static class SpanExtensions
    {
        public static bool IsWhiteSpace(this ReadOnlySpan<char> span)
        {
            foreach (var c in span)
            {
                if (!char.IsWhiteSpace(c)) return false;
            }
            return true;
        }
    }

    #endregion
}

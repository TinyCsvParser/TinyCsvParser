// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TinyCsvParser.Core;
using TinyCsvParser.Internals;
using TinyCsvParser.Models;

namespace TinyCsvParser;

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



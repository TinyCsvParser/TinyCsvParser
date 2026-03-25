using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipelines;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TinyCsvParser.Core;
using TinyCsvParser.Models;

namespace TinyCsvParser;

public class CsvParser<TEntity> where TEntity : class, new()
{
    private readonly CsvOptions _options;
    private readonly ICsvMapping<TEntity> _mapping;
    private readonly Encoding _encoding;

    private enum ParseState { Normal, InQuotedField, AfterQuote }

    public CsvParser(CsvOptions options, ICsvMapping<TEntity> mapping)
    {
        _options = options;
        _mapping = mapping;
        _encoding = options.Encoding ?? Encoding.UTF8;
    }

    public IEnumerable<CsvMappingResult<TEntity>> ReadFromFile(string filePath)
    {
        using FileStream stream = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

        foreach (CsvMappingResult<TEntity> result in Read(stream))
        {
            yield return result;
        }
    }

    public async IAsyncEnumerable<CsvMappingResult<TEntity>> ReadFromFileAsync(string filePath, [EnumeratorCancellation] CancellationToken ct = default)
    {
        using FileStream stream = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

        await foreach (CsvMappingResult<TEntity> result in ReadAsync(stream, ct).ConfigureAwait(false))
        {
            yield return result;
        }
    }

    public IEnumerable<CsvMappingResult<TEntity>> ReadFromString(string csvContent)
    {
        using MemoryStream stream = new(_encoding.GetBytes(csvContent));

        foreach (CsvMappingResult<TEntity> result in Read(stream))
        {
            yield return result;
        }
    }

    public async IAsyncEnumerable<CsvMappingResult<TEntity>> ReadFromStringAsync(string csvContent, [EnumeratorCancellation] CancellationToken ct = default)
    {
        using MemoryStream stream = new(_encoding.GetBytes(csvContent));

        await foreach (CsvMappingResult<TEntity> result in ReadAsync(stream, ct).ConfigureAwait(false))
        {
            yield return result;
        }
    }

    public async IAsyncEnumerable<CsvMappingResult<TEntity>> ReadAsync(Stream stream, [EnumeratorCancellation] CancellationToken ct = default)
    {
        PipeReader reader = PipeReader.Create(stream);

        CsvFieldRange[] rangeBuffer = new CsvFieldRange[256];

        char[] charBuffer = ArrayPool<char>.Shared.Rent(4096);

        int recordIndex = 0;
        int lineNumber = 1;
        bool isInitialized = false;

        CsvHeaderResolution? headerResolution = null;

        try
        {
            while (true)
            {
                ReadResult result = await reader.ReadAsync(ct).ConfigureAwait(false);

                ReadOnlySequence<byte> buffer = result.Buffer;

                while (TryReadLogicalRecord(ref buffer, out ReadOnlySequence<byte> lineSequence, out int linesInRecord, out bool isMalformed))
                {
                    int charCount = DecodeToBuffer(lineSequence, ref charBuffer);

                    if (isMalformed)
                    {
                        yield return new CsvMappingResult<TEntity>(
                            new CsvMappingError(recordIndex++, lineNumber, 0, "Unclosed quote in record"),
                            recordIndex - 1, lineNumber);
                        lineNumber += linesInRecord;
                        continue;
                    }

                    if (InvokeProcessLine(charBuffer, charCount, rangeBuffer, ref recordIndex, ref lineNumber, linesInRecord, ref isInitialized, ref headerResolution, out CsvMappingResult<TEntity> mappingResult))
                    {
                        yield return mappingResult;
                    }
                }

                reader.AdvanceTo(buffer.Start, buffer.End);

                if (result.IsCompleted)
                {
                    if (!buffer.IsEmpty)
                    {
                        int charCount = DecodeToBuffer(buffer, ref charBuffer);

                        if (InvokeProcessLine(charBuffer, charCount, rangeBuffer, ref recordIndex, ref lineNumber, 1, ref isInitialized, ref headerResolution, out CsvMappingResult<TEntity> lastResult))
                        {
                            yield return lastResult;
                        }
                    }
                    break;
                }
            }
        }
        finally
        {
            await reader.CompleteAsync().ConfigureAwait(false);

            ArrayPool<char>.Shared.Return(charBuffer);
        }
    }

    public IEnumerable<CsvMappingResult<TEntity>> Read(Stream stream)
    {
        using StreamReader reader = new(stream, _encoding, true);

        CsvFieldRange[] rangeBuffer = new CsvFieldRange[256];

        int recordIndex = 0, lineNumber = 1;
        bool isInitialized = false;

        CsvHeaderResolution? headerResolution = null;

        StringBuilder sb = new();

        while (true)
        {
            (string? rawLine, int linesConsumed, bool isMalformed) = ReadLogicalRecordSync(reader, sb);
            if (rawLine == null && linesConsumed == 0)
            {
                break;
            }

            if (isMalformed)
            {
                yield return new CsvMappingResult<TEntity>(
                    new CsvMappingError(recordIndex++, lineNumber, 0, "Unclosed quote at end of file"),
                    recordIndex - 1, lineNumber);
                lineNumber += linesConsumed;
                continue;
            }

            if (InvokeProcessLine(rawLine!, rangeBuffer, ref recordIndex, ref lineNumber, linesConsumed, ref isInitialized, ref headerResolution, out CsvMappingResult<TEntity> res))
            {
                yield return res;
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool InvokeProcessLine(char[] buffer, int length, CsvFieldRange[] rangeBuffer, ref int recordIdx, ref int lineNo, int consumed, ref bool initialized, ref CsvHeaderResolution? headerResolution, out CsvMappingResult<TEntity> result)
    {
        return ProcessLine(new ReadOnlySpan<char>(buffer, 0, length), rangeBuffer, ref recordIdx, ref lineNo, consumed, ref initialized, ref headerResolution, out result);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool InvokeProcessLine(string line, CsvFieldRange[] rangeBuffer, ref int recordIdx, ref int lineNo, int consumed, ref bool initialized, ref CsvHeaderResolution? headerResolution, out CsvMappingResult<TEntity> result)
    {
        return ProcessLine(line.AsSpan(), rangeBuffer, ref recordIdx, ref lineNo, consumed, ref initialized, ref headerResolution, out result);
    }

    private bool ProcessLine(ReadOnlySpan<char> lineSpan, CsvFieldRange[] rangeBuffer, ref int recordIdx, ref int lineNo, int consumed, ref bool initialized, ref CsvHeaderResolution? headerResolution, out CsvMappingResult<TEntity> result)
    {
        result = default;

        if (IsComment(lineSpan))
        {
            result = new CsvMappingResult<TEntity>(lineSpan.ToString(), -1, lineNo);
            lineNo += consumed;
            return true;
        }

        if (!initialized)
        {
            initialized = true;

            if (_mapping is IHeaderBinder binder && binder.NeedsHeaderResolution)
            {
                int count = SplitLine(lineSpan, rangeBuffer, out _);
            
                CsvRow headerRow = new(lineSpan, rangeBuffer.AsSpan(0, count), _options, recordIdx, lineNo);

                headerResolution = binder.BindHeaders(ref headerRow);
                lineNo += consumed;
                return false;
            }

            if (_options.SkipHeader)
            {
                lineNo += consumed;
                return false;
            }
        }

        int countSplit = SplitLine(lineSpan, rangeBuffer, out bool overflow);
        
        if (overflow)
        {
            result = new CsvMappingResult<TEntity>(new CsvMappingError(recordIdx++, lineNo, -1, "Buffer overflow"), recordIdx - 1, lineNo);
            lineNo += consumed;
            return true;
        }

        CsvRow row = new(lineSpan, rangeBuffer.AsSpan(0, countSplit), _options, recordIdx++, lineNo);

        try
        {
            result = _mapping.Map(ref row, headerResolution);
            lineNo += consumed;
            return true;
        }
        catch (Exception ex)
        {
            result = new CsvMappingResult<TEntity>(new CsvMappingError(recordIdx - 1, lineNo, -1, ex.Message), recordIdx - 1, lineNo);
            lineNo += consumed;
            return true;
        }
    }

    private (string? Content, int NewLines, bool IsMalformed) ReadLogicalRecordSync(StreamReader reader, StringBuilder sb)
    {
        sb.Clear();
        int lines = 0;
        bool inQuotes = false;
        string? line;

        while ((line = reader.ReadLine()) != null)
        {
            lines++;
            sb.Append(line);

            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] == _options.QuoteChar)
                {
                    inQuotes = !inQuotes;
                }
            }

            if (!inQuotes)
            {
                return (sb.ToString(), lines, false);
            }

            sb.Append('\n');
        }

        return (sb.Length > 0 ? (sb.ToString(), lines, inQuotes) : (null, 0, false));
    }

    private bool TryReadLogicalRecord(ref ReadOnlySequence<byte> buffer, out ReadOnlySequence<byte> lineSequence, out int linesConsumed, out bool isMalformed)
    {
        lineSequence = default;
        linesConsumed = 0;
        isMalformed = false;

        SequenceReader<byte> reader = new(buffer);
        bool inQuotes = false;
        byte quote = (byte)_options.QuoteChar;
        byte lf = (byte)'\n';
        byte cr = (byte)'\r';

        while (!reader.End)
        {
            if (reader.TryRead(out byte b))
            {
                if (b == quote)
                {
                    inQuotes = !inQuotes;
                }

                if (b == lf)
                {
                    linesConsumed++;
                }

                if (!inQuotes)
                {
                    if (b == lf || b == cr)
                    {
                        if (b == cr && reader.IsNext(lf, advancePast: true))
                        {
                            linesConsumed++;
                        }

                        lineSequence = buffer.Slice(0, reader.Consumed);
                        buffer = buffer.Slice(reader.Position);
                        return true;
                    }
                }
            }
        }

        if (inQuotes)
        {
            isMalformed = true;
        }

        return false;
    }

    private int DecodeToBuffer(ReadOnlySequence<byte> sequence, ref char[] buffer)
    {
        int byteCount = (int)sequence.Length;
        int maxCharCount = _encoding.GetMaxCharCount(byteCount);

        if (buffer.Length < maxCharCount)
        {
            ArrayPool<char>.Shared.Return(buffer);
            buffer = ArrayPool<char>.Shared.Rent(maxCharCount);
        }

        int actualChars = 0;
        foreach (ReadOnlyMemory<byte> memory in sequence)
        {
            actualChars += _encoding.GetChars(memory.Span, buffer.AsSpan(actualChars));
        }
        return actualChars;
    }

    private int SplitLine(ReadOnlySpan<char> line, Span<CsvFieldRange> ranges, out bool overflow)
    {
        int rangeCount = 0, fieldStart = 0;
        ParseState state = ParseState.Normal;
        bool needsUnescape = false;
        overflow = false;

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];
            if (rangeCount >= ranges.Length) { overflow = true; return rangeCount; }

            switch (state)
            {
                case ParseState.Normal:
                    if (c == _options.QuoteChar) { state = ParseState.InQuotedField; fieldStart = i; }
                    else if (c == _options.Delimiter)
                    {
                        ranges[rangeCount++] = new CsvFieldRange(fieldStart, i - fieldStart, false, false);
                        fieldStart = i + 1;
                    }
                    break;
                case ParseState.InQuotedField:
                    if (c == _options.EscapeChar && i + 1 < line.Length && line[i + 1] == _options.QuoteChar)
                    {
                        needsUnescape = true; i++;
                    }
                    else if (c == _options.QuoteChar)
                    {
                        state = ParseState.AfterQuote;
                    }

                    break;
                case ParseState.AfterQuote:
                    if (c == _options.Delimiter)
                    {
                        ranges[rangeCount++] = new CsvFieldRange(fieldStart, i - fieldStart, true, needsUnescape);
                        fieldStart = i + 1;
                        state = ParseState.Normal;
                        needsUnescape = false;
                    }
                    break;
            }
        }
        if (rangeCount < ranges.Length)
        {
            ranges[rangeCount++] = new CsvFieldRange(fieldStart, line.Length - fieldStart, state != ParseState.Normal, needsUnescape);
        }

        return rangeCount;
    }

    private bool IsComment(ReadOnlySpan<char> span)
    {
        if (!_options.CommentCharacter.HasValue)
        {
            return false;
        }

        ReadOnlySpan<char> trimmed = span.TrimStart();

        return !trimmed.IsEmpty && trimmed[0] == _options.CommentCharacter.Value;
    }
}
using System;
using TinyCsvParser.Models;

public ref struct CsvRow
{
    private readonly ReadOnlySpan<char> _line;
    private readonly ReadOnlySpan<CsvFieldRange> _ranges;
    private readonly CsvOptions _options;

    public int RecordIndex { get; }
    public int LineNumber { get; }

    public CsvRow(ReadOnlySpan<char> line, ReadOnlySpan<CsvFieldRange> ranges, CsvOptions options, int recordIndex, int lineNumber)
    {
        _line = line;
        _ranges = ranges;
        _options = options;
        RecordIndex = recordIndex;
        LineNumber = lineNumber;
    }

    public int Count => _ranges.Length;

    public string GetString(int index)
    {
        if (index >= _ranges.Length)
        {
            return string.Empty;
        }

        CsvFieldRange range = _ranges[index];
        ReadOnlySpan<char> raw = _line.Slice(range.Start, range.Length);

        if (!range.IsQuoted)
        {
            return raw.ToString();
        }

        if (!range.NeedsUnescape)
        {
            // Entferne Quotes (Start und Ende)
            return raw.Length < 2 ? string.Empty : raw[1..^1].ToString();
        }

        return UnescapeAndCreateString(raw);
    }

    public ReadOnlySpan<char> GetSpan(int index)
    {
        if (index >= _ranges.Length)
        {
            return ReadOnlySpan<char>.Empty;
        }

        CsvFieldRange range = _ranges[index];
        ReadOnlySpan<char> raw = _line.Slice(range.Start, range.Length);


        if (range.IsQuoted && raw.Length >= 2)
        {
            return raw[1..^1];
        }
        return raw;
    }

    private string UnescapeAndCreateString(ReadOnlySpan<char> rawQuoted)
    {
        // Content within the quotes, excluding the surrounding quotes
        ReadOnlySpan<char> content = rawQuoted[1..^1];

        // Use Stackalloc to reduce GC Pressure
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
                    // Double-Quote Case ("")
                    if (srcIdx + 1 < content.Length && content[srcIdx + 1] == quote)
                    {
                        buffer[destIdx++] = quote;
                        srcIdx += 2;
                        continue;
                    }
                }
                else
                {
                    // Standard Escape Case (\")
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

        return new string(buffer[..destIdx]);
    }
}
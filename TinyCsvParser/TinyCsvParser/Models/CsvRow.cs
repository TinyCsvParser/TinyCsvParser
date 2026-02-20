// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.CompilerServices;

namespace TinyCsvParser.Models;

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



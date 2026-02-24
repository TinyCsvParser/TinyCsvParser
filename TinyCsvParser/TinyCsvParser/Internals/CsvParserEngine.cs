// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using TinyCsvParser.Models;

namespace TinyCsvParser.Internals;

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
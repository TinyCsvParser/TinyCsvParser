// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Buffers;
using System.IO;
using System.Text;
using TinyCsvParser.Models;

namespace TinyCsvParser.Internals;

public class SpanBasedCsvReader : IDisposable
{
    private readonly StreamReader _reader;
    private char[]? _buffer;
    private int _charsFilled;
    private int _currentIdx;
    private bool _isEndOfStream;
    private int _currentRecordIndex = 0;
    private int _physicalLineNumber = 1;
    private readonly CsvOptions _options;

    private const int BufferSize = 32 * 1024;

    public SpanBasedCsvReader(Stream stream, CsvOptions options)
    {
        _reader = new StreamReader(stream, options.Encoding ?? Encoding.UTF8, true, BufferSize, leaveOpen: true);
        _buffer = ArrayPool<char>.Shared.Rent(BufferSize);
        _options = options;
    }

    public bool TryGetNextRecord(out ReadOnlySpan<char> recordSpan, out int recordIndex, out int lineNumber, out bool isComment)
    {
        recordSpan = default;
        recordIndex = -1;
        lineNumber = -1;
        isComment = false;

        if (_buffer == null)
        {
            return false;
        }

        while (true)
        {
            if (TryFindEndOfRecord(out int lengthOfData, out int lengthOfDelimiter, out int newlinesConsumed))
            {
                recordSpan = new ReadOnlySpan<char>(_buffer, _currentIdx, lengthOfData);

                // Is this a comment line?
                isComment = IsCommentLine(recordSpan);

                // A comment line is not a record and should not increase the record index, but
                // it should increase the line number.
                recordIndex = isComment ? -1 : _currentRecordIndex++;

                // Increase the actual Line Number nevertheless
                lineNumber = _physicalLineNumber;

                // Increase counters for next record
                _physicalLineNumber += newlinesConsumed;
                _currentIdx += lengthOfData + lengthOfDelimiter;

                return true;
            }

            if (_isEndOfStream)
            {
                if (_currentIdx < _charsFilled)
                {
                    recordSpan = new ReadOnlySpan<char>(_buffer, _currentIdx, _charsFilled - _currentIdx);
                    isComment = IsCommentLine(recordSpan);
                    recordIndex = isComment ? -1 : _currentRecordIndex++;
                    lineNumber = _physicalLineNumber;
                    _currentIdx = _charsFilled;
                    return true;
                }

                return false;
            }

            RefillBuffer();
        }
    }

    private bool IsCommentLine(ReadOnlySpan<char> span)
    {
        if (!_options.CommentCharacter.HasValue)
        {
            return false;
        }

        for (int i = 0; i < span.Length; i++)
        {
            char c = span[i];
            if (c == _options.CommentCharacter.Value)
            {
                return true;
            }

            if (!char.IsWhiteSpace(c))
            {
                return false;
            }
        }

        return false;
    }


    private bool TryFindEndOfRecord(out int dataLength, out int delimiterLength, out int newlinesConsumed)
    {
        dataLength = 0;
        delimiterLength = 0;
        newlinesConsumed = 0;

        if (_buffer == null)
        {
            return false;
        }

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

            if (c == '\n')
            {
                newlinesConsumed++;
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

    public void ResetRecordIndex()
    {
        _currentRecordIndex = 0;
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
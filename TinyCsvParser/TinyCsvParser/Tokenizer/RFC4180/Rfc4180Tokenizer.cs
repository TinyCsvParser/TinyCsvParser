// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using TinyCsvParser.Extensions;

namespace TinyCsvParser.Tokenizer.RFC4180
{
    public class RFC4180Tokenizer : ITokenizer 
    {
        private char _lastChar;

        public RFC4180Tokenizer(Options options)
        {
            Options = options;
        }

        public Options Options { get; }

        public TokenEnumerable Tokenize(ReadOnlySpan<char> input)
        {
            return new TokenEnumerable(input, NextToken);
        }

        private ReadOnlySpan<char> NextToken(ReadOnlySpan<char> chars, out ReadOnlySpan<char> remaining, out bool foundToken)
        {
            chars = chars.TrimStart();
            var options = Options;

            if (chars.IsEmpty)
            {
                foundToken = _lastChar == options.DelimiterCharacter;
                _lastChar = (char)0;
                return remaining = ReadOnlySpan<char>.Empty;
            }

            char c = chars[0];
            _lastChar = c;

            if (c == options.DelimiterCharacter)
            {
                remaining = chars.Slice(1);
                foundToken = true;
                return ReadOnlySpan<char>.Empty;
            }
            else
            {
                var result = ReadOnlySpan<char>.Empty;
                if (IsQuoteCharacter(c))
                {
                    result = ReadQuoted(chars, out chars);

                    chars = chars.TrimStart();

                    if (chars.Length <= 1)
                    {
                        remaining = ReadOnlySpan<char>.Empty;
                        foundToken = true;
                        return result;
                    }

                    if (IsDelimiter(chars[0]))
                    {
                        _lastChar = chars[0];
                        chars = chars.Slice(1);
                    }

                    remaining = chars;
                    foundToken = true;
                    return result;
                }

                result = chars.ReadTo(options.DelimiterCharacter, out chars, trim: true);
                chars = chars.TrimStart();

                if (chars.IsEmpty)
                {
                    remaining = chars;
                    foundToken = true;
                    return result;
                }

                if (IsDelimiter(chars[0]))
                {
                    _lastChar = chars[0];
                    chars = chars.Slice(1);
                }

                remaining = chars;
                foundToken = true;
                return result;
            }
        }

        private ReadOnlySpan<char> ReadQuoted(ReadOnlySpan<char> chars, out ReadOnlySpan<char> remaining)
        {
            var options = Options;
            if (chars[0] == options.QuoteCharacter)
                chars = chars.Slice(1);

            var result = chars.ReadTo(options.QuoteCharacter, out chars);

            if (chars[0] == options.QuoteCharacter)
                chars = chars.Slice(1);

            if (chars.IsEmpty || chars[0] != options.QuoteCharacter)
            {
                remaining = chars;
                return result;
            }

            // this could be more efficient by figuring out indices and slicing the original input accordingly
            var buffer = new List<char>(result.Length + 10);
            buffer.AddRange(result);
            do
            {
                buffer.Add(chars[0]);
                chars = chars.Slice(1);
                var read = chars.ReadTo(options.QuoteCharacter, out chars);
                buffer.AddRange(read);
                chars = chars.Slice(1);
            } while (!chars.IsEmpty && chars[0] == options.QuoteCharacter);

            remaining = chars;
            return buffer.ToArray().AsSpan();
        }

        private bool IsQuoteCharacter(char c) => c == Options.QuoteCharacter;

        private bool IsDelimiter(char c) => c == Options.DelimiterCharacter;

        public override string ToString() => $"RFC4180Tokenizer (Options = {Options})";
    }
}

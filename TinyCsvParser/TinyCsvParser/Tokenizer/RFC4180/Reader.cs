// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Text;

namespace TinyCsvParser.Tokenizer.RFC4180
{
    public class Reader
    {
        public enum TokenType
        {
            Token,
            EndOfRecord
        }

        public class Token
        {
            public readonly TokenType Type;

            public readonly ReadOnlyMemory<char> Content;

            public Token(TokenType type)
                : this(type, ReadOnlyMemory<char>.Empty)
            {
            }

            public Token(TokenType type, ReadOnlyMemory<char> content)
            {
                Type = type;
                Content = content;
            }
        }

        private readonly Options options;

        public Reader(Options options)
        {
            this.options = options;
        }

        public IList<Token> ReadTokens(ReadOnlySpan<char> chars)
        {
            var tokens = new List<Token>();
            while (true)
            {
                Token token = NextToken(chars, out chars);

                tokens.Add(token);

                if (token.Type == TokenType.EndOfRecord)
                {
                    break;
                }
            }
            return tokens;
        }

        private Token NextToken(ReadOnlySpan<char> chars, out ReadOnlySpan<char> remaining)
        {
            chars = chars.TrimStart();

            var result = ReadOnlyMemory<char>.Empty;

            if (chars.IsEmpty)
            {
                remaining = chars;
                return new Token(TokenType.EndOfRecord);
            }

            char c = chars[0];

            if (c == options.DelimiterCharacter)
            {
                remaining = chars.Slice(1);
                return new Token(TokenType.Token);
            }
            else
            {
                if (IsQuoteCharacter(c))
                {
                    result = ReadQuoted(chars, out chars);

                    chars = chars.TrimStart();

                    if (chars.Length <= 1)
                    {
                        remaining = ReadOnlySpan<char>.Empty;
                        return new Token(TokenType.EndOfRecord, result);
                    }

                    if (IsDelimiter(chars[0]))
                    {
                        chars = chars.Slice(1);
                    }

                    remaining = chars;
                    return new Token(TokenType.Token, result);
                }

                result = chars.ReadTo(options.DelimiterCharacter, out chars, trim: true);
                chars = chars.TrimStart();

                if (chars.IsEmpty)
                {
                    remaining = chars;
                    return new Token(TokenType.EndOfRecord, result);
                }

                if (IsDelimiter(chars[0]))
                {
                    chars = chars.Slice(1);
                }

                remaining = chars;
                return new Token(TokenType.Token, result);
            }
        }

        private ReadOnlyMemory<char> ReadQuoted(ReadOnlySpan<char> chars, out ReadOnlySpan<char> remaining)
        {
            chars = chars.Slice(1);

            var result = chars.ReadTo(options.QuoteCharacter, out chars);

            if (chars[0] == options.QuoteCharacter)
                chars = chars.Slice(1);

            if (chars.IsEmpty || chars[0] != options.QuoteCharacter)
            {
                remaining = chars;
                return result;
            }

            var buffer = new StringBuilder(result.ToString());
            do
            {
                buffer.Append(chars[0]);
                chars = chars.Slice(1);
                buffer.Append(chars.ReadTo(options.QuoteCharacter, out chars).Span);
                chars = chars.Slice(1);
            } while (!chars.IsEmpty && chars[0] == options.QuoteCharacter);

            remaining = chars;
            return buffer.ToString().AsMemory();
        }

        private bool IsQuoteCharacter(char c) => c == options.QuoteCharacter;

        private bool IsDelimiter(char c) => c == options.DelimiterCharacter;

        public override string ToString() => $"Reader (Options = {options})";
    }
}
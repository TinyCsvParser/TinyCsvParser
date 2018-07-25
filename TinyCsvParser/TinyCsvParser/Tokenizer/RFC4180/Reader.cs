// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Buffers;
using System.Collections.Generic;
using TinyCsvParser.Extensions;
using IToken = System.Buffers.IMemoryOwner<char>;
using ITokens = System.Buffers.IMemoryOwner<System.Buffers.IMemoryOwner<char>>;

namespace TinyCsvParser.Tokenizer.RFC4180
{
    public sealed class Reader
    {
        public enum TokenType
        {
            Token,
            EndOfRecord
        }

        private readonly Options options;

        public Reader(Options options)
        {
            this.options = options;
        }

        public ITokens ReadTokens(ReadOnlySpan<char> chars)
        {
            var tokens = new List<IToken>();
            while (true)
            {
                var (tokenType, token) = NextToken(chars, out chars);

                tokens.Add(token);

                if (tokenType == TokenType.EndOfRecord)
                {
                    break;
                }
            }

            var output = SizedMemoryPool<IToken>.Instance.Rent(tokens.Count);
            tokens.CopyTo(output.Memory.Span);
            return output;
        }

        private (TokenType, IToken) NextToken(ReadOnlySpan<char> chars, out ReadOnlySpan<char> remaining)
        {
            chars = chars.TrimStart();

            IToken result = null;

            if (chars.IsEmpty)
            {
                remaining = chars;
                return (TokenType.EndOfRecord, result);
            }

            char c = chars[0];

            if (c == options.DelimiterCharacter)
            {
                remaining = chars.Slice(1);
                return (TokenType.Token, result);
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
                        return (TokenType.EndOfRecord, result);
                    }

                    if (IsDelimiter(chars[0]))
                    {
                        chars = chars.Slice(1);
                    }

                    remaining = chars;
                    return (TokenType.Token, result);
                }

                result = chars.ReadTo(options.DelimiterCharacter, out chars, trim: true);
                chars = chars.TrimStart();

                if (chars.IsEmpty)
                {
                    remaining = chars;
                    return (TokenType.EndOfRecord, result);
                }

                if (IsDelimiter(chars[0]))
                {
                    chars = chars.Slice(1);
                }

                remaining = chars;
                return (TokenType.Token, result);
            }
        }

        private IToken ReadQuoted(ReadOnlySpan<char> chars, out ReadOnlySpan<char> remaining)
        {
            chars = chars.Slice(1);

            var result = chars.ReadTo(options.QuoteCharacter, out chars);
            var resultSpan = result.Memory.Span;

            if (chars[0] == options.QuoteCharacter)
                chars = chars.Slice(1);

            if (chars.IsEmpty || chars[0] != options.QuoteCharacter)
            {
                remaining = chars;
                return result;
            }

            try
            {
                var buffer = new List<char>(resultSpan.Length + 10);
                buffer.AddRange(resultSpan);
                do
                {
                    buffer.Add(chars[0]);
                    chars = chars.Slice(1);
                    using (var read = chars.ReadTo(options.QuoteCharacter, out chars))
                        buffer.AddRange(read.Memory.Span);
                    chars = chars.Slice(1);
                } while (!chars.IsEmpty && chars[0] == options.QuoteCharacter);

                remaining = chars;
                var token = SizedMemoryPool<char>.Instance.Rent(buffer.Count);
                buffer.CopyTo(token.Memory.Span);
                return token;
            }
            finally
            {
                result?.Dispose();
            }
        }

        private bool IsQuoteCharacter(char c) => c == options.QuoteCharacter;

        private bool IsDelimiter(char c) => c == options.DelimiterCharacter;

        public override string ToString() => $"Reader (Options = {options})";
    }
}
// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Buffers;
using System.Collections.Generic;
using TinyCsvParser.Extensions;
using IToken = System.Buffers.IMemoryOwner<char>;
using ITokens = System.Buffers.IMemoryOwner<System.Buffers.IMemoryOwner<char>>;

namespace TinyCsvParser.Tokenizer
{
    public class StringSplitTokenizer : ITokenizer, ITokenizer2
    {
        public readonly char[] FieldsSeparator;
        public readonly bool TrimLine;

        public StringSplitTokenizer(char[] fieldsSeparator, bool trimLine)
        {
            FieldsSeparator = fieldsSeparator;
            TrimLine = trimLine;
        }

        public ITokens Tokenize(ReadOnlySpan<char> input)
        {
            var pool = SizedMemoryPool<char>.Instance;
            var tokens = new List<IToken>();
            var parts = TrimLine ? input.Trim().Split(FieldsSeparator) : input.Split(FieldsSeparator);

            foreach (var part in parts)
            {
                var token = pool.Rent(part.Length);
                part.CopyTo(token.Memory.Span);
                tokens.Add(token);
            }

            var output = SizedMemoryPool<IToken>.Instance.Rent(tokens.Count);
            tokens.CopyTo(output.Memory.Span);

            return output;
        }

        TokenEnumerable ITokenizer2.Tokenize(ReadOnlySpan<char> input)
        {
            ReadOnlySpan<char> nextToken(ReadOnlySpan<char> chars, out ReadOnlySpan<char> remaining)
            {
                int idx = chars.IndexOf(FieldsSeparator, StringComparison.Ordinal);
                if (idx == -1)
                {
                    remaining = ReadOnlySpan<char>.Empty;
                    return chars;
                }

                remaining = chars.Slice(idx + 1);
                return chars.Slice(0, idx);
            }

            return new TokenEnumerable(TrimLine ? input.Trim() : input, nextToken);
        }

        public override string ToString()
        {
            return string.Format("StringSplitTokenizer (FieldsSeparator = {0}, TrimLine = {1})", FieldsSeparator, TrimLine);
        }
    }
}
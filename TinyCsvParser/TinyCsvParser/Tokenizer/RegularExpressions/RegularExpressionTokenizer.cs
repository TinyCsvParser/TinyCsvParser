// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TinyCsvParser.Extensions;
using IToken = System.Buffers.IMemoryOwner<char>;
using ITokens = System.Buffers.IMemoryOwner<System.Buffers.IMemoryOwner<char>>;

namespace TinyCsvParser.Tokenizer.RegularExpressions
{
    public abstract class RegularExpressionTokenizer : ITokenizer
    {
        public abstract Regex Regexp { get; }

        public ITokens Tokenize(ReadOnlySpan<char> input)
        {
            var pool = SizedMemoryPool<char>.Instance;
            var matches = Regexp.Matches(input.ToString());
            var tokens = new List<IToken>(matches.Count);
            foreach (var (index, length) in matches.Cast<Match>())
            {
                var token = pool.Rent(length);
                input.Slice(index, length).CopyTo(token.Memory.Span);
                tokens.Add(token);
            }

            var output = SizedMemoryPool<IToken>.Instance.Rent(tokens.Count);
            tokens.CopyTo(output.Memory.Span);
            return output;
        }

        public override string ToString() => $"Regexp = {Regexp}";
    }
}
// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Text.RegularExpressions;

namespace TinyCsvParser.Tokenizer.RegularExpressions
{
    public abstract class RegularExpressionTokenizer : ITokenizer
    {
        public abstract Regex Regexp { get; }

        public TokenEnumerable Tokenize(ReadOnlySpan<char> input)
        {
            // Sadly no support for ReadOnlySpan<char> in Regex yet, so we have to allocate a string here:
            var matches = Regexp.Matches(input.ToString());
            int i = 0;
            
            ReadOnlySpan<char> nextToken(ReadOnlySpan<char> chars, out ReadOnlySpan<char> remaining)
            {
                if (i >= matches.Count)
                {
                    return remaining = ReadOnlySpan<char>.Empty;
                }

                var match = matches[i++];
                remaining = chars;
                return chars.Slice(match.Index, match.Length);
            }

            return new TokenEnumerable(input, nextToken);
        }

        public override string ToString() => $"Regexp = {Regexp}";
    }
}
// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Text.RegularExpressions;

namespace TinyCsvParser.Tokenizer.RegularExpressions
{
    public class RegularExpressionTokenizer : ITokenizer
    {
        public RegularExpressionTokenizer(Regex regex)
        {
            Regexp = regex;
        }

        public Regex Regexp { get; }

        public TokenEnumerable Tokenize(ReadOnlySpan<char> input)
        {
            // Sadly no support for ReadOnlySpan<char> in Regex yet, so we have to allocate a string here:
            var orig = input.ToString();
            var matches = Regexp.Matches(orig);
            int i = 0;
            int lastStart = 0;
            
            ReadOnlySpan<char> nextToken(ReadOnlySpan<char> chars, out ReadOnlySpan<char> remaining, out bool foundToken)
            {
                if (i == matches.Count)
                {
                    foundToken = true;
                    i++;
                    remaining = ReadOnlySpan<char>.Empty;
                    return orig.AsSpan(lastStart);
                }
                else if (i > matches.Count)
                {
                    foundToken = false;
                    return remaining = ReadOnlySpan<char>.Empty;
                }

                var match = matches[i++];
                remaining = orig.AsSpan(match.Index + match.Length);
                foundToken = true;
                var result = orig.AsSpan(lastStart, match.Index - lastStart);
                lastStart = match.Index + match.Length;
                return result;
            }

            return new TokenEnumerable(input, nextToken);
        }

        public override string ToString() => $"Regexp = {Regexp}";
    }
}
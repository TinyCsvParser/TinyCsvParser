// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace TinyCsvParser.Tokenizer.RegularExpressions
{
    public abstract class RegularExpressionTokenizer : ITokenizer
    {
        public abstract Regex Regexp { get; }

        public string[] Tokenize(string input)
        {
            return Regexp.Matches(input)
                .Cast<Match>()
                .Select(x => x.Value)
                .ToArray();
        }

        public string[] Tokenize(ReadOnlySpan<char> input) => Tokenize(input.ToString());

        public override string ToString()
        {
            return string.Format("Regexp = {0}", Regexp);
        }
    }
}
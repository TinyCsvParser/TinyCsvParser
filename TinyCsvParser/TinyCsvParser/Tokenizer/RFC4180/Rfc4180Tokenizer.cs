// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace TinyCsvParser.Tokenizer.RFC4180
{
    public class RFC4180Tokenizer : ITokenizer 
    {
        public RFC4180Tokenizer(Options options)
        {
            Options = options;
        }

        public Options Options { get; }

        public TokenEnumerable Tokenize(ReadOnlySpan<char> input)
        {
            throw new NotImplementedException();
        }

        public override string ToString() => $"RFC4180Tokenizer (Options = {Options})";
    }
}

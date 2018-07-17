// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;

namespace TinyCsvParser.Tokenizer.RFC4180
{
    public class RFC4180Tokenizer : ITokenizer 
    {
        private readonly Reader _reader;

        public RFC4180Tokenizer(Options options)
        {
            _reader = new Reader(options);
        }

        public ReadOnlyMemory<char>[] Tokenize(ReadOnlySpan<char> input)
        {
            var tokens = _reader.ReadTokens(input);
            var len = tokens.Count;
            var output = new ReadOnlyMemory<char>[len];
            for (int i = 0; i < len; i++)
            {
                output[i] = tokens[i].Content;
            }
            return output;
        }

        public override string ToString()
        {
            return string.Format("RFC4180Tokenizer (Reader = {0})", _reader);
        }
    }
}

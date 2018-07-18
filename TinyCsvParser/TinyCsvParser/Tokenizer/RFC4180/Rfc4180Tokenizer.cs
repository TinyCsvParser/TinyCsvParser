// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using ITokens = System.Buffers.IMemoryOwner<System.Buffers.IMemoryOwner<char>>;

namespace TinyCsvParser.Tokenizer.RFC4180
{
    public class RFC4180Tokenizer : ITokenizer 
    {
        private readonly Reader _reader;

        public RFC4180Tokenizer(Options options)
        {
            _reader = new Reader(options);
        }

        public ITokens Tokenize(ReadOnlySpan<char> input)
        {
            return _reader.ReadTokens(input);
        }

        public override string ToString()
        {
            return string.Format("RFC4180Tokenizer (Reader = {0})", _reader);
        }
    }
}

// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace TinyCsvParser.Tokenizer
{
    public interface ITokenizer
    {
        // TODO: implementations could use array pooling, if there's a reliable place to return the arrays when finished.
        ReadOnlyMemory<char>[] Tokenize(ReadOnlySpan<char> input);
    }
}

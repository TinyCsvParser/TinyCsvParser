// Copyright (c) Philipp Wagner and Joel Mueller. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace TinyCsvParser.Tokenizer
{
    public interface ITokenizer
    {
        TokenEnumerable Tokenize(ReadOnlySpan<char> input);
    }
}

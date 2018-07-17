// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace TinyCsvParser.Model
{
    public class TokenizedRow
    {
        public readonly int Index;

        public readonly ReadOnlyMemory<char>[] Tokens;

        public TokenizedRow(int index, ReadOnlyMemory<char>[] tokens)
        {
            Index = index;
            Tokens = tokens;
        }
    }
}

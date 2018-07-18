// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using ITokens = System.Buffers.IMemoryOwner<System.Buffers.IMemoryOwner<char>>;

namespace TinyCsvParser.Model
{
    public class TokenizedRow : IDisposable
    {
        public readonly int Index;

        public readonly ITokens Tokens;

        public TokenizedRow(int index, ITokens tokens)
        {
            Index = index;
            Tokens = tokens;
        }

        public void Dispose()
        {
            Tokens?.Dispose();
        }
    }
}

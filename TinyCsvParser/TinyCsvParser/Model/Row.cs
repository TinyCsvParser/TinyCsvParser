// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using IToken = System.Buffers.IMemoryOwner<char>;

namespace TinyCsvParser.Model
{
    public class Row : IDisposable
    {
        private readonly ReadOnlyMemory<char> _data = ReadOnlyMemory<char>.Empty;
        private readonly IToken _token;

        public readonly int Index;

        public ReadOnlyMemory<char> Data => _token?.Memory ?? _data;

        public Row(int index, ReadOnlyMemory<char> data)
        {
            Index = index;
            _data = data;
        }

        public Row(int index, IToken token)
        {
            Index = index;
            _token = token;
        }

        public void Dispose()
        {
            _token?.Dispose();
        }
    }
}

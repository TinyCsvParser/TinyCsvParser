// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace TinyCsvParser.Model
{
    public class Row
    {
        public readonly int Index;

        public readonly ReadOnlyMemory<char> Data;

        public Row(int index, ReadOnlyMemory<char> data)
        {
            Index = index;
            Data = data;
        }
    }
}

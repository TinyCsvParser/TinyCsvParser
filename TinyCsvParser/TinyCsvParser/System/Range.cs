// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace TinyCsvParser.System
{
    public struct Range
    {
        public readonly int Start;

        public readonly int End;

        public readonly int Length;

        public Range(int start, int end)
        {
            Start = start;
            End = end;
            Length = end - start + 1;
        }
    }
}

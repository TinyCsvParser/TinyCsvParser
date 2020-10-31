// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace TinyCsvParser.Ranges
{
    public class RangeDefinition
    {
        /// <summary>Represent the inclusive start index of the Range.</summary>
        public int Start { get; }

        /// <summary>Represent the exclusive end index of the Range.</summary>
        public int End { get; }

        public int Length { get; }

        /// <summary>Construct a Range object using the start and end indexes.</summary>
        /// <param name="start">Represent the inclusive start index of the range.</param>
        /// <param name="end">Represent the exclusive end index of the range.</param>
        public RangeDefinition(int start, int end)
        {
            Start = start;
            End = end;
            Length = end - start + 1;
        }

        public Tuple<int, int> GetOffsetAndLength(int length)
        {
            return Tuple.Create(Start, End - Start);
        }
    }
}

// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using System.Collections.Generic;
using System;

namespace TinyCsvParser.Tokenizer
{
    /// <summary>
    /// Implements a Tokenizer, that makes it possible to Tokenize a CSV line using fixed length columns.
    /// </summary>
    public class FixedLengthTokenizer : ITokenizer
    {
        /// <summary>
        /// A column in a CSV file, which is described by the start and end position (zero-based indices).
        /// </summary>
        public class ColumnDefinition
        {
            public readonly int Start;

            public readonly int End;

            public ColumnDefinition(int start, int end)
            {
                Start = start;
                End = end;
            }

            public override string ToString()
            {
                return string.Format("ColumnDefinition (Start = {0}, End = {1}", Start, End);
            }
        }

        public readonly ColumnDefinition[] Columns;

        public FixedLengthTokenizer(ColumnDefinition[] columns)
        {
            Columns = columns ?? throw new ArgumentNullException("columns");
        }

        public FixedLengthTokenizer(IList<ColumnDefinition> columns)
        {
            Columns = columns?.ToArray() ?? throw new ArgumentNullException("columns");
        }

        public ReadOnlyMemory<char>[] Tokenize(ReadOnlySpan<char> input)
        {
            var tokenizedLine = new ReadOnlyMemory<char>[Columns.Length];

            for (int columnIndex = 0; columnIndex < Columns.Length; columnIndex++)
            {
                var columnDefinition = Columns[columnIndex];
                var columnData = input.Slice(columnDefinition.Start, columnDefinition.End - columnDefinition.Start);

                tokenizedLine[columnIndex] = columnData.ToArray().AsMemory();
            }

            return tokenizedLine;
        }

        public override string ToString()
        {
            var columnDefinitionsString = string.Join(", ", Columns.Select(x => x.ToString()));

            return string.Format("FixedLengthTokenizer (Columns = [{0}])", columnDefinitionsString);
        }
    }
}
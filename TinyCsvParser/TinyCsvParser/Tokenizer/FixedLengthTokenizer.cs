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
                return $"ColumnDefinition (Start = {Start}, End = {End}";
            }
        }

        public readonly ColumnDefinition[] Columns;

        public readonly bool Trim;

        public FixedLengthTokenizer(ColumnDefinition[] columns, bool trim = false)
        {
            if (columns == null)
            {
                throw new ArgumentNullException(nameof(columns));
            }

            Columns = columns;
            Trim = trim;
        }

        public FixedLengthTokenizer(IList<ColumnDefinition> columns, bool trim = false)
        {
            if (columns == null)
            {
                throw new ArgumentNullException(nameof(columns));
            }

            Columns = columns.ToArray();
            Trim = trim;
        }

        public string[] Tokenize(string input)
        {
            string[] tokenizedLine = new string[Columns.Length];

            for (int columnIndex = 0; columnIndex < Columns.Length; columnIndex++)
            {
                var columnDefinition = Columns[columnIndex];
                var columnData = input.Substring(columnDefinition.Start, columnDefinition.End - columnDefinition.Start);

                tokenizedLine[columnIndex] = Trim ? columnData.Trim() : columnData;
            }

            return tokenizedLine;
        }

        public override string ToString()
        {
            var columnDefinitionsString = string.Join(", ", Columns.Select(x => x.ToString()));

            return $"FixedLengthTokenizer (Columns = [{columnDefinitionsString}], Trim = {Trim})";
        }
    }
}
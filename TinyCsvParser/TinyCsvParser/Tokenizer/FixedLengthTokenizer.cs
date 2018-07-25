// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;

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
        public class Column
        {
            public readonly int Start;

            public readonly int End;

            public Column(int start, int end)
            {
                Start = start;
                End = end;
            }

            public override string ToString()
            {
                return string.Format("ColumnDefinition (Start = {0}, End = {1}", Start, End);
            }
        }

        private readonly Column[] _columns;

        public FixedLengthTokenizer(IList<Column> columns, bool trimToken = false) : this(columns?.ToArray(), trimToken) { }

        public FixedLengthTokenizer(Column[] columns, bool trimToken = false)
        {
            _columns = columns ?? throw new ArgumentNullException("columns");
            TrimToken = trimToken;
        }

        public bool TrimToken { get; }
        public ReadOnlyMemory<Column> Columns => _columns.AsMemory();

        public TokenEnumerable Tokenize(ReadOnlySpan<char> input)
        {
            int colIndex = 0;
            int colCount = Columns.Length;

            ReadOnlySpan<char> nextToken(ReadOnlySpan<char> chars, out ReadOnlySpan<char> remaining)
            {
                if (colIndex >= colCount)
                {
                    remaining = ReadOnlySpan<char>.Empty;
                    return chars;
                }

                var col = _columns[colIndex];

                if (chars.Length < col.End - col.Start)
                {
                    return chars = remaining = ReadOnlySpan<char>.Empty;
                }

                remaining = chars.Slice(col.End);
                return TrimToken ? chars.Slice(col.Start, col.End - col.Start).Trim() : chars.Slice(col.Start, col.End - col.Start);
            }

            return new TokenEnumerable(input, nextToken);
        }

        public override string ToString()
        {
            var columnDefinitionsString = string.Join(", ", _columns.Select(x => x.ToString()));

            return $"FixedLengthTokenizer (Columns = [{columnDefinitionsString}])";
        }
    }
}
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using System.Collections.Generic;
using System;
using TinyCsvParser.Model;

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

        public IEnumerable<TokenizedRow> Tokenize(IEnumerable<string> input)
        {
            int line_no = 1;
            
            foreach(var line in input)
            {
                bool isError = false;

                string[] tokenizedLine = new string[Columns.Length];
                
                for (int columnIndex = 0; columnIndex < Columns.Length; columnIndex++)
                {
                    var columnDefinition = Columns[columnIndex];

                    if(columnDefinition.Start > (line.Length - 1))
                    {
                        yield return new TokenizedRow
                        {
                            Rows = new TokenizedRow.RowData[]
                            {
                                new TokenizedRow.RowData
                                {
                                    LineNo = line_no,
                                    Data = line
                                }
                            },
                            Error = new TokenizedRow.TokenizeError
                            {
                                LineNo = line_no,
                                Reason = $"Column Start Position '{columnDefinition.Start}' is Out of Bounds. Line Length is '{line.Length}'"
                            }
                        };

                        isError = true;

                        break;
                    }

                    if (columnDefinition.Start > (line.Length - 1))
                    {
                        yield return new TokenizedRow
                        {
                            Rows = new TokenizedRow.RowData[]
                            {
                                new TokenizedRow.RowData
                                {
                                    LineNo = line_no,
                                    Data = line
                                }
                            },
                            Error = new TokenizedRow.TokenizeError
                            {
                                LineNo = line_no,
                                Reason = $"Column End Position '{columnDefinition.End}' is Out of Bounds. Line Length is '{line.Length}'"
                            }
                        };

                        isError = true;

                        break;
                    }

                    var columnData = line.Substring(columnDefinition.Start, columnDefinition.End - columnDefinition.Start);

                    tokenizedLine[columnIndex] = Trim ? columnData.Trim() : columnData;
                }

                if(!isError)
                {
                    yield return new TokenizedRow
                    {
                        Rows = new []
                        {
                            new TokenizedRow.RowData
                            {
                                LineNo = line_no,
                                Data = line
                            }
                        },
                        Tokens = tokenizedLine
                    };
                }
                
            }
        }

        public override string ToString()
        {
            var columnDefinitionsString = string.Join(", ", Columns.Select(x => x.ToString()));

            return $"FixedLengthTokenizer (Columns = [{columnDefinitionsString}], Trim = {Trim})";
        }
    }
}
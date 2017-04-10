// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace TinyCsvParser.Tokenizer
{
  /// <summary>
  ///   Implements a Tokenizer, that makes it possible to Tokenize a CSV line using fixed length columns.
  /// </summary>
  public class FixedLengthTokenizer : ITokenizer
  {
    public readonly ColumnDefinition[] Columns;

    public FixedLengthTokenizer(ColumnDefinition[] columns)
    {
      Columns = columns ?? throw new ArgumentNullException(nameof(columns));
    }

    public FixedLengthTokenizer(IList<ColumnDefinition> columns)
    {
      Columns = columns?.ToArray() ?? throw new ArgumentNullException(nameof(columns));
    }

    public string[] Tokenize(string input)
    {
      var tokenizedLine = new string[Columns.Length];

      for (var columnIndex = 0; columnIndex < Columns.Length; columnIndex++)
      {
        var columnDefinition = Columns[columnIndex];
        var columnData = input.Substring(columnDefinition.Start, columnDefinition.End - columnDefinition.Start);

        tokenizedLine[columnIndex] = columnData;
      }

      return tokenizedLine;
    }

    public KeyValuePair<int, string[]> Tokenize(KeyValuePair<int, string> input)
    {
      return new KeyValuePair<int, string[]>(input.Key, Tokenize(input.Value));
    }

    public override string ToString()
    {
      var columnDefinitionsString = string.Join(", ", Columns.Select(x => x.ToString()));

      return string.Format("FixedLengthTokenizer (Columns = [{0}])", columnDefinitionsString);
    }

    /// <summary>
    ///   A column in a CSV file, which is described by the start and end position (zero-based indices).
    /// </summary>
    public class ColumnDefinition
    {
      public readonly int End;
      public readonly int Start;

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
  }
}
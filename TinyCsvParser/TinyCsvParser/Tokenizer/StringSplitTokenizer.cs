// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace TinyCsvParser.Tokenizer
{
  public class StringSplitTokenizer : ITokenizer
  {
    public readonly char[] FieldsSeparator;
    public readonly bool TrimLine;

    public StringSplitTokenizer(char[] fieldsSeparator, bool trimLine)
    {
      FieldsSeparator = fieldsSeparator;
      TrimLine = trimLine;
    }

    public string[] Tokenize(string input)
    {
      if (input == null) return null;
      return TrimLine ? input.Trim().Split(FieldsSeparator) : input.Split(FieldsSeparator);
    }

    public KeyValuePair<int, string[]> Tokenize(KeyValuePair<int, string> input)
    {
      return new KeyValuePair<int, string[]>(input.Key, Tokenize(input.Value));
    }

    public override string ToString()
    {
      return string.Format("StringSplitTokenizer (FieldsSeparator = {0}, TrimLine = {1})", FieldsSeparator, TrimLine);
    }
  }
}
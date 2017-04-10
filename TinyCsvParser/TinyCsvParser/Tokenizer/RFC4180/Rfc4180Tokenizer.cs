// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TinyCsvParser.Tokenizer.RFC4180
{
  public class RFC4180Tokenizer : ITokenizer
  {
    private readonly Reader _reader;

    public RFC4180Tokenizer(Options options)
    {
      _reader = new Reader(options);
    }

    public string[] Tokenize(string input)
    {
      using (var stringReader = new StringReader(input))
      {
        return _reader.ReadTokens(stringReader)
          .Select(token => token.Content)
          .ToArray();
      }
    }

    public KeyValuePair<int, string[]> Tokenize(KeyValuePair<int, string> input)
    {
      return new KeyValuePair<int, string[]>(input.Key, Tokenize(input.Value));
    }

    public override string ToString() => $"RFC4180Tokenizer (Reader = {_reader})";
  }
}
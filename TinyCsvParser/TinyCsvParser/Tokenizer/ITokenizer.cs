// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace TinyCsvParser.Tokenizer
{
  public interface ITokenizer
  {
    string[] Tokenize(string input);
    KeyValuePair<int, string[]> Tokenize(KeyValuePair<int, string> input);
  }
}
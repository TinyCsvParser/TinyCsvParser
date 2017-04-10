// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace TinyCsvParser.Tokenizer.RFC4180
{
    public class RFC4180Tokenizer : ITokenizer
    {
        private readonly Reader reader;

        public RFC4180Tokenizer(Options options)
        {
            this.reader = new Reader(options);
        }

        public string[] Tokenize(string input)
        {
            using (var stringReader = new StringReader(input))
            {
                return reader.ReadTokens(stringReader)
                    .Select(token => token.Content)
                    .ToArray();
            }
        }

      /// <inheritdoc />
      public KeyValuePair<int, string[]> Tokenize(KeyValuePair<int, string> input)
      {
        return new KeyValuePair<int, string[]>(input.Key, Tokenize(input.Value));
      }

      public override string ToString()
        {
            return string.Format("RFC4180Tokenizer (Reader = {0})", reader);
        }
    }
}

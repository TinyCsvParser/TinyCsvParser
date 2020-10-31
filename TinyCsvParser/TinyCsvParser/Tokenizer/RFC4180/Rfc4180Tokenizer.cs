// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using System.IO;

namespace TinyCsvParser.Tokenizer.RFC4180
{
    public class RFC4180Tokenizer : ITokenizer 
    {
        private Reader reader;

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

        public override string ToString()
        {
            return $"RFC4180Tokenizer (Reader = {reader})";
        }
    }
}

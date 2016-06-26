using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace TinyCsvParser.Tokenizer.Rfc4180
{
    public class Rfc4180Tokenizer : ITokenizer 
    {
        private Reader reader;

        public Rfc4180Tokenizer(Options options)
        {
            this.reader = new Reader(options);
        }

        public string[] Tokenize(string input)
        {
            using (var stringReader = new StringReader(input))
            {
                return reader
                    .ReadTokens(stringReader)
                    .Select(x => x.Content.ToString())
                    .ToArray();
            }
        }
    }
}

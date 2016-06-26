using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyCsvParser.Tokenizer.Rfc4180
{
    public class Options
    {
        public readonly char QuoteCharacter;
        public readonly char EscapeCharacter;
        public readonly char DelimiterCharacter;

        public Options(char quoteCharacter, char escapeCharacter, char delimiterCharacter)
        {
            QuoteCharacter = quoteCharacter;
            EscapeCharacter = escapeCharacter;
            DelimiterCharacter = delimiterCharacter;
        }
    }
}

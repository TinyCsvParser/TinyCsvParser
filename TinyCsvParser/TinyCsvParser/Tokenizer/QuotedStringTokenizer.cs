// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using TinyCsvParser.Tokenizer.RFC4180;

namespace TinyCsvParser.Tokenizer
{
    public class QuotedStringTokenizer : RFC4180Tokenizer
    {
        public QuotedStringTokenizer(char columnDelimiter)
            : this('"', '\\', columnDelimiter)
        {
        }

        public QuotedStringTokenizer(char quoteCharacter, char escapeCharacter, char columnDelimiter)
            : base(new Options(quoteCharacter, escapeCharacter, columnDelimiter))
        {
        }        

        public override string ToString()
        {
            return $"QuotedStringTokenizer({base.ToString()})";
        }
    }
}
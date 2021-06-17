// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace TinyCsvParser.Tokenizer.RFC4180
{
    public class Options
    {
        public enum QuoteOptions
        {
            QUOTE_MINIMAL = 0, 
            QUOTE_ALL = 1,
            QUOTE_NONNUMERIC = 2, 
            QUOTE_NONE = 3
        }

        public readonly char QuoteCharacter;
        public readonly char EscapeCharacter;
        public readonly char DelimiterCharacter;
        public readonly bool SkipInitialWhitespaces;

        public Options(char quoteCharacter, char escapeCharacter, char delimiterCharacter, bool skipInitialWhitespaces)
        {
            QuoteCharacter = quoteCharacter;
            EscapeCharacter = escapeCharacter;
            DelimiterCharacter = delimiterCharacter;
            SkipInitialWhitespaces = skipInitialWhitespaces;
        }

        public override string ToString()
        {
            return $"Options (QuoteCharacter = {QuoteCharacter}, EscapeCharacter = {EscapeCharacter}, DelimiterCharacter = {DelimiterCharacter})";
        }
    }
}

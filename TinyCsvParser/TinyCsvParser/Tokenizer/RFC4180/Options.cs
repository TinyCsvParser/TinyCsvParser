// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace TinyCsvParser.Tokenizer.RFC4180
{
    public class Options
    {
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

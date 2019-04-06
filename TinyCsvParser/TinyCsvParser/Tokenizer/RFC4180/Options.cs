// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace TinyCsvParser.Tokenizer.RFC4180
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

        public override string ToString()
        {
            return $"Options (QuoteCharacter = {QuoteCharacter}, EscapeCharacter = {EscapeCharacter}, DelimiterCharacter = {DelimiterCharacter})";
        }
    }
}

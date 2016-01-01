// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace TinyCsvParser.Tokenizer
{
    public class StringSplitTokenizer : ITokenizer
    {
        public readonly char[] FieldsSeparator;
        public readonly bool TrimLine;

        public StringSplitTokenizer(char[] fieldsSeparator, bool trimLine)
        {
            FieldsSeparator = fieldsSeparator;
            TrimLine = trimLine;
        }

        public string[] Tokenize(string input)
        {
            if(TrimLine) 
            {
                return input.Trim().Split(FieldsSeparator);
            }
            return input.Split(FieldsSeparator);
        }

        public override string ToString()
        {
            return string.Format("StringSplitTokenizer (FieldsSeparator = {0}, TrimLine = {1})", FieldsSeparator, TrimLine);
        }
    }
}
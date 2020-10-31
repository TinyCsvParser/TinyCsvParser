// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;

namespace TinyCsvParser.Tokenizer
{
    public class StringSplitTokenizer : ITokenizer
    {
        public readonly char[] FieldsSeparator;
        public readonly bool TrimValues;

        public StringSplitTokenizer(char[] fieldsSeparator, bool trimValues)
        {
            FieldsSeparator = fieldsSeparator;
            TrimValues = trimValues;
        }

        public string[] Tokenize(string input)
        {
            if(TrimValues) 
            {
                return input
                    .Split(FieldsSeparator)
                    .Select(x => x.Trim())
                    .ToArray();
            }

            return input.Split(FieldsSeparator);
        }

        public override string ToString()
        {
            return $"StringSplitTokenizer (FieldsSeparator = {FieldsSeparator}, TrimValues = {TrimValues})";
        }
    }
}
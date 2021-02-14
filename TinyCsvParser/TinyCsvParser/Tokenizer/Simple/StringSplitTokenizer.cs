// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
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

        public IEnumerable<string[]> Tokenize(IEnumerable<string> input)
        {
            return input.Select(x => TokenizeLine(x));
        }

        public string[] TokenizeLine(string input)
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
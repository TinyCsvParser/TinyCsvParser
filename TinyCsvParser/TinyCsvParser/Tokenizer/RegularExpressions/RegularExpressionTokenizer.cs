// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using System.Text.RegularExpressions;

namespace TinyCsvParser.Tokenizer.RegularExpressions
{
    public abstract class RegularExpressionTokenizer : ITokenizer
    {
        public abstract Regex Regexp { get; }
        
        public string[] Tokenize(string input)
        {
            return Regexp.Matches(input)
                .Cast<Match>()
                .Select(x => x.Value)
                .ToArray();
        }

        public override string ToString()
        {
            return $"Regexp = {Regexp}";
        }
    }
}
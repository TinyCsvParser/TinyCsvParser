// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace TinyCsvParser.Model
{
    public class TokenizedRow
    {
        public readonly int Index;

        public readonly string[] Tokens;

        public TokenizedRow(int index, string[] tokens)
        {
            Index = index;
            Tokens = tokens;
        }
    }
}

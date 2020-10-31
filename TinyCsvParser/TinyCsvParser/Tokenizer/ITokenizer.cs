// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace TinyCsvParser.Tokenizer
{
    public interface ITokenizer
    {
        string[] Tokenize(string input);
    }
}

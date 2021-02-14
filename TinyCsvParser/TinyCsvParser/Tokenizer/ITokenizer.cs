// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace TinyCsvParser.Tokenizer
{
    public interface ITokenizer
    {
        IEnumerable<string[]> Tokenize(IEnumerable<string> input);
    }
}

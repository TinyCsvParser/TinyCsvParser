// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using TinyCsvParser.Model;

namespace TinyCsvParser.Tokenizer
{
    public interface ITokenizer
    {
        IEnumerable<TokenizedRow> Tokenize(IEnumerable<string> input);
    }
}

// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using TinyCsvParser.Model;

namespace TinyCsvParser.Mapping
{
    public class CsvStringArrayMapping : ICsvMapping<string[]>
    {
        public CsvMappingResult<string[]> Map(TokenizedRow values, int ignoreColumns = 0)
        {
            return new CsvMappingResult<string[]>
            {
                RowIndex = values.Index,
                Result = values.Tokens
            };
        }

        public CsvHeaderMappingResult MapHeader(TokenizedRow values)
        {
            throw new System.NotImplementedException();
        }

        public Dictionary<int, string> GetPropertyMapping()
        {
            throw new System.NotImplementedException();
        }
    }
}

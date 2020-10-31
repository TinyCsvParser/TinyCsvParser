// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using TinyCsvParser.Model;

namespace TinyCsvParser.Mapping
{
    public class CsvStringArrayMapping : ICsvMapping<string[]>
    {
        public CsvMappingResult<string[]> Map(TokenizedRow values)
        {
            return new CsvMappingResult<string[]> {
                RowIndex =  values.Index,
                Result = values.Tokens
            };
        }
    }
}

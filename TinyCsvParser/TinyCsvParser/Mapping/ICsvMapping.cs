// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using TinyCsvParser.Model;

namespace TinyCsvParser.Mapping
{
    public interface ICsvMapping<TEntity>
    {
        CsvMappingResult<TEntity> Map(TokenizedRow values);
        CsvHeaderMappingResult MapHeader(TokenizedRow values);
        Dictionary<int, string> GetPropertyMapping();
    }
}

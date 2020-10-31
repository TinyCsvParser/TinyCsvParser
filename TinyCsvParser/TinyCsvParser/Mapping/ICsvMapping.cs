// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using TinyCsvParser.Model;

namespace TinyCsvParser.Mapping
{
    public interface ICsvMapping<TEntity>
    {
        CsvMappingResult<TEntity> Map(TokenizedRow values);
    }
}

// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using TinyCsvParser.Mapping;
using TinyCsvParser.Model;

namespace TinyCsvParser
{
    public interface ICsvParser<TEntity>
    {
        ParallelQuery<CsvMappingResult<TEntity>> Parse(IEnumerable<Row> csvData);
    }
}

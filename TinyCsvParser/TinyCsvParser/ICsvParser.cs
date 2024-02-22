// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using TinyCsvParser.Mapping;
using TinyCsvParser.Model;

namespace TinyCsvParser
{
    public interface ICsvParser<TEntity>
    {
        CsvData<TEntity> Parse(IEnumerable<Row> csvData);
        Dictionary<int, string> GetPropertyMapping();
    }

    public class CsvData<TEntity>
    {
        public CsvHeaderMappingResult Header { get; set; }
        public ParallelQuery<CsvMappingResult<TEntity>> Items { get; set; }
    }
}

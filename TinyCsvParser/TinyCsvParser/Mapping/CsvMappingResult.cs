// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace TinyCsvParser.Mapping
{
    public abstract class CsvMappingResultBase
    {
        public int RowIndex { get; set; }

        public CsvMappingError Error { get; set; }

        public bool IsValid { get { return Error == null; } }
    }

    public class CsvMappingResult<TEntity> : CsvMappingResultBase
    {
        public TEntity Result { get; set; }

        public override string ToString()
        {
            return $"CsvMappingResult (Error = {Error}, Result = {Result})";
        }
    }

    public class CsvHeaderMappingResult : CsvMappingResultBase
    {
        public List<string> Values { get; set; }
    }
}

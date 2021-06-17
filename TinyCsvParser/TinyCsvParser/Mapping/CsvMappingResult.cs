// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using TinyCsvParser.Model;

namespace TinyCsvParser.Mapping
{
    public class CsvMappingResult<TEntity>
    {
        public TokenizedRow Row { get; set; }

        public CsvMappingError Error { get; set; }

        public TEntity Result { get; set; }

        public bool IsValid { get { return Error == null; } }

        public override string ToString()
        {
            return $"CsvMappingResult (Error = {Error}, Result = {Result})";
        }
    }
}

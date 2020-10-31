// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace TinyCsvParser.Mapping
{
    public class CsvMappingError
    {
        public int ColumnIndex { get; set; }

        public string Value { get; set; }

        public string UnmappedRow { get; set; }

        public override string ToString()
        {
            return $"CsvMappingError (ColumnIndex = {ColumnIndex}, Value = {Value}, UnmappedRow = {UnmappedRow})";
        }
    }
}
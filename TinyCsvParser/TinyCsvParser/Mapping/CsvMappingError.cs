// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace TinyCsvParser.Mapping
{
    public class CsvMappingError
    {
        public int ColumnIndex { get; set; }

        public string Value { get; set; }

        public string UnmappedRow { get; set; }

        public Dictionary<int, string> ColumnValues { get; set; } = new Dictionary<int, string>();
        public CsvParserErrorCodes ErrorCode;

        public override string ToString()
        {
            return $"CsvMappingError (ColumnIndex = {ColumnIndex}, Value = {Value}, UnmappedRow = {UnmappedRow})";
        }
    }

    public enum CsvParserErrorCodes
    {
        ColumnsExceedNumberOfProperties,
        InvalidColumnData,
        OutOfRange
    }
}
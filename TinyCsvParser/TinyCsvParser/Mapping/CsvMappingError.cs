// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace TinyCsvParser.Mapping
{
    public class CsvMappingError
    {
        public int? RowNumber { get; set; }
        public int ColumnIndex { get; set; }

        public string Value { get; set; }
        public string ColumnMapDetails { get; set; }

      public override string ToString()
        {
            return string.Format("CsvMappingError (ColumnIndex = {0}, Value = {1})", ColumnIndex, Value);
        }
    }
}

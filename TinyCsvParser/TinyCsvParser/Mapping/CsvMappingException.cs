// Copyright (c) Philipp Wagner and Joel Mueller. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


using System;

namespace CoreCsvParser.Mapping
{
    public class CsvMappingException : Exception
    {
        private readonly string _message;

        public CsvMappingException(int rowIndex, int colIndex, string message)
        {
            RowIndex = rowIndex;
            ColumnIndex = colIndex;
            _message = message;
        }

        public int ColumnIndex { get; }
        public int RowIndex { get; }
        public override string Message => _message;
    }
}

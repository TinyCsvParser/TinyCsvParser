// Copyright (c) Philipp Wagner and Joel Mueller. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace CoreCsvParser.Mapping
{
    public readonly struct CsvMappingResult<TEntity> where TEntity : new()
    {
        private readonly TEntity _result;

        public CsvMappingResult(int rowIndex, TEntity result)
        {
            RowIndex = rowIndex;
            _result = result;
            Error = default;
            IsValid = true;
        }

        public CsvMappingResult(int rowIndex, int colIndex, string errorMessage)
        {
            RowIndex = rowIndex;
            _result = default;
            Error = new CsvMappingException(rowIndex, colIndex, errorMessage);
            IsValid = false;
        }

        public readonly int RowIndex;

        public readonly CsvMappingException Error;

        public TEntity Result => 
            IsValid ? _result : throw new InvalidOperationException($"{Error.Message} (Row: {RowIndex}, Column: {Error.ColumnIndex})");

        public readonly bool IsValid;

        public override string ToString()
        {
            if (!IsValid)
                return $"CsvMappingResult (RowIndex = {RowIndex}, Error = {Error})";
            return $"CsvMappingResult (RowIndex = {RowIndex}, Result = {Result})";
        }
    }
}

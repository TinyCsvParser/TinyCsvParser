// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace TinyCsvParser.Mapping
{
    public class CsvMappingResult<TEntity>
        where TEntity : class, new()
    {
        private TEntity _result = null;

        public int RowIndex { get; set; }

        public CsvMappingError Error { get; set; }

        public TEntity Result
        {
            set => _result = value;
            get
            {
                if (_result is null && !(Error is null))
                {
                    throw new InvalidOperationException(Error.ToString());
                }
                return _result;
            }
        }

        public bool IsValid => Error == null;

        public override string ToString()
        {
            if (!(Error is null))
                return $"CsvMappingResult (Error = {Error})";
            return $"CsvMappingResult (Result = {Result})";
        }
    }
}

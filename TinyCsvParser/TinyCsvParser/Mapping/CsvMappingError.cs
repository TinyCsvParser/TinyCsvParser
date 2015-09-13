// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace TinyCsvParser.Mapping
{
    public class CsvMappingError
    {
        public Exception Exception { get; set; }

        public override string ToString()
        {
            return string.Format("CsvMappingError (Exception = {0})", Exception);
        }
    }
}

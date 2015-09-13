// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace TinyCsvParser.Exceptions
{
    public class CsvTypeConversionException : Exception
    {
        public CsvTypeConversionException()
            : base()
        {
        }

        public CsvTypeConversionException(string message)
            : base(message)
        {
        }

        public CsvTypeConversionException(string message, Exception inner)
            : base(message, inner)
        {
        }

        public override string ToString()
        {
            return string.Format("CsvValidationException ({0})", base.ToString());
        }
    }
}

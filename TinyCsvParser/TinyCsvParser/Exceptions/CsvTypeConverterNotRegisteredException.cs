// Copyright (c) Philipp Wagner and Joel Mueller. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace CoreCsvParser.Exceptions
{
    public class CsvTypeConverterNotRegisteredException : Exception
    {
        public CsvTypeConverterNotRegisteredException()
            : base()
        {
        }

        public CsvTypeConverterNotRegisteredException(string message)
            : base(message)
        {
        }

        public CsvTypeConverterNotRegisteredException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}

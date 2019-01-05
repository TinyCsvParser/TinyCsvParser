// Copyright (c) Philipp Wagner and Joel Mueller. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace CoreCsvParser.Exceptions
{
    public class CsvTypeConverterAlreadyRegisteredException : Exception
    {
        public CsvTypeConverterAlreadyRegisteredException()
            : base()
        {
        }

        public CsvTypeConverterAlreadyRegisteredException(string message)
            : base(message)
        {
        }

        public CsvTypeConverterAlreadyRegisteredException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}

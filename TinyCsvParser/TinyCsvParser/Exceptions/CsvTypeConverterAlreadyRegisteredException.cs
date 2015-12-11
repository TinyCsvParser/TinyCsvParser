// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.Serialization;

namespace TinyCsvParser.Exceptions
{
    [Serializable]
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

        protected CsvTypeConverterAlreadyRegisteredException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}

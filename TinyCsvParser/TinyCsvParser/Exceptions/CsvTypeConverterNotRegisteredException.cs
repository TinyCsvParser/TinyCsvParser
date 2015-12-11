// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.Serialization;

namespace TinyCsvParser.Exceptions
{
    [Serializable]
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

        protected CsvTypeConverterNotRegisteredException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}

// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace TinyCsvParser.Exceptions
{
    public class CsvTypeConverterNotRegisteredException : Exception
    {
        private readonly Type targetType;

        public CsvTypeConverterNotRegisteredException(Type targetType)
            : base()
        {
            this.targetType = targetType;
        }

        public CsvTypeConverterNotRegisteredException(Type targetType, string message)
            : base(message)
        {
            this.targetType = targetType;
        }

        public CsvTypeConverterNotRegisteredException(Type targetType, string message, Exception inner)
            : base(message, inner)
        {
            this.targetType = targetType;
        }

        public override string ToString()
        {
            return string.Format("CsvTypeConverterNotRegisteredException (TargetType = {0})", targetType);
        }
    }
}

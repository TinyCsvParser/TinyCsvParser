// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using TinyCsvParser.Exceptions;

namespace TinyCsvParser.TypeConverter
{
    public abstract class NullableNumericConverter<TNumericType> : NullableConverter<TNumericType?>
        where TNumericType : struct
    {
        private readonly NonNullableConverter<TNumericType> internalConverter;

        public NullableNumericConverter(NonNullableConverter<TNumericType> internalConverter)
        {
            this.internalConverter = internalConverter;
        }

        protected override bool InternalConvert(string value, out TNumericType? result)
        {
            result = default(TNumericType?);

            TNumericType innerConverterResult;

            if (internalConverter.TryConvert(value, out innerConverterResult))
            {
                result = innerConverterResult;

                return true;
            }

            return false;
        }
    }
}

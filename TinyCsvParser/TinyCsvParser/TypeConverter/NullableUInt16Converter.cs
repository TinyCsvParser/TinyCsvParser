// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;

namespace TinyCsvParser.TypeConverter
{
    public class NullableUInt16Converter : NullableConverter<UInt16?>
    {
        private readonly UInt16Converter uint16Converter;

        public NullableUInt16Converter()
            : this(CultureInfo.InvariantCulture)
        {
        }

        public NullableUInt16Converter(IFormatProvider formatProvider)
        {
            uint16Converter = new UInt16Converter(formatProvider);
        }

        protected override bool InternalConvert(string value, out UInt16? result)
        {
            result = default(UInt16?);

            UInt16 innerConverterResult;
            if (uint16Converter.TryConvert(value, out innerConverterResult))
            {
                result = innerConverterResult;

                return true;
            }
            return false;
        }
    }
}

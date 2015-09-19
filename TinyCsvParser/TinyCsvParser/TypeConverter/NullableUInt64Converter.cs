// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;

namespace TinyCsvParser.TypeConverter
{
    public class NullableUInt64Converter : NullableConverter<UInt64?>
    {
        private readonly UInt64Converter uint64Converter;

        public NullableUInt64Converter()
            : this(CultureInfo.InvariantCulture)
        {
        }

        public NullableUInt64Converter(IFormatProvider formatProvider)
        {
            uint64Converter = new UInt64Converter(formatProvider);
        }

        protected override bool InternalConvert(string value, out UInt64? result)
        {
            result = default(UInt64?);

            UInt64 innerConverterResult;
            if (uint64Converter.TryConvert(value, out innerConverterResult))
            {
                result = innerConverterResult;

                return true;
            }
            return false;
        }
    }
}

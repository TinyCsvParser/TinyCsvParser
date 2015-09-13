// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;

namespace TinyCsvParser.TypeConverter
{
    public class NullableUInt32Converter : NullableConverter<UInt32?>
    {
        private readonly UInt32Converter uint32Converter;

        public NullableUInt32Converter()
            : this(CultureInfo.InvariantCulture)
        {
        }

        public NullableUInt32Converter(IFormatProvider formatProvider)
        {
            uint32Converter = new UInt32Converter(formatProvider);
        }

        protected override UInt32? InternalConvert(string value)
        {
            return uint32Converter.Convert(value);
        }
    }
}

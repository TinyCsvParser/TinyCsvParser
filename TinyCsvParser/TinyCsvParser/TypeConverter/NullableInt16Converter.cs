// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;

namespace TinyCsvParser.TypeConverter
{
    public class NullableInt16Converter : NullableConverter<Int16?>
    {
        private readonly Int16Converter int16Converter;
        private readonly NumberStyles numberStyles;

        public NullableInt16Converter()
            : this(CultureInfo.InvariantCulture)
        {
        }

        public NullableInt16Converter(IFormatProvider formatProvider)
            : this(formatProvider, NumberStyles.None)
        {
        }

        public NullableInt16Converter(IFormatProvider formatProvider, NumberStyles numberStyles)
        {
            int16Converter = new Int16Converter(formatProvider, numberStyles);
        }

        protected override bool InternalConvert(string value, out Int16? result)
        {
            result = default(Int16?);
            
            Int16 innerConverterResult;
            if (int16Converter.TryConvert(value, out innerConverterResult))
            {
                result = innerConverterResult;

                return true;
            }

            return false;
        }
    }
}
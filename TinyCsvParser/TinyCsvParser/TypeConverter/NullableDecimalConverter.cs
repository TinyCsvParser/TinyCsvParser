// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;

namespace TinyCsvParser.TypeConverter
{
    public class NullableDecimalConverter : NullableConverter<Decimal?>
    {
        private readonly DecimalConverter decimalConverter;
        private readonly NumberStyles numberStyles;

        public NullableDecimalConverter()
            : this(CultureInfo.InvariantCulture)
        {
        }

        public NullableDecimalConverter(IFormatProvider formatProvider)
            : this(formatProvider, NumberStyles.None)
        {
        }

        public NullableDecimalConverter(IFormatProvider formatProvider, NumberStyles numberStyles)
        {
            decimalConverter = new DecimalConverter(formatProvider, numberStyles);
        }

        protected override bool InternalConvert(string value, out Decimal? result)
        {
            result = default(Decimal?);
            
            Decimal innerConverterResult;
            if (decimalConverter.TryConvert(value, out innerConverterResult))
            {
                result = innerConverterResult;

                return true;
            }

            return false;
        }
    }
}
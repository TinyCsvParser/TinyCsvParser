// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;

namespace TinyCsvParser.TypeConverter
{
    public class NullableDoubleConverter : NullableConverter<Double?>
    {
        private readonly DoubleConverter doubleConverter;
        private readonly NumberStyles numberStyles;

        public NullableDoubleConverter()
            : this(CultureInfo.InvariantCulture)
        {
        }

        public NullableDoubleConverter(IFormatProvider formatProvider)
            : this(formatProvider, NumberStyles.None)
        {
        }

        public NullableDoubleConverter(IFormatProvider formatProvider, NumberStyles numberStyles)
        {
            doubleConverter = new DoubleConverter(formatProvider, numberStyles);
        }

        protected override bool InternalConvert(string value, out Double? result)
        {
            result = default(Double?);
            
            Double innerConverterResult;
            if (doubleConverter.TryConvert(value, out innerConverterResult))
            {
                result = innerConverterResult;

                return true;
            }

            return false;
        }
    }
}
// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;

namespace TinyCsvParser.TypeConverter
{
    public class NullableInt64Converter : NullableConverter<Int64?>
    {
        private readonly Int64Converter int64Converter;
        private readonly NumberStyles numberStyles;

        public NullableInt64Converter()
            : this(CultureInfo.InvariantCulture)
        {
        }

        public NullableInt64Converter(IFormatProvider formatProvider)
            : this(formatProvider, NumberStyles.None)
        {
        }

        public NullableInt64Converter(IFormatProvider formatProvider, NumberStyles numberStyles)
        {
            int64Converter = new Int64Converter(formatProvider, numberStyles);
        }

        protected override Int64? InternalConvert(string value)
        {
            return int64Converter.Convert(value);
        }
    }
}

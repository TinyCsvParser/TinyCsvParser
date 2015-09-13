// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;

namespace TinyCsvParser.TypeConverter
{
    public class Int64Converter : NonNullableConverter<Int64>
    {
        private readonly IFormatProvider formatProvider;
        private readonly NumberStyles numberStyles;

        public Int64Converter()
            : this(CultureInfo.InvariantCulture)
        {
        }

        public Int64Converter(IFormatProvider formatProvider)
            : this(formatProvider, NumberStyles.None)
        {
        }

        public Int64Converter(IFormatProvider formatProvider, NumberStyles numberStyles)
        {
            this.formatProvider = formatProvider;
            this.numberStyles = numberStyles;
        }

        protected override Int64 InternalConvert(string value)
        {
            return Int64.Parse(value, numberStyles, formatProvider);
        }
    }
}

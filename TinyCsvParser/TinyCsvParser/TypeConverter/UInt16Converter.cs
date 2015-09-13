// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;

namespace TinyCsvParser.TypeConverter
{
    public class UInt16Converter : NonNullableConverter<UInt16>
    {
        private readonly IFormatProvider formatProvider;
        private readonly NumberStyles numberStyles;

        public UInt16Converter()
            : this(CultureInfo.InvariantCulture)
        {
        }

        public UInt16Converter(IFormatProvider formatProvider)
            : this(formatProvider, NumberStyles.None)
        {
        }

        public UInt16Converter(IFormatProvider formatProvider, NumberStyles numberStyles)
        {
            this.formatProvider = formatProvider;
            this.numberStyles = numberStyles;
        }

        protected override UInt16 InternalConvert(string value)
        {
            return UInt16.Parse(value, numberStyles, formatProvider);
        }
    }
}

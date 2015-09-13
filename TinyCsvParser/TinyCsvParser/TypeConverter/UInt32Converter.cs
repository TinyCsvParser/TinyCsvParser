// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;

namespace TinyCsvParser.TypeConverter
{
    public class UInt32Converter : NonNullableConverter<UInt32>
    {
        private readonly IFormatProvider formatProvider;
        private readonly NumberStyles numberStyles;

        public UInt32Converter()
            : this(CultureInfo.InvariantCulture)
        {
        }

        public UInt32Converter(IFormatProvider formatProvider)
            : this(formatProvider, NumberStyles.None)
        {
        }

        public UInt32Converter(IFormatProvider formatProvider, NumberStyles numberStyles)
        {
            this.formatProvider = formatProvider;
            this.numberStyles = numberStyles;
        }

        protected override UInt32 InternalConvert(string value)
        {
            return UInt32.Parse(value, numberStyles, formatProvider);
        }
    }
}

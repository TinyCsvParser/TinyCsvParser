// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;

namespace TinyCsvParser.TypeConverter
{
    public class ByteConverter : NonNullableConverter<Byte>
    {
        private readonly IFormatProvider formatProvider;
        private readonly NumberStyles numberStyles;

        public ByteConverter()
            : this(CultureInfo.InvariantCulture)
        {
        }

        public ByteConverter(IFormatProvider formatProvider)
            : this(formatProvider, NumberStyles.None)
        {
        }

        public ByteConverter(IFormatProvider formatProvider, NumberStyles numberStyles)
        {
            this.formatProvider = formatProvider;
            this.numberStyles = numberStyles;
        }


        protected override Byte InternalConvert(string value)
        {
            return Byte.Parse(value, numberStyles, formatProvider);
        }
    }
}

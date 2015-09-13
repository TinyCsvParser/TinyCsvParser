// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;

namespace TinyCsvParser.TypeConverter
{
    public class NullableByteConverter : NullableConverter<byte?>
    {
        private readonly ByteConverter byteConverter;
        private readonly NumberStyles numberStyles;

        public NullableByteConverter()
            : this(CultureInfo.InvariantCulture)
        {
        }

        public NullableByteConverter(IFormatProvider formatProvider)
            : this(formatProvider, NumberStyles.None)
        {
        }

        public NullableByteConverter(IFormatProvider formatProvider, NumberStyles numberStyles)
        {
            byteConverter = new ByteConverter(formatProvider, numberStyles);
        }

        protected override byte? InternalConvert(string value)
        {
            return byteConverter.Convert(value);
        }
    }
}
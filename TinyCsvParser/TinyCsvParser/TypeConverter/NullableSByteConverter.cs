// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;

namespace TinyCsvParser.TypeConverter
{
    public class NullableSByteConverter : NullableConverter<SByte?>
    {
        private readonly SByteConverter sbyteConverter;
        private readonly NumberStyles numberStyles;

        public NullableSByteConverter()
            : this(CultureInfo.InvariantCulture)
        {
        }

        public NullableSByteConverter(IFormatProvider formatProvider)
            : this(formatProvider, NumberStyles.None)
        {
        }

        public NullableSByteConverter(IFormatProvider formatProvider, NumberStyles numberStyles)
        {
            sbyteConverter = new SByteConverter(formatProvider, numberStyles);
        }

        protected override SByte? InternalConvert(string value)
        {
            return sbyteConverter.Convert(value);
        }
    }
}
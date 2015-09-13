// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;

namespace TinyCsvParser.TypeConverter
{
    public class NullableInt32Converter : NullableConverter<Int32?>
    {
        private readonly Int32Converter int32Converter;
        private readonly NumberStyles numberStyles;

        public NullableInt32Converter()
            : this(CultureInfo.InvariantCulture)
        {
        }

        public NullableInt32Converter(IFormatProvider formatProvider)
            : this(formatProvider, NumberStyles.None)
        {
        }

        public NullableInt32Converter(IFormatProvider formatProvider, NumberStyles numberStyles)
        {
            int32Converter = new Int32Converter(formatProvider, numberStyles);
        }
        
        protected override Int32? InternalConvert(string value)
        {
            return int32Converter.Convert(value);
        }
    }
}

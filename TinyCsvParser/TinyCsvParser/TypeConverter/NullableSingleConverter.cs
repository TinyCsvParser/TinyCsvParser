// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;

namespace TinyCsvParser.TypeConverter
{
    public class NullableSingleConverter : NullableConverter<Single?>
    {
        private readonly SingleConverter singleConverter;
        private readonly NumberStyles numberStyles;

        public NullableSingleConverter()
            : this(CultureInfo.InvariantCulture)
        {
        }

        public NullableSingleConverter(IFormatProvider formatProvider)
            : this(formatProvider, NumberStyles.None)
        {
        }

        public NullableSingleConverter(IFormatProvider formatProvider, NumberStyles numberStyles)
        {
            singleConverter = new SingleConverter(formatProvider, numberStyles);
        }

        protected override bool InternalConvert(string value, out Single? result)
        {
            result = default(Single?);
            
            Single innerConverterResult;
            if (singleConverter.TryConvert(value, out innerConverterResult))
            {
                result = innerConverterResult;

                return true;
            }

            return false;
        }
    }
}
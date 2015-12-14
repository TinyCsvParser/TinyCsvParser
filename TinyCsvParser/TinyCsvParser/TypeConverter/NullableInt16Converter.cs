// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;

namespace TinyCsvParser.TypeConverter
{
    public class NullableInt16Converter : NullableInnerConverter<Int16>
    {
        public NullableInt16Converter()
            : base(new Int16Converter())
        {
        }

        public NullableInt16Converter(IFormatProvider formatProvider)
            : base(new Int16Converter(formatProvider))
        {
        }

        public NullableInt16Converter(IFormatProvider formatProvider, NumberStyles numberStyles)
            : base(new Int16Converter(formatProvider, numberStyles))
        {
        }
    }
}
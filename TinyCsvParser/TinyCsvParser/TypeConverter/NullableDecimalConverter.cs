// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;

namespace TinyCsvParser.TypeConverter
{
    public class NullableDecimalConverter : NullableInnerConverter<Decimal>
    {
        public NullableDecimalConverter()
            : base(new DecimalConverter())
        {
        }

        public NullableDecimalConverter(IFormatProvider formatProvider)
            : base(new DecimalConverter(formatProvider))
        {
        }

        public NullableDecimalConverter(IFormatProvider formatProvider, NumberStyles numberStyles)
            : base(new DecimalConverter(formatProvider, numberStyles))
        {
        }
    }
}
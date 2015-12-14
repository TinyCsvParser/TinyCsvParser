// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;

namespace TinyCsvParser.TypeConverter
{
    public class NullableTimeSpanConverter : NullableNumericConverter<TimeSpan>
    {
        public NullableTimeSpanConverter()
            : base(new TimeSpanConverter())
        {
        }

        public NullableTimeSpanConverter(string format)
            : base(new TimeSpanConverter(format, CultureInfo.InvariantCulture))
        {
        }

        public NullableTimeSpanConverter(string format, IFormatProvider formatProvider)
            : base(new TimeSpanConverter(format, formatProvider, TimeSpanStyles.None))
        {

        }
    }
}

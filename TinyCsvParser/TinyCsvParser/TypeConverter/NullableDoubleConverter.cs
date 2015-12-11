// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;

namespace TinyCsvParser.TypeConverter
{
    public class NullableDoubleConverter : NullableNumericConverter<Double>
    {
        public NullableDoubleConverter()
            : base(new DoubleConverter(CultureInfo.InvariantCulture, NumberStyles.None))
        {
        }
    }
}
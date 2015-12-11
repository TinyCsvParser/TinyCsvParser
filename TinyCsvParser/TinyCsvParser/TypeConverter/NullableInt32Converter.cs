// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;

namespace TinyCsvParser.TypeConverter
{
    public class NullableInt32Converter : NullableNumericConverter<Int32>
    {
        public NullableInt32Converter()
            : base(new Int32Converter(CultureInfo.InvariantCulture, NumberStyles.None))
        {
        }
    }
}

// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;

namespace TinyCsvParser.TypeConverter
{
    public class NullableUInt16Converter : NullableNumericConverter<UInt16>
    {
        public NullableUInt16Converter()
            : base(new UInt16Converter(CultureInfo.InvariantCulture, NumberStyles.None))
        {
        }
    }
}

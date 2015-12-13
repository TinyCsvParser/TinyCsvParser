// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;

namespace TinyCsvParser.TypeConverter
{
    public class NullableUInt32Converter : NullableNumericConverter<UInt32>
    {
        public NullableUInt32Converter()
            : base(new UInt32Converter())
        {
        }
    }
}

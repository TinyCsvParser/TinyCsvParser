// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;

namespace TinyCsvParser.TypeConverter
{
    public class NullableByteConverter : NullableNumericConverter<byte>
    {
        public NullableByteConverter()
            : base(new ByteConverter(CultureInfo.InvariantCulture, NumberStyles.None))
        {
        }
    }
}
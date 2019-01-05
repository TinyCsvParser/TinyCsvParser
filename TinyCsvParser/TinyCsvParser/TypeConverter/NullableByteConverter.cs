// Copyright (c) Philipp Wagner and Joel Mueller. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;

namespace CoreCsvParser.TypeConverter
{
    public class NullableByteConverter : NullableInnerConverter<byte>
    {
        public NullableByteConverter()
            : base(new ByteConverter())
        {
        }

        public NullableByteConverter(IFormatProvider formatProvider)
            : base(new ByteConverter(formatProvider))
        {
        }

        public NullableByteConverter(IFormatProvider formatProvider, NumberStyles numberStyles)
            : base(new ByteConverter(formatProvider, numberStyles))
        {
        }
    }
}
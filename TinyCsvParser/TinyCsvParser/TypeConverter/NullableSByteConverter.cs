// Copyright (c) Philipp Wagner and Joel Mueller. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;

namespace CoreCsvParser.TypeConverter
{
    public class NullableSByteConverter : NullableInnerConverter<SByte>
    {
        public NullableSByteConverter()
            : base(new SByteConverter())
        {
        }

        public NullableSByteConverter(IFormatProvider formatProvider)
            : base(new SByteConverter(formatProvider))
        {
        }

        public NullableSByteConverter(IFormatProvider formatProvider, NumberStyles numberStyles)
            : base(new SByteConverter(formatProvider, numberStyles))
        {
        }
    }
}
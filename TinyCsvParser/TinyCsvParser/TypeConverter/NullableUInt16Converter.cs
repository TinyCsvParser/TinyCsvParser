// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;

namespace TinyCsvParser.TypeConverter
{
    public class NullableUInt16Converter : NullableInnerConverter<UInt16>
    {
        public NullableUInt16Converter()
            : base(new UInt16Converter())
        {
        }

        public NullableUInt16Converter(IFormatProvider formatProvider)
            : base(new UInt16Converter(formatProvider))
        {
        }

        public NullableUInt16Converter(IFormatProvider formatProvider, NumberStyles numberStyles)
            : base(new UInt16Converter(formatProvider, numberStyles))
        {
        }
    }
}

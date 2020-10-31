// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;

namespace TinyCsvParser.TypeConverter
{
    public class NullableInt64Converter : NullableInnerConverter<Int64>
    {
        public NullableInt64Converter()
            : base(new Int64Converter())
        {
        }

        public NullableInt64Converter(IFormatProvider formatProvider)
            : base(new Int64Converter(formatProvider))
        {
        }

        public NullableInt64Converter(IFormatProvider formatProvider, NumberStyles numberStyles)
            : base(new Int64Converter(formatProvider, numberStyles))
        {
        }
    }
}

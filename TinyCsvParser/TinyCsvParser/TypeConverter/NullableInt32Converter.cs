// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;

namespace TinyCsvParser.TypeConverter
{
    public class NullableInt32Converter : NullableInnerConverter<Int32>
    {
        public NullableInt32Converter()
            : base(new Int32Converter())
        {
        }

        public NullableInt32Converter(IFormatProvider formatProvider)
            : base(new Int32Converter(formatProvider))
        {
        }

        public NullableInt32Converter(IFormatProvider formatProvider, NumberStyles numberStyles)
            : base(new Int32Converter(formatProvider, numberStyles))
        {
        }
    }
}

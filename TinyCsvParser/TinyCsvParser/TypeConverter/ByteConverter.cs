// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;

namespace TinyCsvParser.TypeConverter
{
    public class ByteConverter : NonNullableConverter<Byte>
    {
        private readonly IFormatProvider formatProvider;
        private readonly NumberStyles numberStyles;

        public ByteConverter()
            : this(CultureInfo.InvariantCulture)
        {
        }

        public ByteConverter(IFormatProvider formatProvider)
            : this(formatProvider, NumberStyles.Integer)
        {
        }

        public ByteConverter(IFormatProvider formatProvider, NumberStyles numberStyles)
        {
            this.formatProvider = formatProvider;
            this.numberStyles = numberStyles;
        }


        protected override bool InternalConvert(string value, out Byte result)
        {
            return Byte.TryParse(value, numberStyles, formatProvider, out result);
        }
    }
}

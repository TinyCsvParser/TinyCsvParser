// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;

namespace TinyCsvParser.TypeConverter
{
    public class Int32Converter : NonNullableConverter<Int32>
    {
        private readonly IFormatProvider formatProvider;
        private readonly NumberStyles numberStyles;

        public Int32Converter()
            : this(CultureInfo.InvariantCulture)
        {
        }

        public Int32Converter(IFormatProvider formatProvider)
            : this(formatProvider, NumberStyles.Integer)
        {
        }

        public Int32Converter(IFormatProvider formatProvider, NumberStyles numberStyles)
        {
            this.formatProvider = formatProvider;
            this.numberStyles = numberStyles;
        }

        protected override bool InternalConvert(string value, out Int32 result)
        {
            return Int32.TryParse(value, numberStyles, formatProvider, out result);
        }
    }
}

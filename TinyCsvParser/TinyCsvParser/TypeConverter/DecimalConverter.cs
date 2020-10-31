// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;

namespace TinyCsvParser.TypeConverter
{
    public class DecimalConverter : NonNullableConverter<Decimal>
    {
        private readonly IFormatProvider formatProvider;
        private readonly NumberStyles numberStyles;

        public DecimalConverter()
            : this(CultureInfo.InvariantCulture)
        {
        }

        public DecimalConverter(IFormatProvider formatProvider)
            : this(formatProvider, NumberStyles.Number)
        {
        }

        public DecimalConverter(IFormatProvider formatProvider, NumberStyles numberStyles)
        {
            this.formatProvider = formatProvider;
            this.numberStyles = numberStyles;
        }

        protected override bool InternalConvert(string value, out Decimal result)
        {
            return Decimal.TryParse(value, numberStyles, formatProvider, out result);
        }
    }
}

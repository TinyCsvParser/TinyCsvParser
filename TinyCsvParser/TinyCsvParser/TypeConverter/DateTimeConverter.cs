// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;

namespace TinyCsvParser.TypeConverter
{
    public class DateTimeConverter : NonNullableConverter<DateTime>
    {
        private readonly string dateTimeFormat;
        private readonly IFormatProvider formatProvider;
        private readonly DateTimeStyles dateTimeStyles;

        public DateTimeConverter()
            : this(string.Empty)
        {
        }

        public DateTimeConverter(string dateTimeFormat)
            : this(dateTimeFormat, CultureInfo.InvariantCulture)
        {
        }

        public DateTimeConverter(string dateTimeFormat, IFormatProvider formatProvider)
            : this(dateTimeFormat, formatProvider, DateTimeStyles.None)
        {
        }

        public DateTimeConverter(string dateTimeFormat, IFormatProvider formatProvider, DateTimeStyles dateTimeStyles)
        {
            this.dateTimeFormat = dateTimeFormat;
            this.formatProvider = formatProvider;
            this.dateTimeStyles = dateTimeStyles;
        }

        protected override bool InternalConvert(string value, out DateTime result)
        {
            if (string.IsNullOrWhiteSpace(dateTimeFormat))
            {
                return DateTime.TryParse(value, out result);
            }
            return DateTime.TryParseExact(value, this.dateTimeFormat, this.formatProvider, this.dateTimeStyles, out result);
        }
    }
}

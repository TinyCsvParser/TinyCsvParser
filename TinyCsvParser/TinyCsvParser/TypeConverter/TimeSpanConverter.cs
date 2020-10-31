// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;

namespace TinyCsvParser.TypeConverter
{
    public class TimeSpanConverter : NonNullableConverter<TimeSpan>
    {
        private readonly string format;
        private readonly IFormatProvider formatProvider;
        private readonly TimeSpanStyles timeSpanStyles;

        public TimeSpanConverter()
            : this(string.Empty)
        {
        }

        public TimeSpanConverter(string format)
            : this(format, CultureInfo.InvariantCulture)
        {
        }

        public TimeSpanConverter(string format, IFormatProvider formatProvider)
            : this(format, formatProvider, TimeSpanStyles.None)
        {

        }

        public TimeSpanConverter(string format, IFormatProvider formatProvider, TimeSpanStyles timeSpanStyles)
        {
            this.format = format;
            this.formatProvider = formatProvider;
            this.timeSpanStyles = timeSpanStyles;
        }

        protected override bool InternalConvert(string value, out TimeSpan result)
        {
            if (string.IsNullOrWhiteSpace(format))
            {
                return TimeSpan.TryParse(value, formatProvider, out result);
            }
            return TimeSpan.TryParseExact(value, format, formatProvider, timeSpanStyles, out result);
        }
    }
}

// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace TinyCsvParser.TypeConverter
{
    public class GuidConverter : NonNullableConverter<Guid>
    {
        private readonly string format;

        public GuidConverter()
            : this(string.Empty)
        {
        }

        public GuidConverter(string format)
        {
            this.format = format;
        }

        protected override bool InternalConvert(string value, out Guid result)
        {
            if (string.IsNullOrWhiteSpace(format))
            {
                return Guid.TryParse(value, out result);
            }
            return Guid.TryParseExact(value, format, out result);
        }
    }
}

// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace TinyCsvParser.TypeConverter
{
    public class GuidConverter : NonNullableConverter<Guid>
    {
        private readonly string format;

        public GuidConverter()
        {
            this.format = string.Empty;
        }

        public GuidConverter(string format)
        {
            this.format = format;
        }

        protected override Guid InternalConvert(string value)
        {
            if (string.IsNullOrWhiteSpace(format))
            {
                return Guid.Parse(value);
            }
            return Guid.ParseExact(value, format);
        }
    }
}

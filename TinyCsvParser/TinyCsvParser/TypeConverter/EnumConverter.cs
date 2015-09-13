// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace TinyCsvParser.TypeConverter
{
    public class EnumConverter<TTargetType> : NonNullableConverter<TTargetType>
    {
        private readonly Type enumType;
        private readonly bool ignoreCase;

        public EnumConverter()
            : this(true)
        {
        }

        public EnumConverter(bool ignoreCase)
        {
            if (!ReflectionUtils.IsEnum(typeof(TTargetType)))
            {
                throw new ArgumentException(string.Format("Type {0} is not a valid Enum", enumType));
            }
            this.enumType = typeof(TTargetType);
            this.ignoreCase = ignoreCase;
        }

        protected override TTargetType InternalConvert(string value)
        {
            return (TTargetType) Enum.Parse(enumType, value, ignoreCase);
        }
    }
}

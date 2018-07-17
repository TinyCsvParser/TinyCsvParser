// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using TinyCsvParser.Reflection;

namespace TinyCsvParser.TypeConverter
{
    public class BoolConverter : NonNullableConverter<bool>
    {
        private readonly string trueValue;
        private readonly string falseValue;
        private readonly StringComparison stringComparism;

        public BoolConverter()
            : this("true", "false", StringComparison.OrdinalIgnoreCase) { }

        public BoolConverter(string trueValue, string falseValue, StringComparison stringComparism)
        {
            this.trueValue = trueValue;
            this.falseValue = falseValue;
            this.stringComparism = stringComparism;
        }

        protected override bool InternalConvert(ReadOnlySpan<char> value, out bool result)
        {
            result = false;

            if (value.Equals(trueValue.AsSpan(), stringComparism)) 
            {
                result = true;
                return true;
            }

            if (value.Equals(falseValue.AsSpan(), stringComparism))
            {
                result = false;
                return true;
            }

            return false;
        }
    }
}

// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;

namespace TinyCsvParser.TypeConverter
{
    public class NullableBoolConverter : NullableInnerConverter<bool>
    {
        public NullableBoolConverter()
            : base(new BoolConverter())
        {
        }

        public NullableBoolConverter(string trueValue, string falseValue, StringComparison stringComparism)
            : base(new BoolConverter(trueValue, falseValue, stringComparism))
        {
        }
    }
}
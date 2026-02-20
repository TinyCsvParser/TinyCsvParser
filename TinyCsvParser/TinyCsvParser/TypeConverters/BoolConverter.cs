// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace TinyCsvParser.TypeConverters;

public class BoolConverter : NonNullableConverter<bool>
{
    private readonly string _trueValue;
    private readonly string _falseValue;
    private readonly StringComparison _stringComparison;

    public BoolConverter()
        : this("true", "false", StringComparison.OrdinalIgnoreCase)
    {
    }

    public BoolConverter(string trueValue, string falseValue, StringComparison stringComparison)
    {
        _trueValue = trueValue;
        _falseValue = falseValue;
        _stringComparison = stringComparison;
    }

    protected override bool InternalConvert(ReadOnlySpan<char> value, out bool result)
    {
        if (value.Equals(_trueValue.AsSpan(), _stringComparison))
        {
            result = true;
            return true;
        }

        if (value.Equals(_falseValue.AsSpan(), _stringComparison))
        {
            result = false;
            return true;
        }

        result = false;
        return false;
    }
}
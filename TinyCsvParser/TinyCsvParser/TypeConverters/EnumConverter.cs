// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace TinyCsvParser.TypeConverters;

public class EnumConverter<TEnum> : NonNullableConverter<TEnum> where TEnum : struct, Enum
{
    private readonly (string Name, TEnum Value)[] _enumMap;
    private readonly StringComparison _comparison;

    public EnumConverter() 
        : this(StringComparison.OrdinalIgnoreCase) { }

    public EnumConverter(StringComparison comparison)
    {
        _comparison = comparison;

        var names = Enum.GetNames(typeof(TEnum));
        var values = (TEnum[])Enum.GetValuesAsUnderlyingType(typeof(TEnum));

        _enumMap = new (string, TEnum)[names.Length];

        for (int i = 0; i < names.Length; i++)
        {
            _enumMap[i] = (names[i], values[i]);
        }
    }

    public EnumConverter(IDictionary<string, TEnum> customMap, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
    {
        _comparison = comparison;
        _enumMap = new (string, TEnum)[customMap.Count];
        int i = 0;
        foreach (var kvp in customMap)
        {
            _enumMap[i++] = (kvp.Key, kvp.Value);
        }
    }

    protected override bool InternalConvert(ReadOnlySpan<char> value, out TEnum result)
    {
        for (int i = 0; i < _enumMap.Length; i++)
        {
            if (value.Equals(_enumMap[i].Name.AsSpan(), _comparison))
            {
                result = _enumMap[i].Value;
                return true;
            }
        }

        if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int numericValue))
        {
            result = Unsafe.As<int, TEnum>(ref numericValue);
            return true;
        }

        result = default;
        return false;
    }
}



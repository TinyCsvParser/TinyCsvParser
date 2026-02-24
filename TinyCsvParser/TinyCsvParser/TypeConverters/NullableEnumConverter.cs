// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace TinyCsvParser.TypeConverters;

public class NullableEnumConverter<TEnum> : NullableConverter<TEnum?> where TEnum : struct, Enum
{
    private readonly EnumConverter<TEnum> _baseConverter;

    public NullableEnumConverter() : this(StringComparison.OrdinalIgnoreCase) { }

    public NullableEnumConverter(StringComparison comparison)
    {
        _baseConverter = new EnumConverter<TEnum>(comparison);
    }

    public NullableEnumConverter(IDictionary<string, TEnum> customMap, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
    {
        _baseConverter = new EnumConverter<TEnum>(customMap, comparison);
    }

    protected override bool InternalConvert(ReadOnlySpan<char> value, out TEnum? result)
    {
        if (_baseConverter.TryConvert(value, out TEnum tempResult))
        {
            result = tempResult;
            return true;
        }
        result = null;
        return false;
    }
}
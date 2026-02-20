// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace TinyCsvParser.TypeConverters;

public abstract class NonNullableConverter<TTargetType> : ITypeConverter<TTargetType>
{
    public Type TargetType => typeof(TTargetType);

    public bool TryConvert(ReadOnlySpan<char> value, out TTargetType result)
    {
        if (value.IsEmpty || value.IsWhiteSpace())
        {
            result = default!;
            return false;
        }
        return InternalConvert(value, out result);
    }

    protected abstract bool InternalConvert(ReadOnlySpan<char> value, out TTargetType result);
}
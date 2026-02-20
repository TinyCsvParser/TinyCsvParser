// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace TinyCsvParser.TypeConverters;

public class StringConverter : ITypeConverter<string>
{
    public Type TargetType => typeof(string);

    public bool TryConvert(ReadOnlySpan<char> value, out string result)
    {
        result = value.ToString();
        return true;
    }
}



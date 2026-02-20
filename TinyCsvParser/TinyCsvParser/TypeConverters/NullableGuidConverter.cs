// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace TinyCsvParser.TypeConverters;

public class NullableGuidConverter : NullableConverter<Guid?>
{
    private readonly string _format;
    public NullableGuidConverter() : this(string.Empty) { }
    public NullableGuidConverter(string format) { _format = format; }

    protected override bool InternalConvert(ReadOnlySpan<char> value, out Guid? result)
    {
        if (string.IsNullOrEmpty(_format))
        {
            if (Guid.TryParse(value, out Guid tempResult)) { result = tempResult; return true; }
        }
        else
        {
            if (Guid.TryParseExact(value, _format.AsSpan(), out Guid tempResult)) { result = tempResult; return true; }
        }
        result = null; return false;
    }
}



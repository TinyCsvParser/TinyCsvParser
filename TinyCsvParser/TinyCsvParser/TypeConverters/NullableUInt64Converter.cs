// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;

namespace TinyCsvParser.TypeConverters;

public class NullableUInt64Converter : NullableConverter<ulong?>
{
    private readonly IFormatProvider _formatProvider;
    private readonly NumberStyles _numberStyles;

    public NullableUInt64Converter() : this(CultureInfo.InvariantCulture) { }
    public NullableUInt64Converter(IFormatProvider formatProvider) : this(formatProvider, NumberStyles.Integer) { }
    public NullableUInt64Converter(IFormatProvider formatProvider, NumberStyles numberStyles)
    {
        _formatProvider = formatProvider;
        _numberStyles = numberStyles;
    }

    protected override bool InternalConvert(ReadOnlySpan<char> value, out ulong? result)
    {
        if (ulong.TryParse(value, _numberStyles, _formatProvider, out ulong tempResult))
        {
            result = tempResult;
            return true;
        }
        result = null;
        return false;
    }
}
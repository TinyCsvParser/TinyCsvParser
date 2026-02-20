// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;

namespace TinyCsvParser.TypeConverters;

public class NullableUInt32Converter : NullableConverter<uint?>
{
    private readonly IFormatProvider _formatProvider;
    private readonly NumberStyles _numberStyles;

    public NullableUInt32Converter() : this(CultureInfo.InvariantCulture) { }
    public NullableUInt32Converter(IFormatProvider formatProvider) : this(formatProvider, NumberStyles.Integer) { }
    public NullableUInt32Converter(IFormatProvider formatProvider, NumberStyles numberStyles)
    {
        _formatProvider = formatProvider;
        _numberStyles = numberStyles;
    }

    protected override bool InternalConvert(ReadOnlySpan<char> value, out uint? result)
    {
        if (uint.TryParse(value, _numberStyles, _formatProvider, out uint tempResult))
        {
            result = tempResult;
            return true;
        }
        result = null;
        return false;
    }
}
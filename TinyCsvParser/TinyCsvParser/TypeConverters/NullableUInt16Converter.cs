// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;

namespace TinyCsvParser.TypeConverters;

public class NullableUInt16Converter : NullableConverter<ushort?>
{
    private readonly IFormatProvider _formatProvider;
    private readonly NumberStyles _numberStyles;

    public NullableUInt16Converter() : this(CultureInfo.InvariantCulture) { }
    public NullableUInt16Converter(IFormatProvider formatProvider) : this(formatProvider, NumberStyles.Integer) { }
    public NullableUInt16Converter(IFormatProvider formatProvider, NumberStyles numberStyles)
    {
        _formatProvider = formatProvider;
        _numberStyles = numberStyles;
    }

    protected override bool InternalConvert(ReadOnlySpan<char> value, out ushort? result)
    {
        if (ushort.TryParse(value, _numberStyles, _formatProvider, out ushort tempResult))
        {
            result = tempResult;
            return true;
        }
        result = null;
        return false;
    }
}
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;

namespace TinyCsvParser.TypeConverters;

public class NullableInt16Converter : NullableConverter<short?>
{
    private readonly IFormatProvider _formatProvider;
    private readonly NumberStyles _numberStyles;

    public NullableInt16Converter() : this(CultureInfo.InvariantCulture) { }
    public NullableInt16Converter(IFormatProvider formatProvider) : this(formatProvider, NumberStyles.Integer) { }
    public NullableInt16Converter(IFormatProvider formatProvider, NumberStyles numberStyles)
    {
        _formatProvider = formatProvider;
        _numberStyles = numberStyles;
    }

    protected override bool InternalConvert(ReadOnlySpan<char> value, out short? result)
    {
        if (short.TryParse(value, _numberStyles, _formatProvider, out short tempResult))
        {
            result = tempResult;
            return true;
        }
        result = null;
        return false;
    }
}
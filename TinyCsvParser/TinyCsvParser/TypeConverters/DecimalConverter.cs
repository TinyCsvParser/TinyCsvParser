// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;

namespace TinyCsvParser.TypeConverters;

public class DecimalConverter : NonNullableConverter<decimal>
{
    private readonly IFormatProvider _formatProvider;
    private readonly NumberStyles _numberStyles;

    public DecimalConverter() : this(CultureInfo.InvariantCulture) { }
    public DecimalConverter(IFormatProvider formatProvider) : this(formatProvider, NumberStyles.Number) { }
    public DecimalConverter(IFormatProvider formatProvider, NumberStyles numberStyles)
    {
        _formatProvider = formatProvider;
        _numberStyles = numberStyles;
    }

    protected override bool InternalConvert(ReadOnlySpan<char> value, out decimal result)
    {
        return decimal.TryParse(value, _numberStyles, _formatProvider, out result);
    }
}



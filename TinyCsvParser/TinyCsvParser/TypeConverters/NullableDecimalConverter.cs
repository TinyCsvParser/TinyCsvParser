// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;

namespace TinyCsvParser.TypeConverters;

public class NullableDecimalConverter : NullableConverter<decimal?>
{
    private readonly IFormatProvider _formatProvider;
    private readonly NumberStyles _numberStyles;

    public NullableDecimalConverter() : this(CultureInfo.InvariantCulture) { }
    public NullableDecimalConverter(IFormatProvider formatProvider) : this(formatProvider, NumberStyles.Number) { }
    public NullableDecimalConverter(IFormatProvider formatProvider, NumberStyles numberStyles)
    {
        _formatProvider = formatProvider;
        _numberStyles = numberStyles;
    }

    protected override bool InternalConvert(ReadOnlySpan<char> value, out decimal? result)
    {
        if (decimal.TryParse(value, _numberStyles, _formatProvider, out decimal tempResult))
        {
            result = tempResult;
            return true;
        }
        result = null;
        return false;
    }
}



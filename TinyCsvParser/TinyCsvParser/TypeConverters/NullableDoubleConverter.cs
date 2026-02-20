// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;

namespace TinyCsvParser.TypeConverters;

public class NullableDoubleConverter : NullableConverter<double?>
{
    private readonly IFormatProvider _formatProvider;
    private readonly NumberStyles _numberStyles;

    public NullableDoubleConverter() : this(CultureInfo.InvariantCulture) { }
    public NullableDoubleConverter(IFormatProvider formatProvider) : this(formatProvider, NumberStyles.Float | NumberStyles.AllowThousands) { }
    public NullableDoubleConverter(IFormatProvider formatProvider, NumberStyles numberStyles)
    {
        _formatProvider = formatProvider;
        _numberStyles = numberStyles;
    }

    protected override bool InternalConvert(ReadOnlySpan<char> value, out double? result)
    {
        if (double.TryParse(value, _numberStyles, _formatProvider, out double tempResult))
        {
            result = tempResult;
            return true;
        }
        result = null;
        return false;
    }
}



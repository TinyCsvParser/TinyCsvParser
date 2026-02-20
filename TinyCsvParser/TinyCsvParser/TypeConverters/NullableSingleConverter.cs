// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;

namespace TinyCsvParser.TypeConverters;

public class NullableSingleConverter : NullableConverter<float?>
{
    private readonly IFormatProvider _formatProvider;
    private readonly NumberStyles _numberStyles;

    public NullableSingleConverter() : this(CultureInfo.InvariantCulture) { }
    public NullableSingleConverter(IFormatProvider formatProvider) : this(formatProvider, NumberStyles.Float | NumberStyles.AllowThousands) { }
    public NullableSingleConverter(IFormatProvider formatProvider, NumberStyles numberStyles)
    {
        _formatProvider = formatProvider;
        _numberStyles = numberStyles;
    }

    protected override bool InternalConvert(ReadOnlySpan<char> value, out float? result)
    {
        if (float.TryParse(value, _numberStyles, _formatProvider, out float tempResult))
        {
            result = tempResult;
            return true;
        }
        result = null;
        return false;
    }
}
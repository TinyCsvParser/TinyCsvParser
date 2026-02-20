// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;

namespace TinyCsvParser.TypeConverters;

public class NullableInt32Converter : NullableConverter<int?>
{
    private readonly IFormatProvider _formatProvider;
    private readonly NumberStyles _numberStyles;

    public NullableInt32Converter() : this(CultureInfo.InvariantCulture) { }
    public NullableInt32Converter(IFormatProvider formatProvider) : this(formatProvider, NumberStyles.Integer) { }
    public NullableInt32Converter(IFormatProvider formatProvider, NumberStyles numberStyles)
    {
        _formatProvider = formatProvider;
        _numberStyles = numberStyles;
    }

    protected override bool InternalConvert(ReadOnlySpan<char> value, out int? result)
    {
        if (int.TryParse(value, _numberStyles, _formatProvider, out int tempResult))
        {
            result = tempResult;
            return true;
        }
        result = null;
        return false;
    }
}
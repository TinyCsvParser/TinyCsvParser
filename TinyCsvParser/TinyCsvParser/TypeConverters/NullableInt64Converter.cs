// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;

namespace TinyCsvParser.TypeConverters;

public class NullableInt64Converter : NullableConverter<long?>
{
    private readonly IFormatProvider _formatProvider;
    private readonly NumberStyles _numberStyles;

    public NullableInt64Converter() : this(CultureInfo.InvariantCulture) { }
    public NullableInt64Converter(IFormatProvider formatProvider) : this(formatProvider, NumberStyles.Integer) { }
    public NullableInt64Converter(IFormatProvider formatProvider, NumberStyles numberStyles)
    {
        _formatProvider = formatProvider;
        _numberStyles = numberStyles;
    }

    protected override bool InternalConvert(ReadOnlySpan<char> value, out long? result)
    {
        if (long.TryParse(value, _numberStyles, _formatProvider, out long tempResult))
        {
            result = tempResult;
            return true;
        }
        result = null;
        return false;
    }
}



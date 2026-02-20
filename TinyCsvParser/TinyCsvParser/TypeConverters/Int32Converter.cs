// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;

namespace TinyCsvParser.TypeConverters;

public class Int32Converter : NonNullableConverter<int>
{
    private readonly IFormatProvider _formatProvider;
    private readonly NumberStyles _numberStyles;

    public Int32Converter() : this(CultureInfo.InvariantCulture) { }
    public Int32Converter(IFormatProvider formatProvider) : this(formatProvider, NumberStyles.Integer) { }
    public Int32Converter(IFormatProvider formatProvider, NumberStyles numberStyles)
    {
        _formatProvider = formatProvider;
        _numberStyles = numberStyles;
    }

    protected override bool InternalConvert(ReadOnlySpan<char> value, out int result)
    {
        return int.TryParse(value, _numberStyles, _formatProvider, out result);
    }
}



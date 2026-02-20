// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;

namespace TinyCsvParser.TypeConverters;

public class UInt64Converter : NonNullableConverter<ulong>
{
    private readonly IFormatProvider _formatProvider;
    private readonly NumberStyles _numberStyles;

    public UInt64Converter() : this(CultureInfo.InvariantCulture) { }
    public UInt64Converter(IFormatProvider formatProvider) : this(formatProvider, NumberStyles.Integer) { }
    public UInt64Converter(IFormatProvider formatProvider, NumberStyles numberStyles)
    {
        _formatProvider = formatProvider;
        _numberStyles = numberStyles;
    }

    protected override bool InternalConvert(ReadOnlySpan<char> value, out ulong result)
    {
        return ulong.TryParse(value, _numberStyles, _formatProvider, out result);
    }
}



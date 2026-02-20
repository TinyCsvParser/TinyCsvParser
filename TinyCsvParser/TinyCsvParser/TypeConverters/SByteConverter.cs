// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;

namespace TinyCsvParser.TypeConverters;

public class SByteConverter : NonNullableConverter<sbyte>
{
    private readonly IFormatProvider _formatProvider;
    private readonly NumberStyles _numberStyles;

    public SByteConverter() : this(CultureInfo.InvariantCulture) { }
    public SByteConverter(IFormatProvider formatProvider) : this(formatProvider, NumberStyles.Integer) { }
    public SByteConverter(IFormatProvider formatProvider, NumberStyles numberStyles)
    {
        _formatProvider = formatProvider;
        _numberStyles = numberStyles;
    }

    protected override bool InternalConvert(ReadOnlySpan<char> value, out sbyte result)
    {
        return sbyte.TryParse(value, _numberStyles, _formatProvider, out result);
    }
}
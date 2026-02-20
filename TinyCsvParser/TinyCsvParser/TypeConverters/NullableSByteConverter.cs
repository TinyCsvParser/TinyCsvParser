// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;

namespace TinyCsvParser.TypeConverters;

public class NullableSByteConverter : NullableConverter<sbyte?>
{
    private readonly IFormatProvider _formatProvider;
    private readonly NumberStyles _numberStyles;

    public NullableSByteConverter() : this(CultureInfo.InvariantCulture) { }
    public NullableSByteConverter(IFormatProvider formatProvider) : this(formatProvider, NumberStyles.Integer) { }
    public NullableSByteConverter(IFormatProvider formatProvider, NumberStyles numberStyles)
    {
        _formatProvider = formatProvider;
        _numberStyles = numberStyles;
    }

    protected override bool InternalConvert(ReadOnlySpan<char> value, out sbyte? result)
    {
        if (sbyte.TryParse(value, _numberStyles, _formatProvider, out sbyte tempResult))
        {
            result = tempResult;
            return true;
        }
        result = null;
        return false;
    }
}



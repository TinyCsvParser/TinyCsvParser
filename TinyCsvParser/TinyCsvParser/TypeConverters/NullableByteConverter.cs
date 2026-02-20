// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;

namespace TinyCsvParser.TypeConverters;

public class NullableByteConverter : NullableConverter<byte?>
{
    private readonly IFormatProvider _formatProvider;
    private readonly NumberStyles _numberStyles;

    public NullableByteConverter() : this(CultureInfo.InvariantCulture) { }
    public NullableByteConverter(IFormatProvider formatProvider) : this(formatProvider, NumberStyles.Integer) { }
    public NullableByteConverter(IFormatProvider formatProvider, NumberStyles numberStyles)
    {
        _formatProvider = formatProvider;
        _numberStyles = numberStyles;
    }

    protected override bool InternalConvert(ReadOnlySpan<char> value, out byte? result)
    {
        if (byte.TryParse(value, _numberStyles, _formatProvider, out byte tempResult))
        {
            result = tempResult;
            return true;
        }
        result = null;
        return false;
    }
}



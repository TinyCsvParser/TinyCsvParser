// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;

namespace TinyCsvParser.TypeConverters;

public class NullableTimeSpanConverter : NullableConverter<TimeSpan?>
{
    private readonly string _format;
    private readonly IFormatProvider _formatProvider;
    private readonly TimeSpanStyles _styles;

    public NullableTimeSpanConverter() : this(string.Empty) { }
    public NullableTimeSpanConverter(string format) : this(format, CultureInfo.InvariantCulture) { }
    public NullableTimeSpanConverter(string format, IFormatProvider formatProvider) : this(format, formatProvider, TimeSpanStyles.None) { }
    public NullableTimeSpanConverter(string format, IFormatProvider formatProvider, TimeSpanStyles styles)
    {
        _format = format; _formatProvider = formatProvider; _styles = styles;
    }

    protected override bool InternalConvert(ReadOnlySpan<char> value, out TimeSpan? result)
    {
        if (string.IsNullOrEmpty(_format))
        {
            if (TimeSpan.TryParse(value, _formatProvider, out TimeSpan tempResult)) { result = tempResult; return true; }
        }
        else
        {
            if (TimeSpan.TryParseExact(value, _format.AsSpan(), _formatProvider, _styles, out TimeSpan tempResult)) { result = tempResult; return true; }
        }
        result = null; return false;
    }
}



// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;

namespace TinyCsvParser.TypeConverters;

public class TimeSpanConverter : NonNullableConverter<TimeSpan>
{
    private readonly string _format;
    private readonly IFormatProvider _formatProvider;
    private readonly TimeSpanStyles _styles;

    public TimeSpanConverter() : this(string.Empty) { }
    public TimeSpanConverter(string format) : this(format, CultureInfo.InvariantCulture) { }
    public TimeSpanConverter(string format, IFormatProvider formatProvider) : this(format, formatProvider, TimeSpanStyles.None) { }
    public TimeSpanConverter(string format, IFormatProvider formatProvider, TimeSpanStyles styles)
    {
        _format = format; _formatProvider = formatProvider; _styles = styles;
    }

    protected override bool InternalConvert(ReadOnlySpan<char> value, out TimeSpan result)
    {
        if (string.IsNullOrEmpty(_format)) return TimeSpan.TryParse(value, _formatProvider, out result);
        return TimeSpan.TryParseExact(value, _format.AsSpan(), _formatProvider, _styles, out result);
    }
}



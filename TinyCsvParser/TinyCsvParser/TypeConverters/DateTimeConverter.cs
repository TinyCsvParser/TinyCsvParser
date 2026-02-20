// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;

namespace TinyCsvParser.TypeConverters;

public class DateTimeConverter : NonNullableConverter<DateTime>
{
    private readonly string _dateTimeFormat;
    private readonly IFormatProvider _formatProvider;
    private readonly DateTimeStyles _dateTimeStyles;

    public DateTimeConverter() : this(string.Empty) { }
    public DateTimeConverter(string dateTimeFormat) : this(dateTimeFormat, CultureInfo.InvariantCulture) { }
    public DateTimeConverter(string dateTimeFormat, IFormatProvider formatProvider) : this(dateTimeFormat, formatProvider, DateTimeStyles.None) { }
    public DateTimeConverter(string dateTimeFormat, IFormatProvider formatProvider, DateTimeStyles dateTimeStyles)
    {
        _dateTimeFormat = dateTimeFormat; _formatProvider = formatProvider; _dateTimeStyles = dateTimeStyles;
    }

    protected override bool InternalConvert(ReadOnlySpan<char> value, out DateTime result)
    {
        if (string.IsNullOrWhiteSpace(_dateTimeFormat)) return DateTime.TryParse(value, _formatProvider, _dateTimeStyles, out result);
        return DateTime.TryParseExact(value, _dateTimeFormat.AsSpan(), _formatProvider, _dateTimeStyles, out result);
    }
}



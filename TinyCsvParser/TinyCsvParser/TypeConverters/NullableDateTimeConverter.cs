// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;

namespace TinyCsvParser.TypeConverters;

public class NullableDateTimeConverter : NullableConverter<DateTime?>
{
    private readonly string _dateTimeFormat;
    private readonly IFormatProvider _formatProvider;
    private readonly DateTimeStyles _dateTimeStyles;

    public NullableDateTimeConverter() : this(string.Empty) { }
    public NullableDateTimeConverter(string dateTimeFormat) : this(dateTimeFormat, CultureInfo.InvariantCulture) { }
    public NullableDateTimeConverter(string dateTimeFormat, IFormatProvider formatProvider) : this(dateTimeFormat, formatProvider, DateTimeStyles.None) { }
    public NullableDateTimeConverter(string dateTimeFormat, IFormatProvider formatProvider, DateTimeStyles dateTimeStyles)
    {
        _dateTimeFormat = dateTimeFormat; _formatProvider = formatProvider; _dateTimeStyles = dateTimeStyles;
    }

    protected override bool InternalConvert(ReadOnlySpan<char> value, out DateTime? result)
    {
        if (string.IsNullOrWhiteSpace(_dateTimeFormat))
        {
            if (DateTime.TryParse(value, _formatProvider, _dateTimeStyles, out DateTime tempResult)) { result = tempResult; return true; }
        }
        else
        {
            if (DateTime.TryParseExact(value, _dateTimeFormat.AsSpan(), _formatProvider, _dateTimeStyles, out DateTime tempResult)) { result = tempResult; return true; }
        }
        result = null; return false;
    }
}



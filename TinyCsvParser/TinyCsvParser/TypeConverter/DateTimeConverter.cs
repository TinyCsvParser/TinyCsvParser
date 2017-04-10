// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;

namespace TinyCsvParser.TypeConverter
{
  public class DateTimeConverter : NonNullableConverter<DateTime>
  {
    private readonly string _dateTimeFormat;
    private readonly DateTimeStyles _dateTimeStyles;
    private readonly IFormatProvider _formatProvider;

    public DateTimeConverter()
      : this(string.Empty)
    {
    }

    public DateTimeConverter(string dateTimeFormat)
      : this(dateTimeFormat, CultureInfo.InvariantCulture)
    {
    }

    public DateTimeConverter(string dateTimeFormat, IFormatProvider formatProvider)
      : this(dateTimeFormat, formatProvider, DateTimeStyles.None)
    {
    }

    public DateTimeConverter(string dateTimeFormat, IFormatProvider formatProvider, DateTimeStyles dateTimeStyles)
    {
      _dateTimeFormat = dateTimeFormat;
      _formatProvider = formatProvider;
      _dateTimeStyles = dateTimeStyles;
    }

    protected override bool InternalConvert(string value, out DateTime result)
    {
      if (string.IsNullOrWhiteSpace(_dateTimeFormat))
        return DateTime.TryParse(value, out result);
      return DateTime.TryParseExact(value, _dateTimeFormat, _formatProvider, _dateTimeStyles, out result);
    }
  }
}
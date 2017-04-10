// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;

namespace TinyCsvParser.TypeConverter
{
  public class TimeSpanConverter : NonNullableConverter<TimeSpan>
  {
    private readonly string _format;
    private readonly IFormatProvider _formatProvider;
    private readonly TimeSpanStyles _timeSpanStyles;

    public TimeSpanConverter()
      : this(string.Empty)
    {
    }

    public TimeSpanConverter(string format)
      : this(format, CultureInfo.InvariantCulture)
    {
    }

    public TimeSpanConverter(string format, IFormatProvider formatProvider)
      : this(format, formatProvider, TimeSpanStyles.None)
    {
    }

    public TimeSpanConverter(string format, IFormatProvider formatProvider, TimeSpanStyles timeSpanStyles)
    {
      _format = format;
      _formatProvider = formatProvider;
      _timeSpanStyles = timeSpanStyles;
    }

    protected override bool InternalConvert(string value, out TimeSpan result)
    {
      if (string.IsNullOrWhiteSpace(_format))
        return TimeSpan.TryParse(value, _formatProvider, out result);
      return TimeSpan.TryParseExact(value, _format, _formatProvider, _timeSpanStyles, out result);
    }
  }
}
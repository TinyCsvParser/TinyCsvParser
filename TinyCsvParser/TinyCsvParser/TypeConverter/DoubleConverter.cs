// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;

namespace TinyCsvParser.TypeConverter
{
  public class DoubleConverter : NonNullableConverter<double>
  {
    private readonly IFormatProvider _formatProvider;
    private readonly NumberStyles _numberStyles;

    public DoubleConverter()
      : this(CultureInfo.InvariantCulture)
    {
    }

    public DoubleConverter(IFormatProvider formatProvider)
      : this(formatProvider, NumberStyles.Float | NumberStyles.AllowThousands)
    {
    }

    public DoubleConverter(IFormatProvider formatProvider, NumberStyles numberStyles)
    {
      _formatProvider = formatProvider;
      _numberStyles = numberStyles;
    }

    protected override bool InternalConvert(string value, out double result)
    {
      return double.TryParse(value, _numberStyles, _formatProvider, out result);
    }
  }
}
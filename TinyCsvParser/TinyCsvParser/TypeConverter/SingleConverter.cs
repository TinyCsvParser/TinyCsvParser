// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;

namespace TinyCsvParser.TypeConverter
{
  public class SingleConverter : NonNullableConverter<float>
  {
    private readonly IFormatProvider _formatProvider;
    private readonly NumberStyles _numberStyles;

    public SingleConverter()
      : this(CultureInfo.InvariantCulture)
    {
    }

    public SingleConverter(IFormatProvider formatProvider)
      : this(formatProvider, NumberStyles.Float | NumberStyles.AllowThousands)
    {
    }

    public SingleConverter(IFormatProvider formatProvider, NumberStyles numberStyles)
    {
      _formatProvider = formatProvider;
      _numberStyles = numberStyles;
    }

    protected override bool InternalConvert(string value, out float result)
    {
      return float.TryParse(value, _numberStyles, _formatProvider, out result);
    }
  }
}
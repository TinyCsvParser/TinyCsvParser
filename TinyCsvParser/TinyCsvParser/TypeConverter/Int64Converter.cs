// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;

namespace TinyCsvParser.TypeConverter
{
  public class Int64Converter : NonNullableConverter<long>
  {
    private readonly IFormatProvider _formatProvider;
    private readonly NumberStyles _numberStyles;

    public Int64Converter()
      : this(CultureInfo.InvariantCulture)
    {
    }

    public Int64Converter(IFormatProvider formatProvider)
      : this(formatProvider, NumberStyles.Integer)
    {
    }

    public Int64Converter(IFormatProvider formatProvider, NumberStyles numberStyles)
    {
      _formatProvider = formatProvider;
      _numberStyles = numberStyles;
    }

    protected override bool InternalConvert(string value, out long result)
    {
      return long.TryParse(value, _numberStyles, _formatProvider, out result);
    }
  }
}
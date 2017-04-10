// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;

namespace TinyCsvParser.TypeConverter
{
  public class ByteConverter : NonNullableConverter<byte>
  {
    private readonly IFormatProvider _formatProvider;
    private readonly NumberStyles _numberStyles;

    public ByteConverter()
      : this(CultureInfo.InvariantCulture)
    {
    }

    public ByteConverter(IFormatProvider formatProvider)
      : this(formatProvider, NumberStyles.Integer)
    {
    }

    public ByteConverter(IFormatProvider formatProvider, NumberStyles numberStyles)
    {
      _formatProvider = formatProvider;
      _numberStyles = numberStyles;
    }


    protected override bool InternalConvert(string value, out byte result)
    {
      return byte.TryParse(value, _numberStyles, _formatProvider, out result);
    }
  }
}
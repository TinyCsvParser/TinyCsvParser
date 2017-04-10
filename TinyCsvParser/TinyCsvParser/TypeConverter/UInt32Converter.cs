// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;

namespace TinyCsvParser.TypeConverter
{
  public class UInt32Converter : NonNullableConverter<uint>
  {
    private readonly IFormatProvider _formatProvider;
    private readonly NumberStyles _numberStyles;

    public UInt32Converter()
      : this(CultureInfo.InvariantCulture)
    {
    }

    public UInt32Converter(IFormatProvider formatProvider)
      : this(formatProvider, NumberStyles.Integer)
    {
    }

    public UInt32Converter(IFormatProvider formatProvider, NumberStyles numberStyles)
    {
      _formatProvider = formatProvider;
      _numberStyles = numberStyles;
    }

    protected override bool InternalConvert(string value, out uint result)
    {
      return uint.TryParse(value, _numberStyles, _formatProvider, out result);
    }
  }
}
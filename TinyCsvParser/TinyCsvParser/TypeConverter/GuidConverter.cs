// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace TinyCsvParser.TypeConverter
{
  public class GuidConverter : NonNullableConverter<Guid>
  {
    private readonly string _format;

    public GuidConverter()
      : this(string.Empty)
    {
    }

    public GuidConverter(string format)
    {
      _format = format;
    }

    protected override bool InternalConvert(string value, out Guid result)
    {
      if (string.IsNullOrWhiteSpace(_format))
        return Guid.TryParse(value, out result);
      return Guid.TryParseExact(value, _format, out result);
    }
  }
}
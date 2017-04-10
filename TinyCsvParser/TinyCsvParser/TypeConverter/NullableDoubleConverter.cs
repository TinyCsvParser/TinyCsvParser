﻿// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;

namespace TinyCsvParser.TypeConverter
{
  public class NullableDoubleConverter : NullableInnerConverter<double>
  {
    public NullableDoubleConverter()
      : base(new DoubleConverter())
    {
    }

    public NullableDoubleConverter(IFormatProvider formatProvider)
      : base(new DoubleConverter(formatProvider))
    {
    }

    public NullableDoubleConverter(IFormatProvider formatProvider, NumberStyles numberStyles)
      : base(new DoubleConverter(formatProvider, numberStyles))
    {
    }
  }
}
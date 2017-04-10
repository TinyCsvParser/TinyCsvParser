// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace TinyCsvParser.TypeConverter
{
  public class BoolConverter : NonNullableConverter<bool>
  {
    private readonly string _falseValue;
    private readonly StringComparison _stringComparism;
    private readonly string _trueValue;

    public BoolConverter()
      : this("true", "false", StringComparison.OrdinalIgnoreCase)
    {
    }

    public BoolConverter(string trueValue, string falseValue, StringComparison stringComparism)
    {
      _trueValue = trueValue;
      _falseValue = falseValue;
      _stringComparism = stringComparism;
    }

    protected override bool InternalConvert(string value, out bool result)
    {
      result = false;

      if (string.Equals(_trueValue, value, _stringComparism))
      {
        result = true;

        return true;
      }

      if (string.Equals(_falseValue, value, _stringComparism))
      {
        result = false;

        return true;
      }

      return false;
    }
  }
}
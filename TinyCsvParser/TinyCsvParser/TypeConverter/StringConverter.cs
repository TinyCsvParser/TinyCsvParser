﻿// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace TinyCsvParser.TypeConverter
{
  public class StringConverter : BaseConverter<string>
  {
    public override bool TryConvert(string value, out string result)
    {
      result = value;

      return true;
    }
  }
}
﻿// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace TinyCsvParser.TypeConverter
{
  public abstract class NullableConverter<TTargetType> : BaseConverter<TTargetType>
  {
    public override bool TryConvert(string value, out TTargetType result)
    {
      if (string.IsNullOrWhiteSpace(value))
      {
        result = default(TTargetType);

        return true;
      }

      return InternalConvert(value, out result);
    }

    protected abstract bool InternalConvert(string value, out TTargetType result);
  }
}
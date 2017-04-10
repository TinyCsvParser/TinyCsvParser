﻿// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace TinyCsvParser.TypeConverter
{
  public abstract class NullableInnerConverter<TTargetType> : NullableConverter<TTargetType?>
    where TTargetType : struct
  {
    private readonly NonNullableConverter<TTargetType> _internalConverter;

    public NullableInnerConverter(NonNullableConverter<TTargetType> internalConverter)
    {
      _internalConverter = internalConverter;
    }

    protected override bool InternalConvert(string value, out TTargetType? result)
    {
      result = default(TTargetType?);

      TTargetType innerConverterResult;

      if (_internalConverter.TryConvert(value, out innerConverterResult))
      {
        result = innerConverterResult;

        return true;
      }

      return false;
    }
  }
}
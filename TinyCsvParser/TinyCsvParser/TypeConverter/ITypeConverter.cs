// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace TinyCsvParser.TypeConverter
{
  public interface ITypeConverter
  {
  }

  public interface ITypeConverter<TTargetType> : ITypeConverter
  {
    Type TargetType { get; }
    bool TryConvert(string value, out TTargetType result);
  }
}
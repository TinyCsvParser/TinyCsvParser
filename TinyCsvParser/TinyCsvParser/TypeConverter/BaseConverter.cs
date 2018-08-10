﻿// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace TinyCsvParser.TypeConverter
{
    public abstract class BaseConverter<TTargetType> : ITypeConverter<TTargetType>
    {
        public abstract bool TryConvert(ReadOnlySpan<char> value, out TTargetType result);

        public Type TargetType
        {
            get { return typeof(TTargetType); }
        }
    }
}

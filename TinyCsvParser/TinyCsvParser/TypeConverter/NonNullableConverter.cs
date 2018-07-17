// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using TinyCsvParser.Exceptions;

namespace TinyCsvParser.TypeConverter
{
    public abstract class NonNullableConverter<TTargetType> : BaseConverter<TTargetType>
    {
        public override bool TryConvert(ReadOnlySpan<char> value, out TTargetType result)
        {
            if (value.IsWhiteSpace())
            {
                result = default(TTargetType);

                return false;
            }
            return InternalConvert(value, out result);
            
        }

        protected abstract bool InternalConvert(ReadOnlySpan<char> value, out TTargetType result);
    }
}

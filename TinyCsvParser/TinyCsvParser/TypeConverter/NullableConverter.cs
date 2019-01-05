// Copyright (c) Philipp Wagner and Joel Mueller. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace TinyCsvParser.TypeConverter
{
    public abstract class NullableConverter<TTargetType> : BaseConverter<TTargetType>
    {
        public override bool TryConvert(ReadOnlySpan<char> value, out TTargetType result)
        {
            if (value.IsWhiteSpace())
            {
                result = default(TTargetType);
                return true;
            }

            return InternalConvert(value, out result);
        }

        protected abstract bool InternalConvert(ReadOnlySpan<char> value, out TTargetType result);
    }
}

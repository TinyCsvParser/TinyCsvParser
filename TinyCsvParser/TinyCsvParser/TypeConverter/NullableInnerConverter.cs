// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using TinyCsvParser.Exceptions;

namespace TinyCsvParser.TypeConverter
{
    public abstract class NullableInnerConverter<TTargetType> : NullableConverter<TTargetType?>
        where TTargetType : struct
    {
        private readonly NonNullableConverter<TTargetType> internalConverter;

        public NullableInnerConverter(NonNullableConverter<TTargetType> internalConverter)
        {
            this.internalConverter = internalConverter;
        }

        protected override bool InternalConvert(string value, out TTargetType? result)
        {
            result = default(TTargetType?);

            TTargetType innerConverterResult;

            if (internalConverter.TryConvert(value, out innerConverterResult))
            {
                result = innerConverterResult;

                return true;
            }

            return false;
        }
    }
}

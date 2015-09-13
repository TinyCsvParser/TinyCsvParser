// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using TinyCsvParser.Exceptions;

namespace TinyCsvParser.TypeConverter
{
    public abstract class NonNullableConverter<TTargetType> : BaseConverter<TTargetType>
    {
        public override TTargetType Convert(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new CsvTypeConversionException("No value given");
            }

            try
            {
                return InternalConvert(value);
            }
            catch (Exception e)
            {
                throw new CsvTypeConversionException(string.Format("Unable to parse value {0}", value), e);
            }
        }

        protected abstract TTargetType InternalConvert(string value);
    }
}

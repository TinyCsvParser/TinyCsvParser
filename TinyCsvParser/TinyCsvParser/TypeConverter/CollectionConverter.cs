// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace TinyCsvParser.TypeConverter
{
    public class CollectionsConverter<TTargetType> : IArrayTypeConverter<IEnumerable<TTargetType>>
    {
        private readonly ITypeConverter<TTargetType> internalConverter;

        public CollectionsConverter(ITypeConverter<TTargetType> internalConverter)
        {
            this.internalConverter = internalConverter;
        }

        public bool TryConvert(string[] values, out IEnumerable<TTargetType> result)
        {
            result = Enumerable.Empty<TTargetType>();

            // Temporary Array. Better use Spans to minimize allocations:
            var arr = new TTargetType[values.Length];

            for(int pos = 0; pos < values.Length; pos++)
            {
                TTargetType element = default(TTargetType);

                // If we cannot convert any item, return:
                if (!internalConverter.TryConvert(values[pos], out element))
                {
                    return false;
                }

                arr[pos] = element;
            }

            result = arr;

            return true;
        }

        public Type TargetType => typeof(TTargetType);
    }
}

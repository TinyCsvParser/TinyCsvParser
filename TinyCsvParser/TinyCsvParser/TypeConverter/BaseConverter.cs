// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace TinyCsvParser.TypeConverter
{
    public abstract class BaseConverter<TTargetType> : ITypeConverter<TTargetType>
    {
        public abstract bool TryConvert(string value, out TTargetType result);

        public Type TargetType => typeof(TTargetType);
    }
}

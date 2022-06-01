// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace TinyCsvParser.TypeConverter
{
    public class NullableEnumConverter<TTargetType> : NullableInnerConverter<TTargetType> where TTargetType : struct
    {
        public NullableEnumConverter()
            : base (new EnumConverter<TTargetType>())
        {
        }

        public NullableEnumConverter(bool ignoreCase)
            : base(new EnumConverter<TTargetType>(ignoreCase))
        {
        }
    }
}

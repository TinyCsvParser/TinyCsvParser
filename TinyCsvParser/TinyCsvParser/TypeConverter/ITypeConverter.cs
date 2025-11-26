// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace TinyCsvParser.TypeConverter
{
    public interface ITypeConverter
    {
        Type TargetType { get; }
    }

    public interface ITypeConverter<TTargetType> : ITypeConverter
    {
        bool TryConvert(string value, out TTargetType result);
    }

    public interface IArrayTypeConverter<TTargetType> : ITypeConverter
    {
        bool TryConvert(string[] value, out TTargetType result);
    }
}

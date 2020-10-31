// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace TinyCsvParser.TypeConverter
{
    public interface ITypeConverterProvider
    {
        ITypeConverter<TTargetType> Resolve<TTargetType>();

        IArrayTypeConverter<TTargetType> ResolveCollection<TTargetType>();
    }
}

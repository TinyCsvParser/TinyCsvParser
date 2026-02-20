// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace TinyCsvParser.TypeConverters;


public interface ITypeConverterProvider
{
    ITypeConverter<T> Resolve<T>();
}
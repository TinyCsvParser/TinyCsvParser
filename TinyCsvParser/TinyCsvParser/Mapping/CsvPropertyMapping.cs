// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq.Expressions;
using TinyCsvParser.Reflection;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.Mapping
{
    public class CsvPropertyMapping<TEntity, TProperty> : ICsvPropertyMapping<TEntity, string>
        where TEntity : class, new()
    {
        private readonly string propertyName;
        private readonly ITypeConverter<TProperty> propertyConverter;
        private readonly Action<TEntity, TProperty> propertySetter;

        public CsvPropertyMapping(Expression<Func<TEntity, TProperty>> property, ITypeConverter<TProperty> typeConverter) 
        {
            propertyConverter = typeConverter;
            propertyName = ReflectionUtils.GetPropertyNameFromExpression(property);
            propertySetter = ReflectionUtils.CreateSetter<TEntity, TProperty>(property);
        }

        public bool TryMapValue(TEntity entity, string value) 
        {
            if (!propertyConverter.TryConvert(value, out var convertedValue))
            {
                return false;
            }

            propertySetter(entity, convertedValue);

            return true;
        }
        
        public override string ToString()
        {
            return $"CsvPropertyMapping (PropertyName = {propertyName}, Converter = {propertyConverter})";
        }
    }
}
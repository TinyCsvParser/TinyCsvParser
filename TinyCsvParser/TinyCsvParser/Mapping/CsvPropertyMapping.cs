// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq.Expressions;
using TinyCsvParser.Reflection;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.Mapping
{
    public class CsvPropertyMapping<TEntity, TProperty> : ICsvPropertyMapping<TEntity>
        where TEntity : new()
    {
        private readonly string _propertyName;
        private readonly ITypeConverter<TProperty> _propertyConverter;
        private readonly Action<TEntity, TProperty> _propertySetter;

        public CsvPropertyMapping(Expression<Func<TEntity, TProperty>> property, ITypeConverter<TProperty> typeConverter) 
        {
            _propertyConverter = typeConverter;
            _propertyName = ReflectionUtils.GetPropertyNameFromExpression(property);
            _propertySetter = ReflectionUtils.CreateSetter(property);
        }

        public bool TryMapValue(TEntity entity, ReadOnlySpan<char> value) 
        {
            if (!_propertyConverter.TryConvert(value, out TProperty convertedValue))
            {
                return false;
            }

            _propertySetter(entity, convertedValue);

            return true;
        }
        
        public override string ToString()
        {
            return $"CsvPropertyMapping (PropertyName = {_propertyName}, Converter = {_propertyConverter})";
        }
    }
}
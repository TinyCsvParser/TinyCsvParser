// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq.Expressions;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.Mapping
{
    public class CsvPropertyMapping<TEntity, TProperty> : ICsvPropertyMapping<TEntity>
        where TEntity : class, new()
    {
        ITypeConverter<TProperty> propertyConverter;
        Action<TEntity, TProperty> propertySetter;

        public CsvPropertyMapping(Expression<Func<TEntity, TProperty>> property, ITypeConverterProvider typeConverterProvider) 
        {
            propertyConverter = typeConverterProvider.Resolve<TProperty>();
            propertySetter = ReflectionUtils.CreateSetter<TEntity, TProperty>(property);
        }

        public void MapValue(TEntity entity, string value) 
        {
            TProperty convertedValue = propertyConverter.Convert(value);

            propertySetter(entity, convertedValue);
        }
    }
}
﻿// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq.Expressions;
using TinyCsvParser.Reflection;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.Mapping
{
  public class CsvPropertyMapping<TEntity, TProperty> : ICsvPropertyMapping<TEntity>
      where TEntity : class, new()
  {
    private readonly string _propertyName;
    private ITypeConverter<TProperty> _propertyConverter;
    private readonly Action<TEntity, TProperty> _propertySetter;

    public CsvPropertyMapping(Expression<Func<TEntity, TProperty>> property, ITypeConverter<TProperty> typeConverter)
    {
      _propertyConverter = typeConverter;
      _propertyName = ReflectionUtils.GetPropertyNameFromExpression(property);
      _propertySetter = ReflectionUtils.CreateSetter<TEntity, TProperty>(property);
    }

    public bool TryMapValue(TEntity entity, string value)
    {
      TProperty convertedValue;

      if (!_propertyConverter.TryConvert(value, out convertedValue))
      {
        return false;
      }

      _propertySetter(entity, convertedValue);

      return true;
    }

    public void WithCustomConverter(ITypeConverter<TProperty> typeConverter)
    {
      _propertyConverter = typeConverter;
    }

    public override string ToString()
    {
      return string.Format("CsvPropertyMapping (PropertyName = {0}, Converter = {1})", _propertyName, _propertyConverter);
    }
  }
}
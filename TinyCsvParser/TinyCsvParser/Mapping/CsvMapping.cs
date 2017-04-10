﻿// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using TinyCsvParser.TypeConverter;
using TinyCsvParser.Reflection;

namespace TinyCsvParser.Mapping
{
  public abstract class CsvMapping<TEntity>
      where TEntity : class, new()
  {
    private class IndexToPropertyMapping
    {
      public int ColumnIndex { get; set; }

      public ICsvPropertyMapping<TEntity> PropertyMapping { get; set; }

      public override string ToString()
      {
        return string.Format("IndexToPropertyMapping (ColumnIndex = {0}, PropertyMapping = {1}", ColumnIndex, PropertyMapping);
      }
    }

    private readonly Func<TEntity> _invokeConstructor;
    private readonly ITypeConverterProvider _typeConverterProvider;
    private readonly List<IndexToPropertyMapping> _csvPropertyMappings;

    protected CsvMapping()
        : this(new TypeConverterProvider())
    {
    }

    /// <summary>
    /// Last index of csvPropertyMappings, adjusted for zero based index
    /// </summary>
    public int PropertyMappingLastIndex => _csvPropertyMappings.Count - 1;

    protected CsvMapping(ITypeConverterProvider typeConverterProvider)
    {
      this._typeConverterProvider = typeConverterProvider;
      this._invokeConstructor = ReflectionUtils.CreateDefaultConstructor<TEntity>();
      this._csvPropertyMappings = new List<IndexToPropertyMapping>();
    }

    protected CsvPropertyMapping<TEntity, TProperty> MapProperty<TProperty>(int columnIndex, Expression<Func<TEntity, TProperty>> property)
    {
      return MapProperty(columnIndex, property, _typeConverterProvider.Resolve<TProperty>());
    }

    protected CsvPropertyMapping<TEntity, TProperty> MapProperty<TProperty>(int columnIndex, Expression<Func<TEntity, TProperty>> property, ITypeConverter<TProperty> typeConverter)
    {
      if (_csvPropertyMappings.Any(x => x.ColumnIndex == columnIndex))
      {
        throw new InvalidOperationException(string.Format("Duplicate mapping for column index {0}", columnIndex));
      }

      var propertyMapping = new CsvPropertyMapping<TEntity, TProperty>(property, typeConverter);

      AddPropertyMapping(columnIndex, propertyMapping);

      return propertyMapping;
    }

    private void AddPropertyMapping<TProperty>(int columnIndex, CsvPropertyMapping<TEntity, TProperty> propertyMapping)
    {
      var indexToPropertyMapping = new IndexToPropertyMapping
      {
        ColumnIndex = columnIndex,
        PropertyMapping = propertyMapping
      };

      _csvPropertyMappings.Add(indexToPropertyMapping);
    }

    public CsvMappingResult<TEntity> Map(string[] values)
    {
      TEntity entity = _invokeConstructor();

      for (int pos = 0; pos < _csvPropertyMappings.Count; pos++)
      {
        var indexToPropertyMapping = _csvPropertyMappings[pos];

        var columnIndex = indexToPropertyMapping.ColumnIndex;

        if (columnIndex >= values.Length)
        {
          return new CsvMappingResult<TEntity>
          {
            Error = new CsvMappingError
            {
              ColumnIndex = columnIndex,
              Value = string.Format("Column {0} Out Of Range", columnIndex)
            }
          };
        }

        var value = values[columnIndex];

        if (!indexToPropertyMapping.PropertyMapping.TryMapValue(entity, value))
        {
          return new CsvMappingResult<TEntity>
          {
            Error = new CsvMappingError
            {
              ColumnIndex = columnIndex,
              Value = value,
              ColumnMapDetails = indexToPropertyMapping.PropertyMapping.ToString()
            }
          };
        }
      }

      return new CsvMappingResult<TEntity>
      {
        Result = entity
      };
    }


    public override string ToString()
    {
      var csvPropertyMappingsString = string.Join(", ", _csvPropertyMappings.Select(x => x.ToString()));

      return string.Format("CsvMapping (TypeConverterProvider = {0}, Mappings = {1})", _typeConverterProvider, csvPropertyMappingsString);
    }
  }
}
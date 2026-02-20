// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using TinyCsvParser.Core;
using TinyCsvParser.Models;
using TinyCsvParser.TypeConverters;

namespace TinyCsvParser;

/// <summary>
/// Base class for CSV mapping definitions.
/// </summary>
public abstract class CsvMapping<TEntity> : ICsvMapping<TEntity>, IHeaderBinder
    where TEntity : class, new()
{
    private readonly ITypeConverterProvider _typeConverterProvider;
    private readonly List<PropertyMapping> _propertyMappings = new();

    protected CsvMapping() : this(new TypeConverterProvider()) { }

    protected CsvMapping(ITypeConverterProvider typeConverterProvider)
    {
        _typeConverterProvider = typeConverterProvider;
    }

    public bool NeedsHeaderResolution => _propertyMappings.Any(m => m.ColumnIndex == -1);

    public void BindHeaders(ref CsvRow headerRow)
    {
        var headerMap = new Dictionary<string, int>(headerRow.Count, StringComparer.OrdinalIgnoreCase);
        for (int i = 0; i < headerRow.Count; i++)
        {
            headerMap[headerRow.GetString(i)] = i;
        }

        foreach (var mapping in _propertyMappings)
        {
            if (mapping.ColumnIndex == -1 && mapping.ColumnName != null)
            {
                if (headerMap.TryGetValue(mapping.ColumnName, out int index))
                {
                    mapping.ColumnIndex = index;
                }
                else
                {
                    throw new InvalidOperationException($"The column '{mapping.ColumnName}' was not found in the CSV header.");
                }
            }
        }
    }

    public CsvMappingResult<TEntity> Map(ref CsvRow row)
    {
        if (NeedsHeaderResolution)
        {
            throw new InvalidOperationException("Mapping contains unresolved header names. Call BindHeaders() first.");
        }

        var entity = new TEntity();
        foreach (var mapping in _propertyMappings)
        {
            if (mapping.ColumnIndex < 0 || mapping.ColumnIndex >= row.Count)
            {
                return new CsvMappingError { ColumnIndex = mapping.ColumnIndex, Value = "Index Out Of Range" };
            }

            if (!mapping.TryMap(ref row, entity))
            {
                return new CsvMappingError { ColumnIndex = mapping.ColumnIndex, Value = $"Conversion failed: {row.GetString(mapping.ColumnIndex)}" };
            }
        }
        return entity;
    }

    /// <summary>
    /// Maps a property by its column index using the default converter.
    /// </summary>
    public void MapProperty<TProperty>(int columnIndex, Expression<Func<TEntity, TProperty>> property)
    {
        MapProperty(columnIndex, property, _typeConverterProvider.Resolve<TProperty>());
    }

    /// <summary>
    /// Maps a property by its column index using a specific converter.
    /// </summary>
    public void MapProperty<TProperty>(int columnIndex, Expression<Func<TEntity, TProperty>> property, ITypeConverter<TProperty> converter)
    {
        var setter = CreateSetter(property);
        _propertyMappings.Add(new PropertyMapping<TProperty>(columnIndex, null, converter, setter));
    }

    /// <summary>
    /// Maps a property by its header column name using the default converter.
    /// </summary>
    public void MapProperty<TProperty>(string columnName, Expression<Func<TEntity, TProperty>> property)
    {
        MapProperty(columnName, property, _typeConverterProvider.Resolve<TProperty>());
    }

    /// <summary>
    /// Maps a property by its header column name using a specific converter.
    /// </summary>
    public void MapProperty<TProperty>(string columnName, Expression<Func<TEntity, TProperty>> property, ITypeConverter<TProperty> converter)
    {
        var setter = CreateSetter(property);
        _propertyMappings.Add(new PropertyMapping<TProperty>(-1, columnName, converter, setter));
    }

    private abstract class PropertyMapping
    {
        public int ColumnIndex;
        public string? ColumnName;
        public abstract bool TryMap(ref CsvRow row, TEntity entity);
    }

    private class PropertyMapping<TProperty> : PropertyMapping
    {
        private readonly ITypeConverter<TProperty> _converter;
        private readonly Action<TEntity, TProperty> _setter;

        public PropertyMapping(int index, string? name, ITypeConverter<TProperty> converter, Action<TEntity, TProperty> setter)
        {
            ColumnIndex = index;
            ColumnName = name;
            _converter = converter;
            _setter = setter;
        }

        public override bool TryMap(ref CsvRow row, TEntity entity)
        {
            var span = row.GetSpan(ColumnIndex);
            if (_converter.TryConvert(span, out TProperty? value))
            {
                _setter(entity, value!);
                return true;
            }
            return false;
        }
    }

    private static Action<TEntity, TProperty> CreateSetter<TProperty>(Expression<Func<TEntity, TProperty>> property)
    {
        ParameterExpression valueParam = Expression.Parameter(typeof(TProperty), "value");
        BinaryExpression assign = Expression.Assign(property.Body, valueParam);
        return Expression.Lambda<Action<TEntity, TProperty>>(assign, property.Parameters[0], valueParam).Compile();
    }
}
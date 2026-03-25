using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using TinyCsvParser.Core;
using TinyCsvParser.Models;
using TinyCsvParser.TypeConverters;

namespace TinyCsvParser;

public abstract class CsvMapping<TEntity> : ICsvMapping<TEntity>, IHeaderBinder
    where TEntity : class, new()
{
    private readonly ITypeConverterProvider _typeConverterProvider;
    private readonly List<PropertyMapping> _propertyMappings = [];
    
    private MapUsingFunc<TEntity>? _mapUsingFunc;

    protected CsvMapping() : this(new TypeConverterProvider())
    {
    }

    protected CsvMapping(ITypeConverterProvider typeConverterProvider)
    {
        _typeConverterProvider = typeConverterProvider;
    }

    public virtual bool NeedsHeaderResolution => _propertyMappings.Any(m => m.ColumnIndex == -1);

    public virtual CsvHeaderResolution BindHeaders(ref CsvRow headerRow)
    {
        Dictionary<string, int> headerMap = new(headerRow.Count, StringComparer.OrdinalIgnoreCase);

        for (int i = 0; i < headerRow.Count; i++)
        {
            headerMap[headerRow.GetString(i)] = i;
        }

        int[] resolvedIndices = new int[_propertyMappings.Count];

        for (int i = 0; i < _propertyMappings.Count; i++)
        {
            PropertyMapping mapping = _propertyMappings[i];

            if (mapping is { ColumnIndex: -1, ColumnName: not null })
            {
                if (headerMap.TryGetValue(mapping.ColumnName, out int index))
                {
                    resolvedIndices[i] = index;
                }
                else
                {
                    throw new InvalidOperationException($"Column '{mapping.ColumnName}' not found in header.");
                }
            }
            else
            {
                resolvedIndices[i] = mapping.ColumnIndex;
            }
        }

        return new CsvHeaderResolution(resolvedIndices);
    }

    public CsvMappingResult<TEntity> Map(ref CsvRow row, CsvHeaderResolution? headerResolution = null)
    {
        if (NeedsHeaderResolution && !headerResolution.HasValue)
        {
            throw new InvalidOperationException("Unresolved headers. Call BindHeaders first.");
        }

        TEntity entity = new();

        int[]? indices = headerResolution?.ResolvedIndices;

        for (int i = 0; i < _propertyMappings.Count; i++)
        {
            PropertyMapping mapping = _propertyMappings[i];

            int actualIndex = indices != null ? indices[i] : mapping.ColumnIndex;

            if (actualIndex < 0 || actualIndex >= row.Count)
            {
                return CreateError(row, actualIndex, "Index Out Of Range");
            }

            if (!mapping.TryMap(ref row, entity, actualIndex))
            {
                return CreateError(row, actualIndex, $"Conversion failed: {row.GetString(actualIndex)}");
            }
        }

        if (_mapUsingFunc != null)
        {
            MapUsingResult mapResult = _mapUsingFunc(entity, ref row);

            int recordIndex = row.RecordIndex;
            int lineNumber = row.LineNumber;

            return mapResult.Match(
                onSuccess: () => new CsvMappingResult<TEntity>(entity, recordIndex, lineNumber),
                onFailure: (errorMsg) => new CsvMappingResult<TEntity>(
                    new CsvMappingError(recordIndex, lineNumber, -1, errorMsg),
                    recordIndex, lineNumber)
            );
        }

        return new CsvMappingResult<TEntity>(entity, row.RecordIndex, row.LineNumber);
    }

    private CsvMappingResult<TEntity> CreateError(CsvRow row, int col, string msg) =>
        new(new CsvMappingError(row.RecordIndex, row.LineNumber, col, msg), row.RecordIndex, row.LineNumber);

    public void MapUsing(MapUsingFunc<TEntity> mapUsingFunc) => _mapUsingFunc = mapUsingFunc;

    public void MapProperty<TProperty>(int columnIndex, Expression<Func<TEntity, TProperty>> property, ITypeConverter<TProperty>? converter = null) =>
        _propertyMappings.Add(new PropertyMapping<TProperty>(columnIndex, null, converter ?? _typeConverterProvider.Resolve<TProperty>(), CreateSetter(property)));

    public void MapProperty<TProperty>(string columnName, Expression<Func<TEntity, TProperty>> property, ITypeConverter<TProperty>? converter = null) =>
        _propertyMappings.Add(new PropertyMapping<TProperty>(-1, columnName, converter ?? _typeConverterProvider.Resolve<TProperty>(), CreateSetter(property)));

    private abstract class PropertyMapping
    {
        public int ColumnIndex;

        public string? ColumnName;

        public abstract bool TryMap(ref CsvRow row, TEntity entity, int actualIndex);
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

        public override bool TryMap(ref CsvRow row, TEntity entity, int actualIndex)
        {
            if (_converter.TryConvert(row.GetSpan(actualIndex), out TProperty? value))
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

        return Expression.Lambda<Action<TEntity, TProperty>>(Expression.Assign(property.Body, valueParam), property.Parameters[0], valueParam).Compile();
    }
}
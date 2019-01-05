// Copyright (c) Philipp Wagner and Joel Mueller. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using CoreCsvParser.Tokenizer;
using CoreCsvParser.TypeConverter;

namespace CoreCsvParser.Mapping
{
    public abstract class CsvMapping<TEntity> where TEntity : new()
    {
        private readonly ITypeConverterProvider _typeConverterProvider;
        private readonly Dictionary<int, ICsvPropertyMapping<TEntity>> _csvPropertyMappings;
        private int _maxMapped;

        protected CsvMapping() : this(new TypeConverterProvider())
        {
        }

        protected CsvMapping(ITypeConverterProvider typeConverterProvider)
        {
            _typeConverterProvider = typeConverterProvider;
            _csvPropertyMappings = new Dictionary<int, ICsvPropertyMapping<TEntity>>();
        }

        protected CsvPropertyMapping<TEntity, TProperty> MapProperty<TProperty>(int columnIndex, Expression<Func<TEntity, TProperty>> property)
        {
            return MapProperty(columnIndex, property, _typeConverterProvider.Resolve<TProperty>());
        }

        protected CsvPropertyMapping<TEntity, TProperty> MapProperty<TProperty>(int columnIndex, Expression<Func<TEntity, TProperty>> property, ITypeConverter<TProperty> typeConverter)
        {
            if (_csvPropertyMappings.ContainsKey(columnIndex))
            {
                throw new InvalidOperationException($"Duplicate mapping for column index {columnIndex}.");
            }

            var propertyMapping = new CsvPropertyMapping<TEntity, TProperty>(property, typeConverter);

            _csvPropertyMappings.Add(columnIndex, propertyMapping);
            _maxMapped = _csvPropertyMappings.Keys.Max();

            return propertyMapping;
        }

        public CsvMappingResult<TEntity> Map(TokenEnumerable tokens, int rowIndex)
        {
            TEntity entity = new TEntity();

            int mappedCols = 0;
            int colIndex = 0;

            foreach (var token in tokens)
            {
                if (_csvPropertyMappings.TryGetValue(colIndex, out var mapping))
                {
                    if (mapping.TryMapValue(entity, token))
                    {
                        mappedCols++;
                    }
                    else
                    {
                        return new CsvMappingResult<TEntity>(rowIndex, colIndex, 
                            $"Column {colIndex} with Value '{token.ToString()}' cannot be converted.");
                    }
                }

                colIndex++;
                if (colIndex > _maxMapped)
                    break;
            }

            if (mappedCols == 0)
            {
                return new CsvMappingResult<TEntity>(rowIndex, colIndex,
                            $"No columns were mapped for {_csvPropertyMappings.Count} mappings, {colIndex} columns in row {rowIndex}.");
            }

            return new CsvMappingResult<TEntity>(rowIndex, entity);
        }

        public override string ToString()
        {
            var csvPropertyMappingsString = string.Join(", ", _csvPropertyMappings.Select(x => x.ToString()));

            return $"CsvMapping (TypeConverterProvider = {_typeConverterProvider}, Mappings = {csvPropertyMappingsString})";
        }
    }
}
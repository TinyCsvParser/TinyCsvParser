// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using TinyCsvParser.Tokenizer;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.Mapping
{
    public abstract class CsvMapping<TEntity> where TEntity : new()
    {
        private class IndexToPropertyMapping
        {
            public int ColumnIndex { get; set; }

            public ICsvPropertyMapping<TEntity> PropertyMapping { get; set; }

            public void Deconstruct(out int colIndex, out ICsvPropertyMapping<TEntity> mapping)
            {
                colIndex = ColumnIndex;
                mapping = PropertyMapping;
            }

            public override string ToString()
            {
                return $"IndexToPropertyMapping (ColumnIndex = {ColumnIndex}, PropertyMapping = {PropertyMapping}";
            }
        }

        private readonly ITypeConverterProvider _typeConverterProvider;
        private readonly Dictionary<int, ICsvPropertyMapping<TEntity>> _csvPropertyMappings;

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

            return propertyMapping;
        }

        public CsvMappingResult<TEntity> Map(TokenEnumerable tokens, int rowIndex)
        {
            TEntity entity = new TEntity();

            int colIndex = 0;
            foreach (var token in tokens)
            {
                if (_csvPropertyMappings.TryGetValue(colIndex, out var mapping))
                {
                    if (!mapping.TryMapValue(entity, token))
                    {
                        return new CsvMappingResult<TEntity>(rowIndex, colIndex, 
                            $"Column {colIndex} with Value '{token.ToString()}' cannot be converted.");
                    }
                }
                else
                {
                    return new CsvMappingResult<TEntity>(rowIndex, colIndex, $"No mapping found for column {colIndex}.");
                }

                colIndex++;
            }

            if (colIndex < _csvPropertyMappings.Count)
            {
                return new CsvMappingResult<TEntity>(rowIndex, colIndex,
                    $"Expected {_csvPropertyMappings.Count} columns, but found {colIndex} columns.");
            }

            return new CsvMappingResult<TEntity>(rowIndex, entity);
        }

        public override string ToString()
        {
            var csvPropertyMappingsString = string.Join(", ", _csvPropertyMappings.Select(x => x.ToString()));

            return string.Format("CsvMapping (TypeConverterProvider = {0}, Mappings = {1})", _typeConverterProvider, csvPropertyMappingsString);
        }
    }
}
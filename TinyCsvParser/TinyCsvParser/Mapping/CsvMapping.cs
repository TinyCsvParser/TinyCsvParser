// Copyright (c) Philipp Wagner. All rights reserved.
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

        private readonly Func<TEntity> invokeConstructor;
        private readonly ITypeConverterProvider typeConverterProvider;
        private readonly List<IndexToPropertyMapping> csvPropertyMappings;

        protected CsvMapping()
            : this(new TypeConverterProvider())
        {
        }

        protected CsvMapping(ITypeConverterProvider typeConverterProvider)
        {
            this.typeConverterProvider = typeConverterProvider;
            this.invokeConstructor = ReflectionUtils.CreateDefaultConstructor<TEntity>();
            this.csvPropertyMappings = new List<IndexToPropertyMapping>();
        }

        protected CsvPropertyMapping<TEntity, TProperty> MapProperty<TProperty>(int columnIndex, Expression<Func<TEntity, TProperty>> property)
        {
            if (csvPropertyMappings.Any(x => x.ColumnIndex == columnIndex))
            {
                throw new InvalidOperationException(string.Format("Duplicate mapping for column index {0}"));
            }

            var propertyMapping = new CsvPropertyMapping<TEntity, TProperty>(property, typeConverterProvider);

            var indexToPropertyMapping = new IndexToPropertyMapping
            {
                ColumnIndex = columnIndex,
                PropertyMapping = propertyMapping
            };

            csvPropertyMappings.Add(indexToPropertyMapping);

            return propertyMapping;
        }

        public CsvMappingResult<TEntity> Map(string[] values)
        {
            TEntity entity = invokeConstructor();

            for (int pos = 0; pos < csvPropertyMappings.Count; pos++)
            {
                var indexToPropertyMapping = csvPropertyMappings[pos];

                var columnIndex = indexToPropertyMapping.ColumnIndex;

                if (columnIndex >= values.Length)
                {
                    throw new ArgumentOutOfRangeException(string.Format("No column with index {0} exists", columnIndex));
                }

                var value = values[columnIndex];

                if (!indexToPropertyMapping.PropertyMapping.TryMapValue(entity, value))
                {
                    return new CsvMappingResult<TEntity>()
                    {
                        Error = new CsvMappingError
                        {
                            ColumnIndex = columnIndex,
                            Value = value
                        }
                    };
                }
            }

            return new CsvMappingResult<TEntity>()
            {
                Result = entity
            };
        }

        
        public override string ToString()
        {
            var csvPropertyMappingsString =  string.Join(", ", csvPropertyMappings.Select(x => x.ToString()));

            return string.Format("CsvMapping (TypeConverterProvider = {0}, Mappings = {1})", typeConverterProvider, csvPropertyMappingsString);
        }
    }
}
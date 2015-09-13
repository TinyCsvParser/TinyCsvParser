// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.Mapping
{
    public abstract class CsvMapping<TEntity>
        where TEntity : class, new()
    {
        private IDictionary<int, ICsvPropertyMapping<TEntity>> csvPropertyMappings = new Dictionary<int, ICsvPropertyMapping<TEntity>>();

        private readonly ITypeConverterProvider typeConverterProvider;

        protected CsvMapping()
            : this(new TypeConverterProvider())
        {
        }

        protected CsvMapping(ITypeConverterProvider typeConverterProvider)
        {
            this.typeConverterProvider = typeConverterProvider;
        }

        protected void MapProperty<TProperty>(int columnIndex, Expression<Func<TEntity, TProperty>> property)
        {
            if (csvPropertyMappings.ContainsKey(columnIndex))
            {
                throw new InvalidOperationException(string.Format("Duplicate mapping for column index {0}"));
            }
            
            csvPropertyMappings[columnIndex] = new CsvPropertyMapping<TEntity, TProperty>(property, typeConverterProvider);
        }

        public CsvMappingResult<TEntity> Map(string[] values)
        {
            try
            {
                var entity = InternalMap(values);

                return new CsvMappingResult<TEntity>()
                {
                    Result = entity
                };
            }
            catch (Exception e)
            {
                return new CsvMappingResult<TEntity>()
                {
                    Error = new CsvMappingError()
                    {
                        Exception = e
                    }
                };
            }
        }

        private TEntity InternalMap(string[] values)
        {
            TEntity entity = (TEntity)Activator.CreateInstance(typeof(TEntity));

            foreach (var kv in csvPropertyMappings)
            {
                int columnIndex = kv.Key;
                
                if (columnIndex >= values.Length)
                {
                    throw new ArgumentOutOfRangeException(string.Format("No column with index {0} exists", columnIndex));
                }

                ICsvPropertyMapping<TEntity> csvPropertyMapping = kv.Value;

                var value = values[columnIndex];

                csvPropertyMapping.MapValue(entity, value);
            }

            return entity;
        }
        
        public override string ToString()
        {
            var csvPropertyMappingsString =  string.Join(", ", csvPropertyMappings.Select(x => string.Format("(Index = {0}, Mapping = {1})", x.Key, x.Value)));

            return string.Format("CsvMapping (TypeConverterProvider = {0}, Mappings = {1})", 
                typeConverterProvider,
                csvPropertyMappingsString);
        }
    }
}
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using TinyCsvParser.TypeConverter;
using TinyCsvParser.Model;
using TinyCsvParser.Ranges;

namespace TinyCsvParser.Mapping
{
    public abstract class CsvMapping<TEntity> : ICsvMapping<TEntity>
        where TEntity : class, new()
    {
        private class IndexToPropertyMapping
        {
            public int ColumnIndex { get; set; }

            public ICsvPropertyMapping<TEntity, string> PropertyMapping { get; set; }

            public override string ToString()
            {
                return $"IndexToPropertyMapping (ColumnIndex = {ColumnIndex}, PropertyMapping = {PropertyMapping}";
            }
        }

        private class RangeToPropertyMapping
        {
            public RangeDefinition Range { get; set; }

            public ICsvPropertyMapping<TEntity, string[]> PropertyMapping { get; set; }

            public override string ToString()
            {
                return $"IndexToPropertyMapping (Range = {Range}, PropertyMapping = {PropertyMapping}";
            }
        }
        

        private readonly ITypeConverterProvider typeConverterProvider;
        private readonly List<IndexToPropertyMapping> csvIndexPropertyMappings;
        private readonly List<RangeToPropertyMapping> csvRangePropertyMappings;
        private readonly List<CsvRowMapping<TEntity>> csvRowMappings;

        protected CsvMapping()
            : this(new TypeConverterProvider())
        {
        }

        protected CsvMapping(ITypeConverterProvider typeConverterProvider)
        {
            this.typeConverterProvider = typeConverterProvider;
            this.csvIndexPropertyMappings = new List<IndexToPropertyMapping>();
            this.csvRangePropertyMappings = new List<RangeToPropertyMapping>();
            this.csvRowMappings = new List<CsvRowMapping<TEntity>>();
        }

        protected CsvRowMapping<TEntity> MapUsing(Func<TEntity, TokenizedRow, bool> action)
        {
            var rowMapping = new CsvRowMapping<TEntity>(action);

            csvRowMappings.Add(rowMapping);

            return rowMapping;
        }

        protected CsvPropertyMapping<TEntity, TProperty> MapProperty<TProperty>(int columnIndex, Expression<Func<TEntity, TProperty>> property)
        {
            return MapProperty(columnIndex, property, typeConverterProvider.Resolve<TProperty>());
        }

        protected CsvCollectionPropertyMapping<TEntity, TProperty> MapProperty<TProperty>(RangeDefinition range, Expression<Func<TEntity, TProperty>> property)
        {
            return MapProperty(range, property, typeConverterProvider.ResolveCollection<TProperty>());
        }

        protected CsvCollectionPropertyMapping<TEntity, TProperty> MapProperty<TProperty>(RangeDefinition range, Expression<Func<TEntity, TProperty>> property, IArrayTypeConverter<TProperty> typeConverter)
        {
            var propertyMapping = new CsvCollectionPropertyMapping<TEntity, TProperty>(property, typeConverter);

            AddPropertyMapping(range, propertyMapping);

            return propertyMapping;
        }

        protected CsvPropertyMapping<TEntity, TProperty> MapProperty<TProperty>(int columnIndex, Expression<Func<TEntity, TProperty>> property, ITypeConverter<TProperty> typeConverter)
        {
            if (csvIndexPropertyMappings.Any(x => x.ColumnIndex == columnIndex))
            {
                throw new InvalidOperationException($"Duplicate mapping for column index {columnIndex}");
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

            csvIndexPropertyMappings.Add(indexToPropertyMapping);
        }

        private void AddPropertyMapping<TProperty>(RangeDefinition range, CsvCollectionPropertyMapping<TEntity, TProperty> propertyMapping)
        {
            var rangeToPropertyMapping = new RangeToPropertyMapping
            {
                Range = range,
                PropertyMapping = propertyMapping
            };

            csvRangePropertyMappings.Add(rangeToPropertyMapping);
        }

        public CsvMappingResult<TEntity> Map(TokenizedRow values)
        {
            TEntity entity = new TEntity();

            // Iterate over Index Mappings:
            for (int pos = 0; pos < csvIndexPropertyMappings.Count; pos++)
            {
                var indexToPropertyMapping = csvIndexPropertyMappings[pos];

                var columnIndex = indexToPropertyMapping.ColumnIndex;

                if (columnIndex >= values.Tokens.Length)
                {
                    return new CsvMappingResult<TEntity>
                    {
                        RowIndex = values.Index,
                        Error = new CsvMappingError
                        {
                            ColumnIndex = columnIndex,
                            Value = $"Column {columnIndex} is Out Of Range",
                            UnmappedRow = string.Join("|", values.Tokens)
                        }
                    };
                }

                var value = values.Tokens[columnIndex];

                if (!indexToPropertyMapping.PropertyMapping.TryMapValue(entity, value))
                {
                    return new CsvMappingResult<TEntity>
                    {
                        RowIndex = values.Index,
                        Error = new CsvMappingError
                        {
                            ColumnIndex = columnIndex,
                            Value = $"Column {columnIndex} with Value '{value}' cannot be converted",
                            UnmappedRow = string.Join("|", values.Tokens)
                        }
                    };
                }
            }

            // Iterate over Range Mappings:
            for (int pos = 0; pos < csvRangePropertyMappings.Count; pos++)
            {
                var rangeToPropertyMapping = csvRangePropertyMappings[pos];

                var range = rangeToPropertyMapping.Range;

                // Copy the Sub Array. This needs optimization, like ReadOnlyMemory!
                var slice = values.Tokens.Skip(range.Start).Take(range.Length).ToArray();

                if (!rangeToPropertyMapping.PropertyMapping.TryMapValue(entity, slice))
                {
                    var columnIndex = range.Start;

                    return new CsvMappingResult<TEntity>
                    {
                        RowIndex = values.Index,
                        Error = new CsvMappingError
                        {
                            ColumnIndex = columnIndex,
                            Value = $"Range with Start Index {range.Start} and End Index {range.End} cannot be converted!",
                            UnmappedRow = string.Join("|", values.Tokens)
                        }
                    };
                }
            }

            // Iterate over Row Mappings. At this point previous values for the entity 
            // should be set:
            for(int pos = 0; pos < csvRowMappings.Count; pos++)
            {
                var csvRowMapping = csvRowMappings[pos];

                if(!csvRowMapping.TryMapValue(entity, values))
                {
                    return new CsvMappingResult<TEntity>
                    {
                        RowIndex = values.Index,
                        Error = new CsvMappingError
                        {
                            Value = $"Row could not be mapped!",
                            UnmappedRow = string.Join("|", values.Tokens)
                        }
                    };
                }
            }

            return new CsvMappingResult<TEntity>
            {
                RowIndex = values.Index,
                Result = entity
            };
        }
        
        public override string ToString()
        {
            var csvPropertyMappingsString =  string.Join(", ", csvIndexPropertyMappings.Select(x => x.ToString()));

            return $"CsvMapping (TypeConverterProvider = {typeConverterProvider}, Mappings = {csvPropertyMappingsString})";
        }
    }
}
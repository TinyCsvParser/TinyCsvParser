// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using TinyCsvParser.Mapping;
using TinyCsvParser.Model;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.Test.Mapping
{
    public class CsvPermissiveMapping<TEntity> : ICsvMapping<TEntity>
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

        private readonly ITypeConverterProvider typeConverterProvider;
        private readonly List<IndexToPropertyMapping> csvIndexPropertyMappings;

        protected CsvPermissiveMapping()
            : this(new TypeConverterProvider())
        {
        }

        protected CsvPermissiveMapping(ITypeConverterProvider typeConverterProvider)
        {
            this.typeConverterProvider = typeConverterProvider;
            this.csvIndexPropertyMappings = new List<IndexToPropertyMapping>();
        }

        protected CsvPropertyMapping<TEntity, TProperty> MapProperty<TProperty>(int columnIndex, Expression<Func<TEntity, TProperty>> property)
        {
            return MapProperty(columnIndex, property, typeConverterProvider.Resolve<TProperty>());
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

        public CsvMappingResult<TEntity> Map(TokenizedRow values, int ignoreColumns = 0)
        {
            TEntity entity = new TEntity();

            // Iterate over Index Mappings:
            for (int pos = 0; pos < csvIndexPropertyMappings.Count; pos++)
            {
                var indexToPropertyMapping = csvIndexPropertyMappings[pos];

                var columnIndex = indexToPropertyMapping.ColumnIndex;

                if (columnIndex >= values.Tokens.Length)
                {
                    continue;
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

            return new CsvMappingResult<TEntity>
            {
                RowIndex = values.Index,
                Result = entity
            };
        }

        public override string ToString()
        {
            var csvPropertyMappingsString = string.Join(", ", csvIndexPropertyMappings.Select(x => x.ToString()));

            return $"CsvMissingValuesMapping (TypeConverterProvider = {typeConverterProvider}, Mappings = {csvPropertyMappingsString})";
        }

        public CsvHeaderMappingResult MapHeader(TokenizedRow values)
        {
            throw new NotImplementedException();
        }

        public Dictionary<int, string> GetPropertyMapping()
        {
            throw new NotImplementedException();
        }
    }

    [TestFixture]
    public class CsvPermissiveMappingTest
    {
        private class SampleEntity
        {
            public string Value1 { get; set; }

            public string Value2 { get; set; }

            public string Value3 { get; set; }
        }

        private class PermissiveSampleEntityMapper : CsvPermissiveMapping<SampleEntity>
        {
            public PermissiveSampleEntityMapper()
            {
                MapProperty(0, x => x.Value1);
                MapProperty(1, x => x.Value2);
                MapProperty(2, x => x.Value3);
            }
        }

        [Test]
        public void ExecuteWithMissingValuesTest()
        {
            CsvParserOptions options = new CsvParserOptions(true, ';');
            CsvParser<SampleEntity> customCsvParser = new CsvParser<SampleEntity>(options, new PermissiveSampleEntityMapper());

            var stringBuilder = new StringBuilder()
                .AppendLine("FirstName;LastName;BirthDate")
                .AppendLine("1;2;3")
                .AppendLine("4");

            var csvReaderOptions = new CsvReaderOptions(new[] { Environment.NewLine });

            var result = customCsvParser
                .ReadFromString(csvReaderOptions, stringBuilder.ToString())
                .Items
                .ToList();

            Assert.AreEqual(2, result.Count);

            Assert.IsTrue(result.All(x => x.IsValid));

            Assert.AreEqual("1", result[0].Result.Value1);
            Assert.AreEqual("2", result[0].Result.Value2);
            Assert.AreEqual("3", result[0].Result.Value3);

            Assert.AreEqual("4", result[1].Result.Value1);
            Assert.AreEqual(null, result[1].Result.Value2);
            Assert.AreEqual(null, result[1].Result.Value3);
        }
    }
}

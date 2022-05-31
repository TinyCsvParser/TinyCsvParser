// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using System.Linq;
using System.Text;
using TinyCsvParser.Mapping;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.Test.CsvParser
{
    [TestFixture]
    public class CustomConverterTest
    {
        private class Entity
        {
            public double? Property { get; set; }
        }

        private class NullTypeConverter : BaseConverter<double?>
        {
            private readonly ITypeConverter<double?> converter;

            public NullTypeConverter()
                : this(new NullableDoubleConverter())
            {
            }

            public NullTypeConverter(ITypeConverter<double?> converter)
            {
                this.converter = converter;
            }

            public override bool TryConvert(string value, out double? result)
            {
                result = default(double?);

                if (string.Equals("NULL", value))
                {
                    return true;
                }

                return converter.TryConvert(value, out result);
            }
        }

        private class CsvEntityMapping : CsvMapping<Entity>
        {
            public CsvEntityMapping()
            {
                MapProperty(0, x => x.Property, new NullTypeConverter());
            }
        }


        [Test]
        public void ParseWithNULLStringTest()
        {
            CsvParserOptions csvParserOptions = new CsvParserOptions(false,  ';');
            CsvReaderOptions csvReaderOptions = new CsvReaderOptions(new[] { Environment.NewLine });
            CsvEntityMapping csvMapper = new CsvEntityMapping();
            CsvParser<Entity> csvParser = new CsvParser<Entity>(csvParserOptions, csvMapper);

            var stringBuilder = new StringBuilder()
                .AppendLine("NULL")
                .AppendLine("1.0")
                .AppendLine("NULL")
                .AppendLine("2.0");

            var result = csvParser
                .ReadFromString(csvReaderOptions, stringBuilder.ToString())
                .ToList();

            Assert.AreEqual(null, result[0].Result.Property);
            Assert.That(result[1].Result.Property, Is.EqualTo(1.0).Within(1e-5));
            Assert.AreEqual(null, result[2].Result.Property);
            Assert.That(result[3].Result.Property, Is.EqualTo(2.0).Within(1e-5));
        }
    }
}

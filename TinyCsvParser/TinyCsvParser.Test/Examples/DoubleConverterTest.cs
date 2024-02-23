// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using System.Globalization;
using System.Linq;
using System.Text;
using TinyCsvParser.Mapping;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.Test.CsvParser
{
    [TestFixture]
    public class DoubleConverterTest
    {
        private class Entity
        {
            public double? Value { get; set; }
        }

        private class CsvEntityMapping : CsvMapping<Entity>
        {
            public CsvEntityMapping()
            {
                MapProperty(0, x => x.Value, new NullableDoubleConverter(new CultureInfo("de-DE")));
            }
        }

        [Test]
        public void ParseWithNULLStringTest()
        {
            CsvParserOptions csvParserOptions = new CsvParserOptions(false, ';');
            CsvReaderOptions csvReaderOptions = new CsvReaderOptions(new[] { Environment.NewLine });
            CsvEntityMapping csvMapper = new CsvEntityMapping();
            CsvParser<Entity> csvParser = new CsvParser<Entity>(csvParserOptions, csvMapper);

            var stringBuilder = new StringBuilder()
                .AppendLine("123,456");

            var result = csvParser
                .ReadFromString(csvReaderOptions, stringBuilder.ToString())
                .Items
                .ToList();

            Assert.AreEqual(123.456, result[0].Result.Value);
        }
    }
}

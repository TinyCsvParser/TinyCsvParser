// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using System.Linq;
using System.Text;
using NUnit.Framework;
using TinyCsvParser.Mapping;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.Test.Examples;

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
        var csvParserOptions = new CsvParserOptions(false,  ';');
        var csvReaderOptions = new CsvReaderOptions([Environment.NewLine]);
        var csvMapper = new CsvEntityMapping();
        var csvParser = new CsvParser<Entity>(csvParserOptions, csvMapper);

        var stringBuilder = new StringBuilder()
            .AppendLine("123,456");

        var result = csvParser
            .ReadFromString(csvReaderOptions, stringBuilder.ToString())
            .ToList();

        Assert.AreEqual(123.456, result[0].Result.Value);
    }
}
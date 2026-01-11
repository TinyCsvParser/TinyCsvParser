// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Text;
using NUnit.Framework;
using TinyCsvParser.Mapping;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.Test.Examples;

[TestFixture]
public class CustomConverterTest
{
    private class Entity
    {
        public double? Property { get; set; }
    }

    private class NullTypeConverter(ITypeConverter<double?> converter) : BaseConverter<double?>
    {
        public NullTypeConverter()
            : this(new NullableDoubleConverter())
        {
        }

        public override bool TryConvert(string value, out double? result)
        {
            result = null;

            return string.Equals("NULL", value) || converter.TryConvert(value, out result);
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
    public void ParseWithNullStringTest()
    {
        var csvParserOptions = new CsvParserOptions(false, ';');
        var csvReaderOptions = new CsvReaderOptions([Environment.NewLine]);
        var csvMapper = new CsvEntityMapping();
        var csvParser = new CsvParser<Entity>(csvParserOptions, csvMapper);

        var stringBuilder = new StringBuilder()
            .AppendLine("NULL")
            .AppendLine("1.0")
            .AppendLine("NULL")
            .AppendLine("2.0");

        var result = csvParser
            .ReadFromString(csvReaderOptions, stringBuilder.ToString())
            .ToList();

        Assert.AreEqual(null, result[0].Result.Property);
        NUnit.Framework.Assert.That(result[1].Result.Property, Is.EqualTo(1.0).Within(1e-5));
        Assert.AreEqual(null, result[2].Result.Property);
        NUnit.Framework.Assert.That(result[3].Result.Property, Is.EqualTo(2.0).Within(1e-5));
    }
}
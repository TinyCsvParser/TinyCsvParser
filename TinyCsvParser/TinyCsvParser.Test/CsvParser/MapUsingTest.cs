// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using System.Linq;
using System.Text;
using TinyCsvParser.Mapping;

namespace TinyCsvParser.Test.CsvParser;

[TestFixture]
public class CsvRowMappingTest
{
    private class MainClass
    {
        public string Property1 { get; set; }

        public SubClass SubClass { get; set; }
    }

    private class SubClass
    {
        public string Property2 { get; set; }

        public string Property3 { get; set; }
    }


    private class CsvMainClassMapping : CsvMapping<MainClass>
    {
        public CsvMainClassMapping()
        {
            MapProperty(0, x => x.Property1);
            MapUsing((entity, values) =>
            {
                // Example of invalidating the row based on its contents
                if (values.Tokens.Any(t => t == "Z"))
                {
                    return false;
                }

                var subClass = new SubClass
                {
                    Property2 = values.Tokens[1],
                    Property3 = values.Tokens[2]
                };

                entity.SubClass = subClass;

                return true;
            });
        }
    }

    [Test]
    public void MapUsingTest()
    {
        var csvParserOptions = new CsvParserOptions(false, ';');
        var csvReaderOptions = new CsvReaderOptions([Environment.NewLine]);
        var csvMapper = new CsvMainClassMapping();
        var csvParser = new CsvParser<MainClass>(csvParserOptions, csvMapper);

        var stringBuilder = new StringBuilder()
            .AppendLine("X;Y;Z")
            .AppendLine("A;B;C");

        var result = csvParser
            .ReadFromString(csvReaderOptions, stringBuilder.ToString())
            .ToList();

        Assert.AreEqual(2, result.Count);

        Assert.IsFalse(result[0].IsValid);
        Assert.IsTrue(result[1].IsValid);

        Assert.AreEqual("A", result[1].Result.Property1);

        Assert.IsNotNull(result[1].Result.SubClass);

        Assert.AreEqual("B", result[1].Result.SubClass.Property2);
        Assert.AreEqual("C", result[1].Result.SubClass.Property3);
    }
}
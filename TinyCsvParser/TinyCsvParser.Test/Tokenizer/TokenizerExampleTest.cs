// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using System.Linq;
using System.Text;
using TinyCsvParser.Mapping;
using TinyCsvParser.Tokenizer;

namespace TinyCsvParser.Test.Tokenizer;

[TestFixture]
public class TokenizerExampleTest
{
    private class Person
    {
        public string FirstNameWithLastName { get; set; }
        public DateTime BirthDate { get; set; }
    }

    private class CsvPersonMapping : CsvMapping<Person>
    {
        public CsvPersonMapping()
        {
            MapProperty(0, x => x.FirstNameWithLastName);
            MapProperty(1, x => x.BirthDate);
        }
    }

    [Test]
    public void QuotedStringTokenizerExampleTest()
    {
        var csvParserOptions = new CsvParserOptions(true, string.Empty, new QuotedStringTokenizer(','));
        var csvReaderOptions = new CsvReaderOptions([Environment.NewLine]);
        var csvMapper = new CsvPersonMapping();
        var csvParser = new CsvParser<Person>(csvParserOptions, csvMapper);

        var stringBuilder = new StringBuilder()
            .AppendLine("FirstNameLastName;BirthDate")
            .AppendLine("\"Philipp,Wagner\",1986/05/12")
            .AppendLine("\"Max,Mustermann\",2014/01/01");

        var result = csvParser
            .ReadFromString(csvReaderOptions, stringBuilder.ToString())
            .ToList();

        // Make sure we got 2 results:
        Assert.AreEqual(2, result.Count);

        // And all of them have been parsed correctly:
        Assert.IsTrue(result.All(x => x.IsValid));

        // Now check the values:
        Assert.AreEqual("Philipp,Wagner", result[0].Result.FirstNameWithLastName);

        Assert.AreEqual(1986, result[0].Result.BirthDate.Year);
        Assert.AreEqual(5, result[0].Result.BirthDate.Month);
        Assert.AreEqual(12, result[0].Result.BirthDate.Day);

        Assert.AreEqual("Max,Mustermann", result[1].Result.FirstNameWithLastName);

        Assert.AreEqual(2014, result[1].Result.BirthDate.Year);
        Assert.AreEqual(1, result[1].Result.BirthDate.Month);
        Assert.AreEqual(1, result[1].Result.BirthDate.Day);
    }
}
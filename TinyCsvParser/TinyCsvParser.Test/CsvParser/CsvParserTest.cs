// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using System.Linq;
using System.Text;
using TinyCsvParser.Mapping;
using TinyCsvParser.Tokenizer;

namespace TinyCsvParser.Test.CsvParser;

[TestFixture]
public class CsvParserTest
{
    private class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
    }

    private class CsvPersonMapping : CsvMapping<Person>
    {
        public CsvPersonMapping()
        {
            MapProperty(0, x => x.FirstName);
            MapProperty(1, x => x.LastName);
            MapProperty(2, x => x.BirthDate);
        }
    }

    [Test]
    public void NullInputTest()
    {
        var csvParserOptions = new CsvParserOptions(true, ';');
        var csvMapper = new CsvPersonMapping();
        var csvParser = new CsvParser<Person>(csvParserOptions, csvMapper);

        NUnit.Framework.Assert.Throws<ArgumentNullException>(() => { csvParser.Parse(null); });
    }

    [Test]
    public void ToStringTest()
    {
        var csvParserOptions = new CsvParserOptions(true, ';');
        var csvMapper = new CsvPersonMapping();
        var csvParser = new CsvParser<Person>(csvParserOptions, csvMapper);

        var result = string.Empty;
        NUnit.Framework.Assert.DoesNotThrow(() => result = csvParser.ToString());
        Assert.IsNotEmpty(result);
    }

    [Test]
    public void SkipHeaderTest()
    {
        var csvParserOptions = new CsvParserOptions(true, ';');
        var csvReaderOptions = new CsvReaderOptions([Environment.NewLine]);
        var csvMapper = new CsvPersonMapping();
        var csvParser = new CsvParser<Person>(csvParserOptions, csvMapper);

        var stringBuilder = new StringBuilder()
            .AppendLine("FirstName;LastName;BirthDate")
            .AppendLine("Philipp;Wagner;1986/05/12")
            .AppendLine("Max;Mustermann;2014/01/01");

        var result = csvParser
            .ReadFromString(csvReaderOptions, stringBuilder.ToString())
            .ToList();

        Assert.AreEqual(2, result.Count);

        Assert.IsTrue(result.All(x => x.IsValid));

        Assert.AreEqual("Philipp", result[0].Result.FirstName);
        Assert.AreEqual("Wagner", result[0].Result.LastName);

        Assert.AreEqual(1, result[0].RowIndex);
        Assert.AreEqual(1986, result[0].Result.BirthDate.Year);
        Assert.AreEqual(5, result[0].Result.BirthDate.Month);
        Assert.AreEqual(12, result[0].Result.BirthDate.Day);

        Assert.AreEqual("Max", result[1].Result.FirstName);
        Assert.AreEqual("Mustermann", result[1].Result.LastName);

        Assert.AreEqual(2, result[1].RowIndex);
        Assert.AreEqual(2014, result[1].Result.BirthDate.Year);
        Assert.AreEqual(1, result[1].Result.BirthDate.Month);
        Assert.AreEqual(1, result[1].Result.BirthDate.Day);
    }

    [Test]
    public void DoNotSkipHeaderTest()
    {
        var csvParserOptions = new CsvParserOptions(false, ';');
        var csvReaderOptions = new CsvReaderOptions([Environment.NewLine]);
        var csvMapper = new CsvPersonMapping();
        var csvParser = new CsvParser<Person>(csvParserOptions, csvMapper);

        var stringBuilder = new StringBuilder()
            .AppendLine("Philipp;Wagner;1986/05/12")
            .AppendLine("Max;Mustermann;2014/01/01");

        var result = csvParser
            .ReadFromString(csvReaderOptions, stringBuilder.ToString())
            .ToList();

        Assert.AreEqual(2, result.Count);

        Assert.IsTrue(result.All(x => x.IsValid));

        Assert.AreEqual("Philipp", result[0].Result.FirstName);
        Assert.AreEqual("Wagner", result[0].Result.LastName);

        Assert.AreEqual(0, result[0].RowIndex);
        Assert.AreEqual(1986, result[0].Result.BirthDate.Year);
        Assert.AreEqual(5, result[0].Result.BirthDate.Month);
        Assert.AreEqual(12, result[0].Result.BirthDate.Day);

        Assert.AreEqual("Max", result[1].Result.FirstName);
        Assert.AreEqual("Mustermann", result[1].Result.LastName);

        Assert.AreEqual(1, result[1].RowIndex);
        Assert.AreEqual(2014, result[1].Result.BirthDate.Year);
        Assert.AreEqual(1, result[1].Result.BirthDate.Month);
        Assert.AreEqual(1, result[1].Result.BirthDate.Day);
    }


    [Test]
    public void EmptyDataTest()
    {
        var csvParserOptions = new CsvParserOptions(true, ';');
        var csvReaderOptions = new CsvReaderOptions([Environment.NewLine]);

        var csvMapper = new CsvPersonMapping();
        var csvParser = new CsvParser<Person>(csvParserOptions, csvMapper);

        var stringBuilder = new StringBuilder()
            .AppendLine("FirstName;LastName;BirthDate");

        var result = csvParser
            .ReadFromString(csvReaderOptions, stringBuilder.ToString())
            .ToList();

        Assert.AreEqual(0, result.Count);
    }

    [Test]
    public void TrimLineTest()
    {
        var csvParserOptions = new CsvParserOptions(true, ';', 1, true);
        var csvReaderOptions = new CsvReaderOptions([Environment.NewLine]);
        var csvMapper = new CsvPersonMapping();
        var csvParser = new CsvParser<Person>(csvParserOptions, csvMapper);

        var stringBuilder = new StringBuilder()
            .AppendLine("FirstName;LastName;BirthDate")
            .AppendLine("     Philipp;Wagner;1986/05/12       ")
            .AppendLine("Max;Mustermann;2014/01/01");

        var result = csvParser
            .ReadFromString(csvReaderOptions, stringBuilder.ToString())
            .ToList();

        Assert.AreEqual(2, result.Count);

        Assert.IsTrue(result.All(x => x.IsValid));

        Assert.AreEqual("Philipp", result[0].Result.FirstName);
        Assert.AreEqual("Wagner", result[0].Result.LastName);

        Assert.AreEqual(1986, result[0].Result.BirthDate.Year);
        Assert.AreEqual(5, result[0].Result.BirthDate.Month);
        Assert.AreEqual(12, result[0].Result.BirthDate.Day);

        Assert.AreEqual("Max", result[1].Result.FirstName);
        Assert.AreEqual("Mustermann", result[1].Result.LastName);
        Assert.AreEqual(2014, result[1].Result.BirthDate.Year);
        Assert.AreEqual(1, result[1].Result.BirthDate.Month);
        Assert.AreEqual(1, result[1].Result.BirthDate.Day);
    }


    [Test]
    public void ParallelLinqTest()
    {
        var csvParserOptions = new CsvParserOptions(true, ';', 3, true);
        var csvReaderOptions = new CsvReaderOptions([Environment.NewLine]);
        var csvMapper = new CsvPersonMapping();
        var csvParser = new CsvParser<Person>(csvParserOptions, csvMapper);

        var stringBuilder = new StringBuilder()
            .AppendLine("FirstName;LastName;BirthDate")
            .AppendLine("Philipp;Wagner;1986/05/12")
            .AppendLine("Max;Mustermann;2014/01/01");

        var result = csvParser
            .ReadFromString(csvReaderOptions, stringBuilder.ToString())
            .Where(x => x.IsValid)
            .Where(x => x.Result.FirstName == "Philipp")
            .ToList();

        Assert.AreEqual(1, result.Count);

        Assert.AreEqual("Philipp", result[0].Result.FirstName);
        Assert.AreEqual("Wagner", result[0].Result.LastName);

        Assert.AreEqual(1986, result[0].Result.BirthDate.Year);
        Assert.AreEqual(5, result[0].Result.BirthDate.Month);
        Assert.AreEqual(12, result[0].Result.BirthDate.Day);
    }

    [Test]
    public void CommentLineTest()
    {
        var csvParserOptions = new CsvParserOptions(true, "#", new StringSplitTokenizer([';'], false));
        var csvReaderOptions = new CsvReaderOptions([Environment.NewLine]);
        var csvMapper = new CsvPersonMapping();
        var csvParser = new CsvParser<Person>(csvParserOptions, csvMapper);

        var stringBuilder = new StringBuilder()
            .AppendLine("FirstName;LastName;BirthDate")
            .AppendLine("#Philipp;Wagner;1986/05/12")
            .AppendLine("Max;Mustermann;2014/01/01");

        var result = csvParser
            .ReadFromString(csvReaderOptions, stringBuilder.ToString())
            .Where(x => x.IsValid)
            .ToList();

        Assert.AreEqual(1, result.Count);

        Assert.AreEqual("Max", result[0].Result.FirstName);
        Assert.AreEqual("Mustermann", result[0].Result.LastName);

        Assert.AreEqual(2014, result[0].Result.BirthDate.Year);
        Assert.AreEqual(1, result[0].Result.BirthDate.Month);
        Assert.AreEqual(1, result[0].Result.BirthDate.Day);
    }

    [Test]
    public void StringArrayMappingTest()
    {
        var csvParserOptions = new CsvParserOptions(false, ';');
        var csvReaderOptions = new CsvReaderOptions([Environment.NewLine]);
        var csvMapper = new CsvStringArrayMapping();
        var csvParser = new CsvParser<string[]>(csvParserOptions, csvMapper);

        var stringBuilder = new StringBuilder()
            .AppendLine("Philipp;Wagner;1986/05/12")
            .AppendLine("Max;Mustermann;2014/01/01");

        var result = csvParser
            .ReadFromString(csvReaderOptions, stringBuilder.ToString())
            .ToList();

        Assert.AreEqual(2, result.Count);

        Assert.IsTrue(result.All(x => x.IsValid));

        Assert.AreEqual("Philipp", result[0].Result[0]);
        Assert.AreEqual("Wagner", result[0].Result[1]);
        Assert.AreEqual("1986/05/12", result[0].Result[2]);

        Assert.AreEqual("Max", result[1].Result[0]);
        Assert.AreEqual("Mustermann", result[1].Result[1]);
        Assert.AreEqual("2014/01/01", result[1].Result[2]);
    }
}
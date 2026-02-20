// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using TinyCsvParser.Models;

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
    public void ToStringTest()
    {
        var csvParserOptions = CsvOptions.Default;
        var csvMapper = new CsvPersonMapping();
        var csvParser = new CsvParser<Person>(csvParserOptions, csvMapper);

        var result = string.Empty;
        NUnit.Framework.Assert.DoesNotThrow(() => result = csvParser.ToString());
        Assert.IsNotEmpty(result);
    }

    [Test]
    public void SkipHeaderTest()
    {
        var csvOptions = CsvOptions.Default with
        {
            SkipHeader = true
        };

        var csvMapper = new CsvPersonMapping();
        var csvParser = new CsvParser<Person>(csvOptions, csvMapper);

        var stringBuilder = new StringBuilder()
            .AppendLine("FirstName;LastName;BirthDate")
            .AppendLine("Philipp;Wagner;1986/05/12")
            .AppendLine("Max;Mustermann;2014/01/01");

        var result = csvParser
            .ReadFromString(stringBuilder.ToString())
            .ToList();

        Assert.AreEqual(2, result.Count);

        Assert.IsTrue(result.All(x => x.IsSuccess));

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
    public void DoNotSkipHeaderTest()
    {
        var csvOptions = CsvOptions.Default with
        {
            SkipHeader = false
        };

        var csvMapper = new CsvPersonMapping();
        var csvParser = new CsvParser<Person>(csvOptions, csvMapper);

        var stringBuilder = new StringBuilder()
            .AppendLine("Philipp;Wagner;1986/05/12")
            .AppendLine("Max;Mustermann;2014/01/01");

        var result = csvParser
            .ReadFromString(stringBuilder.ToString())
            .ToList();

        Assert.AreEqual(2, result.Count);

        Assert.IsTrue(result.All(x => x.IsSuccess));

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

    private class CsvPersonHeaderMapping : CsvMapping<Person>
    {
        public CsvPersonHeaderMapping()
        {
            MapProperty("FirstName", x => x.FirstName);
            MapProperty("LastName", x => x.LastName);
            MapProperty("BirthDate", x => x.BirthDate);
        }
    }


    [Test]
    public void UseColumnNamesTest()
    {
        var csvOptions = CsvOptions.Default with
        {
            SkipHeader = false
        };

        var csvMapper = new CsvPersonHeaderMapping();
        var csvParser = new CsvParser<Person>(csvOptions, csvMapper);

        var stringBuilder = new StringBuilder()
            .AppendLine("FirstName;LastName;BirthDate")
            .AppendLine("Philipp;Wagner;1986/05/12")
            .AppendLine("Max;Mustermann;2014/01/01");

        var result = csvParser
            .ReadFromString(stringBuilder.ToString())
            .ToList();

        Assert.AreEqual(2, result.Count);

        Assert.IsTrue(result.All(x => x.IsSuccess));

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
    public void EmptyDataTest()
    {
        var csvOptions = CsvOptions.Default with
        {
            SkipHeader = true
        };

        var csvMapper = new CsvPersonMapping();
        var csvParser = new CsvParser<Person>(csvOptions, csvMapper);

        var stringBuilder = new StringBuilder()
            .AppendLine("FirstName;LastName;BirthDate");

        var result = csvParser
            .ReadFromString(stringBuilder.ToString())
            .ToList();

        Assert.AreEqual(0, result.Count);
    }
}
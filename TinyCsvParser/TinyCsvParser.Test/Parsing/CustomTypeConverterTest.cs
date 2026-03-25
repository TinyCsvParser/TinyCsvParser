// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyCsvParser.Models;
using TinyCsvParser.TypeConverters;

namespace TinyCsvParser.Test.Parsing;

[TestFixture]
public class TinyCsvParserTest
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

    private class CsvPersonMappingWithTypeConverterProvider : CsvMapping<Person>
    {
        public CsvPersonMappingWithTypeConverterProvider()
            : this(new TypeConverterProvider())
        {
        }
        public CsvPersonMappingWithTypeConverterProvider(ITypeConverterProvider typeConverterProvider)
            : base(typeConverterProvider)
        {
            MapProperty(0, x => x.FirstName);
            MapProperty(1, x => x.LastName);
            MapProperty(2, x => x.BirthDate);
        }
    }

    private class CsvPersonMappingWithCustomConverter : CsvMapping<Person>
    {
        public CsvPersonMappingWithCustomConverter()
        {
            MapProperty(0, x => x.FirstName);
            MapProperty(1, x => x.LastName);
            MapProperty(2, x => x.BirthDate, new DateTimeConverter("yyyy###MM###dd"));
        }
    }

    [Test]
    public void WeirdDateTimeTest_CustomConverterBased()
    {
        CsvOptions csvOptions = CsvOptions.Default with
        {
            SkipHeader = true
        };

        CsvPersonMappingWithCustomConverter csvMapper = new();
        CsvParser<Person> csvParser = new(csvOptions, csvMapper);

        StringBuilder stringBuilder = new StringBuilder()
            .AppendLine("FirstName;LastName;BirthDate")
            .AppendLine("Philipp;Wagner;1986###05###12");

        List<CsvMappingResult<Person>> result = csvParser
            .ReadFromString(stringBuilder.ToString())
            .ToList();

        Assert.AreEqual("Philipp", result[0].Result.FirstName);
        Assert.AreEqual("Wagner", result[0].Result.LastName);

        Assert.AreEqual(1986, result[0].Result.BirthDate.Year);
        Assert.AreEqual(5, result[0].Result.BirthDate.Month);
        Assert.AreEqual(12, result[0].Result.BirthDate.Day);
    }
}
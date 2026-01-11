// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using TinyCsvParser.Mapping;
using TinyCsvParser.Model;

namespace TinyCsvParser.Test.Issues;

public static class MyCsvParserExtensions
{
    public static ParallelQuery<CsvMappingResult<TEntity>> ReadFromFile<TEntity>(this CsvParser<TEntity> csvParser, string fileName, Encoding encoding, int skipLast)
    {
        ArgumentNullException.ThrowIfNull(fileName);

        var lines = File
            .ReadLines(fileName, encoding)
            .SkipLast(skipLast)
            .Select((line, index) => new Row(index, line));

        return csvParser.Parse(lines);
    }
}


[TestFixture]
public class Issue63_SkipLast
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
    public void TestSkipLastOne()
    {
        var csvParserOptions = new CsvParserOptions(skipHeader: true, ';');
        var csvMapper = new CsvPersonMapping();
        var csvParser = new CsvParser<Person>(csvParserOptions, csvMapper);

        var stringBuilder = new StringBuilder()
            .AppendLine("FirstName;LastName;BirthDate")
            .AppendLine("     Philipp;Wagner;1986/05/12       ")
            .AppendLine("Max;Mustermann;2014/01/01");

        var basePath = AppDomain.CurrentDomain.BaseDirectory;
        var filePath = Path.Combine(basePath, "test_file.txt");

        File.WriteAllText(filePath, stringBuilder.ToString(), Encoding.UTF8);

        var result = csvParser
            .ReadFromFile(filePath, Encoding.UTF8, skipLast: 1)
            .ToList();

        Assert.AreEqual(1, result.Count);

        Assert.AreEqual("Philipp", result[0].Result.FirstName);
        Assert.AreEqual("Wagner", result[0].Result.LastName);
    }

    [Test]
    public void TestSkipLastTwo()
    {
        var csvParserOptions = new CsvParserOptions(skipHeader: true, ';');
        var csvMapper = new CsvPersonMapping();
        var csvParser = new CsvParser<Person>(csvParserOptions, csvMapper);

        var stringBuilder = new StringBuilder()
            .AppendLine("FirstName;LastName;BirthDate")
            .AppendLine("     Philipp;Wagner;1986/05/12       ")
            .AppendLine("Max;Mustermann;2014/01/01");

        var basePath = AppDomain.CurrentDomain.BaseDirectory;
        var filePath = Path.Combine(basePath, "test_file.txt");

        File.WriteAllText(filePath, stringBuilder.ToString(), Encoding.UTF8);

        var result = csvParser
            .ReadFromFile(filePath, Encoding.UTF8, skipLast: 2)
            .ToList();

        Assert.AreEqual(0, result.Count);
    }

}
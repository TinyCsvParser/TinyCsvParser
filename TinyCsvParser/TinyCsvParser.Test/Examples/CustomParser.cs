// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using TinyCsvParser.Model;

namespace TinyCsvParser.Test.Examples;

public class CustomCsvParser
{
    private readonly CsvParserOptions options;

    public CustomCsvParser(CsvParserOptions options)
    {
        this.options = options;
    }

    public ParallelQuery<TokenizedRow> Parse(IEnumerable<Row> csvData)
    {
        ArgumentNullException.ThrowIfNull(csvData);

        var query = csvData
            .Skip(options.SkipHeader ? 1 : 0)
            .AsParallel();

        // If you want to get the same order as in the CSV file, this option needs to be set:
        if (options.KeepOrder)
        {
            query = query.AsOrdered();
        }

        query = query
            .WithDegreeOfParallelism(options.DegreeOfParallelism)
            .Where(row => !string.IsNullOrWhiteSpace(row.Data));

        // Ignore Lines, that start with a comment character:
        if (!string.IsNullOrWhiteSpace(options.CommentCharacter))
        {
            query = query.Where(line => !line.Data.StartsWith(options.CommentCharacter));
        }

        return query
            .Select(line => new TokenizedRow(line.Index, options.Tokenizer.Tokenize(line.Data)));
    }

    public override string ToString()
    {
        return $"CsvParser (Options = {options})";
    }
}

public static class CustomCsvParserExtensions
{
    public static ParallelQuery<TokenizedRow> ReadFromFile(this CustomCsvParser csvParser, string fileName,
        Encoding encoding)
    {
        ArgumentNullException.ThrowIfNull(fileName);

        var lines = File
            .ReadLines(fileName, encoding)
            .Select((line, index) => new Row(index, line));

        return csvParser.Parse(lines);
    }
}

[TestFixture]
[Ignore("Example for Issue 47: https://github.com/bytefish/TinyCsvParser/issues/47")]
public class CustomCsvParserTest
{
    [Test]
    public void CustomCsvParserReadFromFile()
    {
        var options = new CsvParserOptions(true, ';');

        var customCsvParser = new CustomCsvParser(options);

        var stringBuilder = new StringBuilder()
            .AppendLine("FirstName;LastName;BirthDate")
            .AppendLine("     Philipp;Wagner;1986/05/12       ")
            .AppendLine("Max;Mustermann;2014/01/01");

        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test_file.txt");

        File.WriteAllText(filePath, stringBuilder.ToString(), Encoding.UTF8);

        var result = customCsvParser
            .ReadFromFile(filePath, Encoding.UTF8)
            .ToList();

        Assert.AreEqual(2, result.Count);


        Assert.AreEqual("Philipp", result[0].Tokens[0]);
        Assert.AreEqual("Wagner", result[0].Tokens[1]);

        Assert.AreEqual("Max", result[1].Tokens[0]);
        Assert.AreEqual("Mustermann", result[1].Tokens[1]);
    }

    [Test]
    public void CustomCsvParserGetAsStringArrays()
    {
        var options = new CsvParserOptions(true, ';');

        var customCsvParser = new CustomCsvParser(options);

        var stringBuilder = new StringBuilder()
            .AppendLine("FirstName;LastName;BirthDate")
            .AppendLine("     Philipp;Wagner;1986/05/12       ")
            .AppendLine("Max;Mustermann;2014/01/01");

        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test_file.txt");

        File.WriteAllText(filePath, stringBuilder.ToString(), Encoding.UTF8);

        var result = customCsvParser
            .ReadFromFile(filePath, Encoding.UTF8)
            .Select(x => x.Tokens)
            .ToList();

        Assert.AreEqual(2, result.Count);


        Assert.AreEqual("Philipp", result[0][0]);
        Assert.AreEqual("Wagner", result[0][1]);

        Assert.AreEqual("Max", result[1][0]);
        Assert.AreEqual("Mustermann", result[1][1]);
    }
}
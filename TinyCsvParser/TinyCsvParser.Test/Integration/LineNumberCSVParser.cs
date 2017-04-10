using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using TinyCsvParser.Mapping;
using TinyCsvParser.Test.Benchmarks;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.Test.Integration
{
  // Holds the Row Data and Line Number:
  public class Row
  {
    public readonly string Data;
    public readonly int Position;

    public Row(int position, string data)
    {
      Position = position;
      Data = data;
    }
  }

  // Holds the Tokenized Row Data and Line Number:
  public class TokenizedRow
  {
    public readonly string[] Data;
    public readonly int Position;

    public TokenizedRow(int position, string[] data)
    {
      Position = position;
      Data = data;
    }
  }

  // Holds the Parse Result and Line Number:
  public class ParsedRow<TEntity>
    where TEntity : class, new()
  {
    public readonly int Position;
    public readonly CsvMappingResult<TEntity> Result;

    public ParsedRow(int position, CsvMappingResult<TEntity> result)
    {
      Position = position;
      Result = result;
    }
  }

  public static class LineCountingCsvParserExtensions
  {
    public static ParallelQuery<ParsedRow<TEntity>> ReadFromFile<TEntity>(this LineCountingCsvParser<TEntity> csvParser, string fileName, Encoding encoding)
      where TEntity : class, new()
    {
      if (fileName == null)
        throw new ArgumentNullException(nameof(fileName));

      return csvParser.Parse(File.ReadLines(fileName, encoding));
    }
  }

  // Implements a CsvParser, which counts rows:
    public class LineCountingCsvParser<TEntity>
    where TEntity : class, new()
  {
    private readonly CsvMapping<TEntity> _mapping;
    private readonly CsvParserOptions _options;

    public LineCountingCsvParser(CsvParserOptions options, CsvMapping<TEntity> mapping)
    {
      this._options = options;
      this._mapping = mapping;
    }

    public ParallelQuery<ParsedRow<TEntity>> Parse(IEnumerable<string> csvData)
    {
      if (csvData == null)
        throw new ArgumentNullException(nameof(csvData));

      var query = csvData
        .Select((x, pos) => new Row(pos, x))
        .Skip(_options.SkipHeader ? 1 : 0)
        .AsParallel();

      // If you want to get the same order as in the CSV file, this option needs to be set:
      if (_options.KeepOrder)
        query = query.AsOrdered();

      query = query
        .WithDegreeOfParallelism(_options.DegreeOfParallelism)
        .Where(row => !string.IsNullOrWhiteSpace(row.Data));

      // Ignore Lines, that start with a comment character:
      if (!string.IsNullOrWhiteSpace(_options.CommentCharacter))
        query = query.Where(row => !row.Data.StartsWith(_options.CommentCharacter));

      return query
        .Select(row => new TokenizedRow(row.Position, _options.Tokenizer.Tokenize(row.Data)))
        .Select(tokenizedRow => new ParsedRow<TEntity>(tokenizedRow.Position, _mapping.Map(tokenizedRow.Data)));
    }

    public ParallelQuery<ParsedRow<TEntity>> ReadFromString(CsvReaderOptions csvReaderOptions, string csvData)
    {
      var lines = csvData.Split(csvReaderOptions.NewLine, StringSplitOptions.None);

      return Parse(lines);
    }


    public override string ToString()
    {
      return $"CsvParser (Options = {_options}, Mapping = {_mapping})";
    }
  }

  [TestFixture]
  public class LineNumberCsvParserTest
  {
    private class Person
    {
      public DateTime BirthDate { get; set; }
      public string FirstName { get; set; }
      public string LastName { get; set; }
    }

    public class LocalWeatherData
    {
      public string WBAN { get; set; }
      public DateTime Date { get; set; }
      public string SkyCondition { get; set; }
    }


    public class LocalWeatherDataMapper : CsvMapping<LocalWeatherData>
    {
      public LocalWeatherDataMapper()
      {
        MapProperty(0, x => x.WBAN);
        MapProperty(1, x => x.Date).WithCustomConverter(new DateTimeConverter("yyyyMMdd"));
        MapProperty(4, x => x.SkyCondition);
      }
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
    public void CsvParserWithLineCountingTest()
    {
      var csvParserOptions = new CsvParserOptions(true, new[] {';'});
      var csvReaderOptions = new CsvReaderOptions(new[] {Environment.NewLine});
      var csvMapper = new CsvPersonMapping();
      var csvParser = new LineCountingCsvParser<Person>(csvParserOptions, csvMapper);

      var stringBuilder = new StringBuilder()
          .AppendLine("FirstName;LastName;BirthDate")
          .AppendLine("Philipp;Wagner;1986/05/12")
          .AppendLine("")
          .AppendLine("Max;Mustermann;2014/01/01")
          .AppendLine("Philipp;Wagner;0/05/12")
        ;

      var result = csvParser
        .ReadFromString(csvReaderOptions, stringBuilder.ToString())
        .ToList();

      Assert.AreEqual(3, result.Count);

      Assert.AreEqual(1, result[0].Position);
      Assert.AreEqual(3, result[1].Position);

      Assert.AreEqual(1, result.Count(c=>!c.Result.IsValid));

    }

    [Test, Ignore("Slow running test, requires https://www.ncdc.noaa.gov/orders/qclcd/QCLCD201503.zip")]
    public void BenchmarkWithLines()
    {

      CsvParserOptions csvParserOptions = new CsvParserOptions(true, new[] { ',' });
      LocalWeatherDataMapper csvMapper = new LocalWeatherDataMapper();
      var csvParser = new LineCountingCsvParser<LocalWeatherData>(csvParserOptions, csvMapper);

      MeasurementUtils.MeasureElapsedTime($"LocalWeather (with line numbers)",
        () =>
        {
          var a = csvParser
            .ReadFromFile(@"c:\filepathtoweatherdata\201503hourly.txt", Encoding.ASCII)
            .ToList();
        });
    }
  }
}
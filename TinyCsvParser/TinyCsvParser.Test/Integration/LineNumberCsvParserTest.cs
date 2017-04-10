using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyCsvParser.Mapping;

namespace TinyCsvParser.Test.Integration
{
    // Holds the Row Data and Line Number:
    public class Row
    {
        public readonly int Position;
        public readonly string Data;

        public Row(int position, string data)
        {
            Position = position;
            Data = data;
        }
    }

    // Holds the Tokenized Row Data and Line Number:
    public class TokenizedRow
    {
        public readonly int Position;
        public readonly string[] Data;

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

    // Implements a CsvParser, which counts rows:
    public class LineCountingCsvParser<TEntity>
        where TEntity : class, new()
    {
        private readonly CsvParserOptions options;
        private readonly CsvMapping<TEntity> mapping;

        public LineCountingCsvParser(CsvParserOptions options, CsvMapping<TEntity> mapping)
        {
            this.options = options;
            this.mapping = mapping;
        }

        public ParallelQuery<ParsedRow<TEntity>> Parse(IEnumerable<string> csvData)
        {
            if (csvData == null)
            {
                throw new ArgumentNullException("csvData");
            }

            var query = csvData
                .Select((x, pos) => new Row(pos, x))
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
                query = query.Where(row => !row.Data.StartsWith(options.CommentCharacter));
            }

            return query
                .Select(row => new TokenizedRow(row.Position, options.Tokenizer.Tokenize(row.Data)))
                .Select(tokenizedRow => new ParsedRow<TEntity>(tokenizedRow.Position, mapping.Map(tokenizedRow.Data)));
        }

        public ParallelQuery<ParsedRow<TEntity>> ReadFromString(CsvReaderOptions csvReaderOptions, string csvData)
        {
            var lines = csvData.Split(csvReaderOptions.NewLine, StringSplitOptions.None);

            return Parse(lines);
        }

        public override string ToString()
        {
            return string.Format("CsvParser (Options = {0}, Mapping = {1})", options, mapping);
        }
    }

    [TestFixture]
    public class LineNumberCsvParserTest
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
        public void CsvParserWithLineCountingTest()
        {
            var csvParserOptions = new CsvParserOptions(true, new[] { ';' });
            var csvReaderOptions = new CsvReaderOptions(new[] { Environment.NewLine });
            var csvMapper = new CsvPersonMapping();
            var csvParser = new LineCountingCsvParser<Person>(csvParserOptions, csvMapper);

            var stringBuilder = new StringBuilder()
                .AppendLine("FirstName;LastName;BirthDate")
                .AppendLine("Philipp;Wagner;1986/05/12")
                .AppendLine("")
                .AppendLine("Max;Mustermann;2014/01/01");

            var result = csvParser
                .ReadFromString(csvReaderOptions, stringBuilder.ToString())
                .ToList();

            Assert.AreEqual(2, result.Count);

            Assert.AreEqual(1, result[0].Position);
            Assert.AreEqual(3, result[1].Position);

            // Asserts ...
        }
    }
}

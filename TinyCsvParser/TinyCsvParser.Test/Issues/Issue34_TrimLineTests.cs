﻿// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using TinyCsvParser.Mapping;
using TinyCsvParser.Tokenizer;

namespace TinyCsvParser.Test.Issues
{

    [TestFixture]
    [Description("https://github.com/bytefish/TinyCsvParser/issues/34")]
    public class Issue34_TrimLineTests
    {

        private class TestEntity
        {
            public string Column1 { get; set; }

            public string Column2 { get; set; }

            public string Column3 { get; set; }
        }

        private class TestEntityMapping : CsvMapping<TestEntity>
        {
            public TestEntityMapping()
            {
                MapProperty(0, x => x.Column1);
                MapProperty(1, x => x.Column2);
                MapProperty(2, x => x.Column3);
            }
        }

        [Test]
        public void LastLineParseTest()
        {
            // We have a Line, where the last Column is "Empty":
            string csvLine = "1\t2\t";

            // We want to check with different Tokenizers:
            CsvParser<TestEntity>[] csvParsers = new[]
            {
                GetRfc4180Parser(),
                GetStringSplitParser()
            };

            // Now iterate over the Parsers:
            CsvReaderOptions csvReaderOptions = new CsvReaderOptions(new[] { Environment.NewLine });

            foreach (var csvParser in csvParsers)
            {
                var result = csvParser
                    .ReadFromString(csvReaderOptions, csvLine)
                    .result
                    .ToList();

                Assert.AreEqual(1, result.Count);

                Assert.IsTrue(result.All(x => x.IsValid));

                Assert.AreEqual("1", result.First().Result.Column1);
                Assert.AreEqual("2", result.First().Result.Column2);
                Assert.AreEqual(string.Empty, result.First().Result.Column3);
            }
        }

        private CsvParser<TestEntity> GetRfc4180Parser()
        {
            // Construct the Parser:
            CsvParserOptions csvParserOptions = new CsvParserOptions(false, new QuotedStringTokenizer('\t'));
            TestEntityMapping csvMapper = new TestEntityMapping();

            return new CsvParser<TestEntity>(csvParserOptions, csvMapper);
        }

        private CsvParser<TestEntity> GetStringSplitParser()
        {
            // Construct the Parser:
            CsvParserOptions csvParserOptions = new CsvParserOptions(false, new StringSplitTokenizer(new[] { '\t' }, true));
            TestEntityMapping csvMapper = new TestEntityMapping();

            return new CsvParser<TestEntity>(csvParserOptions, csvMapper);
        }


    }
}

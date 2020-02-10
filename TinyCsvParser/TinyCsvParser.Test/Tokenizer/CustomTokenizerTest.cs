// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using System.Linq;
using System.Text;
using TinyCsvParser.Mapping;
using TinyCsvParser.Tokenizer;

namespace TinyCsvParser.Test.Tokenizer
{
    [TestFixture]
    public class CustomTokenizerTest
    {
        private class CustomTokenizer : ITokenizer
        {
            public string[] Tokenize(string input)
            {
                return input?.Split(';');
            }
        }

        [Test]
        public void IgnoreCommentedLinesAndHeader()
        {
            CsvParserOptions csvParserOptions = new CsvParserOptions(true, "#", new CustomTokenizer());
            CsvReaderOptions csvReaderOptions = new CsvReaderOptions(new[] { Environment.NewLine });
            CsvStringArrayMapping csvMapper = new CsvStringArrayMapping();
            CsvParser<string[]> csvParser = new CsvParser<string[]>(csvParserOptions, csvMapper);

            var stringBuilder = new StringBuilder()
                                .AppendLine("#comment1")
                                .AppendLine("#comment2")
                                .AppendLine("FirstName;LastName;BirthDate")
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
}

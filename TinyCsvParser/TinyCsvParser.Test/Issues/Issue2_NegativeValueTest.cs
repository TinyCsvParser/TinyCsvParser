// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using System.Linq;
using System.Text;
using TinyCsvParser.Mapping;

namespace TinyCsvParser.Test.Issues
{

    [TestFixture, Description("https://github.com/bytefish/TinyCsvParser/issues/2")]
    public class Issue2_NegativeValueTest
    {
        public class NegativeValueEntity
        {
            public int Value { get; set; }
        }

        private class NegativeValueEntityMapping : CsvMapping<NegativeValueEntity>
        {
            public NegativeValueEntityMapping()
            {
                MapProperty(0, x => x.Value);
            }
        }

        [Test]
        public void NegativeValueParsingTest()
        {
            CsvParserOptions csvParserOptions = new CsvParserOptions(true, ';');
            CsvReaderOptions csvReaderOptions = new CsvReaderOptions(new[] { Environment.NewLine });
            NegativeValueEntityMapping csvMapper = new NegativeValueEntityMapping();
            CsvParser<NegativeValueEntity> csvParser = new CsvParser<NegativeValueEntity>(csvParserOptions, csvMapper);

            var stringBuilder = new StringBuilder()
                .AppendLine("Value")
                .AppendLine("-1");

            var result = csvParser
                .ReadFromString(csvReaderOptions, stringBuilder.ToString())
                .ToList();

            Assert.AreEqual(1, result.Count);
            
            Assert.IsTrue(result.All(x => x.IsValid));

            Assert.AreEqual(-1, result.First().Result.Value);
        }
    }
}

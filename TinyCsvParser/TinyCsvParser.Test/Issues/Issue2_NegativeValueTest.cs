// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyCsvParser.Mapping;

namespace TinyCsvParser.Test.Issues
{

    //https://github.com/bytefish/TinyCsvParser/issues/2
    [TestClass]
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

        [TestMethod]
        public async Task NegativeValueParsingTest()
        {
            CsvParserOptions csvParserOptions = new CsvParserOptions(true, ';');
            CsvReaderOptions csvReaderOptions = new CsvReaderOptions(new[] { Environment.NewLine });
            NegativeValueEntityMapping csvMapper = new NegativeValueEntityMapping();
            CsvParser<NegativeValueEntity> csvParser = new CsvParser<NegativeValueEntity>(csvParserOptions, csvMapper);

            var stringBuilder = new StringBuilder()
                .AppendLine("Value")
                .AppendLine("-1");

            var result = await csvParser
                .ReadFromStringAsync(csvReaderOptions, stringBuilder.ToString())
                .ToListAsync();

            Assert.AreEqual(1, result.Count);
            
            Assert.IsTrue(result.All(x => x.IsValid));

            Assert.AreEqual(-1, result.First().Result.Value);
        }
    }
}

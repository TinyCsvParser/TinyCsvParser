// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using System.Linq;
using System.Text;
using TinyCsvParser.Mapping;
using TinyCsvParser.Tokenizer;

namespace TinyCsvParser.Test.Issues
{
    [TestFixture, Description("https://github.com/bytefish/TinyCsvParser/issues/4")]
    public class Issue4_QuotedStringProcessingTest
    {
        public class SampleEntity
        {
            public string Column1 { get; set; }

            public string Column2 { get; set; }

            public string Column3 { get; set; }

            public string Column4 { get; set; }
        }

        private class SampleEntityMapping : CsvMapping<SampleEntity>
        {
            public SampleEntityMapping()
            {
                MapProperty(0, x => x.Column1);
                MapProperty(1, x => x.Column2);
                MapProperty(2, x => x.Column3);
                MapProperty(3, x => x.Column4);
            }
        }

        [Test]
        public void QuotedStringParsingTest()
        {
            CsvParserOptions csvParserOptions = new CsvParserOptions(true, string.Empty, new QuotedStringTokenizer(','));
            CsvReaderOptions csvReaderOptions = new CsvReaderOptions(new[] { Environment.NewLine });
            SampleEntityMapping csvMapper = new SampleEntityMapping();
            CsvParser<SampleEntity> csvParser = new CsvParser<SampleEntity>(csvParserOptions, csvMapper);

            var stringBuilder = new StringBuilder()
                .AppendLine("Column1,Colum2,Column3,Column4")
                .AppendLine("1,\"2,3,4\",\"5,6,7\",8");

            var result = csvParser
                .ReadFromString(csvReaderOptions, stringBuilder.ToString())
                .Items
                .ToList();

            Assert.AreEqual(1, result.Count);

            Assert.IsTrue(result.All(x => x.IsValid));

            Assert.AreEqual("1", result.First().Result.Column1);
            Assert.AreEqual("2,3,4", result.First().Result.Column2);
            Assert.AreEqual("5,6,7", result.First().Result.Column3);
            Assert.AreEqual("8", result.First().Result.Column4);
        }
    }
}
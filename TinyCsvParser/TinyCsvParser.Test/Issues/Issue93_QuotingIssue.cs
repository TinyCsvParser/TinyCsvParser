// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using System.Linq;
using System.Text;
using TinyCsvParser.Mapping;
using TinyCsvParser.Tokenizer;

namespace TinyCsvParser.Test.Issues
{

    [TestFixture]
    public class Issue93_QuotingIssue
    {

        private class SomeDto
        {
            public string Column1 { get; set; }
            public string Column2 { get; set; }
            public string Column3 { get; set; }
            public string Column4 { get; set; }

        }

        private class SomeDtoMapping : CsvMapping<SomeDto>
        {
            public SomeDtoMapping()
            {
                MapProperty(0, x => x.Column1);
                MapProperty(1, x => x.Column2);
                MapProperty(2, x => x.Column3);
                MapProperty(3, x => x.Column4);
            }
        }

        [Test]
        public void TestSkipLastOne()
        {
            CsvParserOptions csvParserOptions = new CsvParserOptions(skipHeader: false, tokenizer: new StringSplitTokenizer(new char[] { '|' }, false));
            CsvReaderOptions csvReaderOptions = new CsvReaderOptions(new[] { Environment.NewLine });
            SomeDtoMapping csvMapper = new SomeDtoMapping();
            CsvParser<SomeDto> csvParser = new CsvParser<SomeDto>(csvParserOptions, csvMapper);

            var csv = new StringBuilder()
                .AppendLine("1|1|\" 1 BALL VALVE|2129")
                .ToString();

            var result = csvParser
                .ReadFromString(csvReaderOptions, csv)
                .Items
                .ToList();

            Assert.AreEqual(1, result.Count);

            Assert.AreEqual("1", result[0].Result.Column1);
            Assert.AreEqual("1", result[0].Result.Column2);
            Assert.AreEqual("\" 1 BALL VALVE", result[0].Result.Column3);
            Assert.AreEqual("2129", result[0].Result.Column4);

        }
    }
}

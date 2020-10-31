// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using System.Linq;
using System.Text;
using TinyCsvParser.Mapping;
using TinyCsvParser.Tokenizer;

namespace TinyCsvParser.Test.Issues
{
    [TestFixture, Description("https://github.com/bytefish/TinyCsvParser/issues/60")]
    public class Issue60_TrailingComma
    {
        public class TeacherEntity
        {
            public string Col0 { get; set; }

            public string Col1 { get; set; }

            public int Col2 { get; set; }

            public int Col3 { get; set; }

            public float Col4 { get; set; }

            public string Col5 { get; set; }

            public string Col6 { get; set; }
        }

        private class TeacherEntityMapping : CsvMapping<TeacherEntity>
        {
            public TeacherEntityMapping()
            {
                MapProperty(0, x => x.Col0);
                MapProperty(1, x => x.Col1);
                MapProperty(2, x => x.Col2);
                MapProperty(3, x => x.Col3);
                MapProperty(4, x => x.Col4);
                MapProperty(5, x => x.Col5);
                MapProperty(6, x => x.Col6);
            }
        }

        [Test]
        public void Issue60_TrailingComma_Test()
        {
            var csvParserOptions = new CsvParserOptions(false, '|');
            var csvReaderOptions = new CsvReaderOptions(new[] { Environment.NewLine });
            var csvMapper = new TeacherEntityMapping();
            var csvParser = new CsvParser<TeacherEntity>(csvParserOptions, csvMapper);

            var stringBuilder = new StringBuilder()
                .AppendLine(@"Johnson|Anne|110236|697125|1.00|Teacher|js@someemail.org");

            var result = csvParser
                .ReadFromString(csvReaderOptions, stringBuilder.ToString())
                .ToList();

            Assert.AreEqual(1, result.Count);

            Assert.IsTrue(result.All(x => x.IsValid));

            Assert.AreEqual("Johnson", result.First().Result.Col0);
            Assert.AreEqual("Anne", result.First().Result.Col1);
            Assert.AreEqual(110236, result.First().Result.Col2);
            Assert.AreEqual(697125, result.First().Result.Col3);
            Assert.AreEqual(1.00f, result.First().Result.Col4, 1e-3);
            Assert.AreEqual("Teacher", result.First().Result.Col5);
            Assert.AreEqual("js@someemail.org", result.First().Result.Col6);
            
        }
    }
}
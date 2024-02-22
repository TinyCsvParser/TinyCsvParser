// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TinyCsvParser.Mapping;
using TinyCsvParser.Tokenizer;

namespace TinyCsvParser.Test.Integration
{
    [TestFixture]
    [Ignore("Example for https://github.com/Golapadeog/blog/issues/1")]
    public class TokenizerBenchmark
    {
        private class TestModel
        {
            public string Prop1 { get; set; }

            public string Prop2 { get; set; }

            public string Prop3 { get; set; }
        }

        private class TestModelMapping : CsvMapping<TestModel>
        {
            public TestModelMapping()
            {
                MapProperty(0, x => x.Prop1);
                MapProperty(1, x => x.Prop2);
                MapProperty(2, x => x.Prop3);
            }
        }

        private class CustomTokenizer : ITokenizer
        {
            public string[] Tokenize(string input)
            {
                var result = new List<string>();

                bool isInQuotes = false;

                var chars = input.ToCharArray();

                StringBuilder str = new StringBuilder(string.Empty);

                foreach (var t in chars)
                {
                    if (t == '"')
                    {
                        isInQuotes = !isInQuotes;
                    }
                    else if (t == ',' && !isInQuotes)
                    {
                        result.Add(str.ToString());

                        str.Clear();
                    }
                    else
                    {
                        str.Append(t);
                    }
                }

                result.Add(str.ToString());

                return result.ToArray();
            }
        }

        [Test]
        public void RunTest()
        {
            var options = new CsvParserOptions(false, new StringSplitTokenizer(new[] { ',' }, false));
            var mapping = new TestModelMapping();
            var parser = new CsvParser<TestModel>(options, mapping);

            var testFilePath = GetTestFilePath();

            MeasurementUtils.MeasureElapsedTime("Reading 100 000 Lines", () =>
            {
                var lines = parser
                    .ReadFromFile(testFilePath, Encoding.UTF8)
                    .Items
                    .Where(x => x.IsValid)
                    .Count();

                Assert.AreEqual(100000, lines);
            });
        }

        [SetUp]
        public void SetUp()
        {
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < 100000; i++)
            {
                stringBuilder.AppendLine("1312452433443,93742834623543,234277237242");
            }

            var testFilePath = GetTestFilePath();

            File.WriteAllText(testFilePath, stringBuilder.ToString(), Encoding.UTF8);
        }

        [TearDown]
        public void TearDown()
        {
            var testFilePath = GetTestFilePath();

            File.Delete(testFilePath);
        }

        private string GetTestFilePath()
        {
#if NETCOREAPP1_1
            var basePath = AppContext.BaseDirectory;
#else
            var basePath = AppDomain.CurrentDomain.BaseDirectory;
#endif
            return Path.Combine(basePath, "test_file.txt");
        }
    }
}

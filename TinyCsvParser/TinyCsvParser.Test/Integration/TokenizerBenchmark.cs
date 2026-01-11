// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TinyCsvParser.Mapping;
using TinyCsvParser.Tokenizer;

namespace TinyCsvParser.Test.Integration;

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

            var isInQuotes = false;

            var chars = input.ToCharArray();

            var str = new StringBuilder(string.Empty);

            foreach (var t in chars)
            {
                switch (t)
                {
                    case '"':
                        isInQuotes = !isInQuotes;
                        break;
                    case ',' when !isInQuotes:
                        result.Add(str.ToString());

                        str.Clear();
                        break;
                    default:
                        str.Append(t);
                        break;
                }
            }

            result.Add(str.ToString());

            return result.ToArray();
        }
    }

    [Test]
    public void RunTest()
    {
        var options = new CsvParserOptions(false, new StringSplitTokenizer([','], false));
        var mapping = new TestModelMapping();
        var parser = new CsvParser<TestModel>(options, mapping);

        var testFilePath = GetTestFilePath();

        MeasurementUtils.MeasureElapsedTime("Reading 100 000 Lines", () =>
        {
            var lines = parser
                .ReadFromFile(testFilePath, Encoding.UTF8)
                .Count(x => x.IsValid);

            Assert.AreEqual(100000, lines);
        });
    }

    [SetUp]
    public void SetUp()
    {
        var stringBuilder = new StringBuilder();

        for (var i = 0; i < 100000; i++)
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

    private static string GetTestFilePath()
    {
        return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test_file.txt");
    }
}
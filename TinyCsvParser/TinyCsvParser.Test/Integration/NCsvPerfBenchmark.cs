using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyCsvParser.Mapping;
using TinyCsvParser.Tokenizer;
using TinyCsvParser.Tokenizer.RFC4180;

namespace TinyCsvParser.Test.Integration
{
    [TestFixture]
    [Explicit("Example for https://github.com/joelverhagen/NCsvPerf/issues/2")]
    public class NCsvPerfBenchmark
    {

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

        private class TestModel
        {
            public string Id { get; set; }

            public DateTime LastCrawled { get; set; }

            public string Project { get; set; }

            public string ProjectVersion { get; set; }

            public DateTime LastUpdate { get; set; }

            public string Assets { get; set; }

            public string RuntimeAssemblies { get; set; }

            public string Placeholder1 { get; set; }

            public string Platform { get; set; }

            public string Runtime { get; set; }

            public string Placeholder2 { get; set; }

            public string Placeholder3 { get; set; }

            public string Placeholder4 { get; set; }

            public string Placeholder5 { get; set; }

            public string Placeholder6 { get; set; }

            public string Placeholder7 { get; set; }

            public string Placeholder8 { get; set; }

            public string Filename1 { get; set; }

            public string Filename2 { get; set; }

            public string Extension { get; set; }

            public string Type { get; set; }

            public string Target1 { get; set; }

            public string Target2 { get; set; }

            public string RuntimeVersion { get; set; }

            public string Version { get; set; }
        }

        private class TestModelMapping : CsvMapping<TestModel>
        {
            /// <summary>
            /// {CsvMappingError (ColumnIndex = 25, Value = Column 25 is Out Of Range, UnmappedRow = 75fcf875-017d-4579-bfd9-791d3e6767f0|2020-11-28T01:50:41.2449947+00:00|Akinzekeel.BlazorGrid|0.9.1-preview|2020-11-27T22:42:54.3100000+00:00|AvailableAssets|RuntimeAssemblies|||net5.0||||||lib/net5.0/BlazorGrid.dll|BlazorGrid.dll|.dll|lib|net5.0|.NETCoreApp|5.0.0.0|||0.0.0.0)}
            /// </summary>
            public TestModelMapping()
            {
                MapProperty(0, x => x.Id);
                MapProperty(1, x => x.LastCrawled);
                MapProperty(2, x => x.Project);
                MapProperty(3, x => x.ProjectVersion);
                MapProperty(4, x => x.LastUpdate);
                MapProperty(5, x => x.Assets);
                MapProperty(6, x => x.RuntimeAssemblies);
                MapProperty(7, x => x.Placeholder1);
                MapProperty(8, x => x.Platform);
                MapProperty(9, x => x.Runtime);
                MapProperty(10, x => x.Placeholder2);
                MapProperty(11, x => x.Placeholder3);
                MapProperty(12, x => x.Placeholder4);
                MapProperty(13, x => x.Placeholder5);
                MapProperty(14, x => x.Placeholder6);
                MapProperty(15, x => x.Filename1);
                MapProperty(16, x => x.Filename2);
                MapProperty(17, x => x.Extension);
                MapProperty(18, x => x.Type);
                MapProperty(19, x => x.Target1);
                MapProperty(20, x => x.Target2);
                MapProperty(21, x => x.RuntimeVersion);
                MapProperty(22, x => x.Placeholder7);
                MapProperty(23, x => x.Placeholder8);
                MapProperty(24, x => x.Version);
            }
        }

        [Test]
        public void RunStringSplitTokenizerTest()
        {
            var options = new CsvParserOptions(false, new CustomTokenizer());
            var mapping = new TestModelMapping();
            var parser = new CsvParser<TestModel>(options, mapping);

            string filename = GetTestFilePath();

            MeasurementUtils.MeasureElapsedTime(
                description: $"Reading {filename} ...",
                action: () =>
                {
                    var cnt = parser
                        .ReadFromFile(filename, Encoding.UTF8)
                        .Where(x => x.IsValid)
                        .Count();
    
                    TestContext.WriteLine($"Parsed {cnt} valid lines ...");
                },
                timespanFormatter: x => $"{x.TotalMilliseconds} Milliseconds");
        }

        [Test]
        public void RunCustomTokenizerTest()
        {
            var options = new CsvParserOptions(false, new CustomTokenizer());
            var mapping = new TestModelMapping();
            var parser = new CsvParser<TestModel>(options, mapping);

            string filename = GetTestFilePath();

            MeasurementUtils.MeasureElapsedTime(
                description: $"Reading {filename} ...",
                action: () =>
                {
                    var cnt = parser
                        .ReadFromFile(filename, Encoding.UTF8)
                        .Where(x => x.IsValid)
                        .Count();

                    TestContext.WriteLine($"Parsed {cnt} valid lines ...");
                },
                timespanFormatter: x => $"{x.TotalMilliseconds} Milliseconds");
        }

        [Test]
        public void RunRfc4180TokenizerTest()
        {
            var options = new CsvParserOptions(false, new RFC4180Tokenizer(new Options('"', '\\', ',')));
            var mapping = new TestModelMapping();
            var parser = new CsvParser<TestModel>(options, mapping);

            string filename = GetTestFilePath();

            MeasurementUtils.MeasureElapsedTime(
                description: $"Reading {filename} ...",
                action: () =>
                {
                    var cnt = parser
                        .ReadFromFile(filename, Encoding.UTF8)
                        .Where(x => x.IsValid)
                        .Count();

                    TestContext.WriteLine($"Parsed {cnt} valid lines ...");
                },
                timespanFormatter: x => $"{x.TotalMilliseconds} Milliseconds");
        }


        [SetUp]
        public void SetUp()
        {
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < 1_000_000; i++)
            {
                stringBuilder.AppendLine("75fcf875-017d-4579-bfd9-791d3e6767f0,2020-11-28T01:50:41.2449947+00:00,Akinzekeel.BlazorGrid,0.9.1-preview,2020-11-27T22:42:54.3100000+00:00,AvailableAssets,ResourceAssemblies,,,net5.0,,,,,,lib/net5.0/de/BlazorGrid.resources.dll,BlazorGrid.resources.dll,.dll,lib,net5.0,.NETCoreApp,5.0.0.0,,,0.0.0.0");
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

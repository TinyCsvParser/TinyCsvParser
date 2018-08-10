using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using CoreCsvParser.Mapping;
using CoreCsvParser.TypeConverter;

namespace CoreCsvParser.Benchmark
{
    public class LocalWeatherData : IEquatable<LocalWeatherData>
    {
        public int RowNum { get; set; }
        public string WBAN { get; set; }
        public DateTime Date { get; set; }
        public string SkyCondition { get; set; }

        public bool Equals(LocalWeatherData other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return RowNum == other.RowNum
                && WBAN == other.WBAN
                && Date == other.Date
                && SkyCondition == other.SkyCondition;
        }

        public override bool Equals(object obj) => Equals(obj as LocalWeatherData);

        public override int GetHashCode()
        {
            return 17 ^ RowNum.GetHashCode() ^ (WBAN?.GetHashCode() ?? 0) ^ Date.GetHashCode() ^ (SkyCondition?.GetHashCode() ?? 0);
        }

        public override string ToString()
        {
            return $"{RowNum}: {Date}, {WBAN}, {SkyCondition}";
        }
    }

    public class LocalWeatherDataMapper : CsvMapping<LocalWeatherData>
    {
        public LocalWeatherDataMapper()
        {
            MapProperty(0, x => x.RowNum);
            MapProperty(1, x => x.WBAN);
            MapProperty(2, x => x.Date, new DateTimeConverter("yyyyMMdd"));
            MapProperty(5, x => x.SkyCondition);
        }
    }

    [MemoryDiagnoser]
    public class CsvBenchmark
    {
        [Benchmark]
        public void LocalWeatherRead_One_Core()
        {
            var csvParserOptions = new CsvParserOptions(true, ',', 1, true);
            var csvMapper = new LocalWeatherDataMapper();
            var csvParser = new CsvParser<LocalWeatherData>(csvParserOptions, csvMapper);

            var a = csvParser
                .ReadFromFile(@"C:\Temp\201503hourly.txt", Encoding.ASCII)
                .ToList();
        }

        [Benchmark]
        public void LocalWeatherRead_4_Cores()
        {
            var csvParserOptions = new CsvParserOptions(true, ',', 4, true);
            var csvMapper = new LocalWeatherDataMapper();
            var csvParser = new CsvParser<LocalWeatherData>(csvParserOptions, csvMapper);

            var a = csvParser
                .ReadFromFile(@"C:\Temp\201503hourly.txt", Encoding.ASCII)
                .ToList();
        }

        [Benchmark]
        public void LocalWeatherPipeline()
        {
            // TODO: This method is missing some lines!
            // LocalWeatherRead_One_Core: Parsed 4,496,262 lines.
            // LocalWeatherPipeline:      Parsed 4,441,695 lines.

            var csvParserOptions = new CsvParserOptions(true, ',', 4, true);
            var csvMapper = new LocalWeatherDataMapper();
            var csvParser = new CsvParser<LocalWeatherData>(csvParserOptions, csvMapper);

            var items = new List<LocalWeatherData>();
            var observable = csvParser.ObserveFromFile(@"C:\Temp\201503hourly.txt", Encoding.ASCII);
            observable.Subscribe(items.Add, ex => throw ex);
            observable.ToTask().Wait();
            //Console.WriteLine($"Parsed {items.Count:N0} lines.");
        }

        public static void CompareLoaders()
        {
            // use this in C# interactive to create subsets of the big file with a specific number of rows
            //void SubsetFile(string baseFilePath, int rows)
            //{
            //    var lines = System.IO.File.ReadLines(baseFilePath)
            //        .Take(rows + 1)
            //        .Select((ln, i) => i == 0 ? "RowNum," + ln : $"{i},{ln}");

            //    var path = System.IO.Path.GetDirectoryName(baseFilePath);
            //    var fileName = System.IO.Path.GetFileNameWithoutExtension(baseFilePath) + "-first-" + rows;
            //    var ext = System.IO.Path.GetExtension(baseFilePath);

            //    var newPath = System.IO.Path.Combine(path, fileName) + ext;

            //    System.IO.File.WriteAllLines(newPath, lines);
            //}

            Console.WriteLine("Starting direct file read...");
            var csvParserOptions = new CsvParserOptions(true, ',', 1, true);
            var csvMapper = new LocalWeatherDataMapper();
            var csvParser = new CsvParser<LocalWeatherData>(csvParserOptions, csvMapper);

            var read = csvParser.ReadFromFile(@"C:\Temp\201503hourly-first-1000.txt", Encoding.ASCII)
                .Select(x =>
                {
                    if (x.IsValid)
                        return x.Result;
                    else
                        throw x.Error;
                });

            var readItems = new List<LocalWeatherData>(read);
            Console.WriteLine($"Read {readItems.Count:N0} lines.");
            Console.WriteLine("Starting Pipeline file read...");

            var pipedItems = new List<LocalWeatherData>(readItems.Count);
            var observable = csvParser.ObserveFromFile(@"C:\Temp\201503hourly-first-1000.txt", Encoding.ASCII);
            observable.Subscribe(x => { pipedItems.Add(x); }, ex => Console.WriteLine(ex.Message));
            observable.ToTask().Wait();
            Console.WriteLine($"Piped {pipedItems.Count:N0} lines.");

            if (readItems.Count > pipedItems.Count)
            {
                Console.WriteLine($"Items missing: {readItems.Count - pipedItems.Count:N0}.");
                foreach (var item in readItems.Except(pipedItems))
                {
                    Console.WriteLine(item.ToString());
                }
            }
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            //var summary = BenchmarkRunner.Run<CsvBenchmark>();
            CsvBenchmark.CompareLoaders();
        }
    }
}

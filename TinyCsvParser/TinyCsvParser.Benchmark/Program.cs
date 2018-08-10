using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using TinyCsvParser.Mapping;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.Benchmark
{
    public class LocalWeatherData : IEquatable<LocalWeatherData>
    {
        public string WBAN { get; set; }
        public DateTime Date { get; set; }
        public string SkyCondition { get; set; }

        public bool Equals(LocalWeatherData other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return WBAN == other.WBAN
                && Date == other.Date
                && SkyCondition == other.SkyCondition;
        }

        public override bool Equals(object obj) => Equals(obj as LocalWeatherData);

        public override int GetHashCode()
        {
            return 17 ^ (WBAN?.GetHashCode() ?? 0) ^ Date.GetHashCode() ^ (SkyCondition?.GetHashCode() ?? 0);
        }

        public override string ToString()
        {
            return $"{Date}: {WBAN}, {SkyCondition}";
        }
    }

    public class LocalWeatherDataMapper : CsvMapping<LocalWeatherData>
    {
        public LocalWeatherDataMapper()
        {
            MapProperty(0, x => x.WBAN);
            MapProperty(1, x => x.Date, new DateTimeConverter("yyyyMMdd"));
            MapProperty(4, x => x.SkyCondition);
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
            var csvParserOptions = new CsvParserOptions(true, ',', 4, true);
            var csvMapper = new LocalWeatherDataMapper();
            var csvParser = new CsvParser<LocalWeatherData>(csvParserOptions, csvMapper);

            var read = csvParser.ReadFromFile(@"C:\Temp\201503hourly.txt", Encoding.ASCII)
                .Select(x =>
                {
                    if (x.IsValid)
                        return x.Result;
                    else
                        throw x.Error;
                });

            var readSet = new HashSet<LocalWeatherData>(read);
            Console.WriteLine($"Read {readSet.Count:N0} distinct lines.");

            var pipedSet = new HashSet<LocalWeatherData>(readSet.Count);
            var observable = csvParser.ObserveFromFile(@"C:\Temp\201503hourly.txt", Encoding.ASCII);
            observable.Subscribe(x => { pipedSet.Add(x); }, ex => throw ex);
            observable.ToTask().Wait();
            Console.WriteLine($"Piped {pipedSet.Count:N0} distinct lines.");

            if (readSet.Count != pipedSet.Count)
            {
                readSet.SymmetricExceptWith(pipedSet);
                Console.WriteLine($"Items not in both sets: {readSet.Count:N0}.");
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

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Linq;
using System.Text;
using TinyCsvParser.Mapping;
using TinyCsvParser.TypeConverter;
using System.Reactive;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;

namespace TinyCsvParser.Benchmark
{
    public class LocalWeatherData
    {
        public string WBAN { get; set; }
        public DateTime Date { get; set; }
        public string SkyCondition { get; set; }

        public override string ToString()
        {
            return $"{Date}: {SkyCondition}";
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

            //Console.WriteLine($"Parsed {a.Count:N0} lines.");
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
        public async Task LocalWeatherPipeline()
        {
            // TODO: This method is missing some lines!
            // LocalWeatherRead_One_Core: Parsed 4,496,262 lines.
            // LocalWeatherPipeline:      Parsed 4,441,695 lines.

            var csvParserOptions = new CsvParserOptions(true, ',', 4, true);
            var csvMapper = new LocalWeatherDataMapper();
            var csvParser = new CsvParser<LocalWeatherData>(csvParserOptions, csvMapper);

            int parsedCount = 0;
            var observable = csvParser.ObserveFromFile(@"C:\Temp\201503hourly.txt", Encoding.ASCII);
            observable.Subscribe(x => { parsedCount++; }, ex => Console.Error.WriteLine(ex.ToString()));
            await observable.ToTask();
            //Console.WriteLine($"Parsed {parsedCount:N0} lines.");
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<CsvBenchmark>();
            //var bm = new CsvBenchmark();
            //bm.LocalWeatherRead_One_Core();
            //bm.LocalWeatherPipeline().Wait();
        }
    }
}

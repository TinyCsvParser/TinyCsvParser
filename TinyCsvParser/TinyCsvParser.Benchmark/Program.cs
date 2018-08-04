using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Linq;
using System.Text;
using TinyCsvParser.Mapping;
using TinyCsvParser.TypeConverter;

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
            var csvParserOptions = new CsvParserOptions(true, ',', 4, true);
            var csvMapper = new LocalWeatherDataMapper();
            var csvParser = new CsvParser<LocalWeatherData>(csvParserOptions, csvMapper);

            Piper.ReadFileAsync(@"C:\Temp\201503hourly.txt", Encoding.ASCII, csvParser).Wait();
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            //var summary = BenchmarkRunner.Run<CsvBenchmark>();
            new CsvBenchmark().LocalWeatherPipeline();
        }
    }
}

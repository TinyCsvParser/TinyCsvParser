using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyCsvParser.Benchmark.Mapper;
using TinyCsvParser.Benchmark.Model;

namespace TinyCsvParser.Benchmark
{
    [MemoryDiagnoser]
    public class TinyCsvParserBenchmarks
    {
        [Benchmark]
        public void LocalWeatherRead()
        {
            var csvParserOptions = new CsvParserOptions(true, ',', 4, false);
            var csvMapper = new LocalWeatherDataMapper();
            var csvParser = new CsvParser<LocalWeatherData>(csvParserOptions, csvMapper);

            float result = csvParser.ReadFromFile(@"G:\Datasets\QCLCD\201503hourly.txt", Encoding.ASCII)
                .Where(x => x.IsValid)
                .Select(x => x.Result)
                .Average(x => x.DryBulbCelsius);

            Console.WriteLine($"Average Temperature {result}");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<TinyCsvParserBenchmarks>();
        }
    }
}

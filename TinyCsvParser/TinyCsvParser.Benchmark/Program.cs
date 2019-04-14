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
        public async Task LocalWeatherReadAsync()
        {
            var csvParserOptions = new CsvParserOptions(true, ',');
            var csvMapper = new LocalWeatherDataMapper();
            var csvParser = new CsvParser<LocalWeatherData>(csvParserOptions, csvMapper);

            float result = await csvParser.ReadFromFileAsync(@"G:\Datasets\QCLCD\201503hourly.txt", Encoding.ASCII)
                .Where(x => x.IsValid)
                .Select(x => x.Result)
                .AverageAsync(x => x.DryBulbCelsius);

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

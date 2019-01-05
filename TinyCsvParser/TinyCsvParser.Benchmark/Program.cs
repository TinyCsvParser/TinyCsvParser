using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using TinyCsvParser.Mapping;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.Benchmark
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
            MapProperty(0, x => x.WBAN);
            MapProperty(1, x => x.Date, new DateTimeConverter("yyyyMMdd"));
            MapProperty(4, x => x.SkyCondition);
        }
    }

    public class LocalWeatherDataMapperWithRowNum : CsvMapping<LocalWeatherData>
    {
        public LocalWeatherDataMapperWithRowNum()
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
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<CsvBenchmark>();
        }
    }
}

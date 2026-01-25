// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using System.IO;
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
        [GlobalSetup]
        public void SetupBenchmarkData()
        {
            const string csvHeader =
                "WBAN,Date,Time,StationType,SkyCondition,SkyConditionFlag,Visibility,VisibilityFlag,WeatherType,WeatherTypeFlag,DryBulbFarenheit,DryBulbFarenheitFlag,DryBulbCelsius,DryBulbCelsiusFlag,WetBulbFarenheit,WetBulbFarenheitFlag,WetBulbCelsius,WetBulbCelsiusFlag,DewPointFarenheit,DewPointFarenheitFlag,DewPointCelsius,DewPointCelsiusFlag,RelativeHumidity,RelativeHumidityFlag,WindSpeed,WindSpeedFlag,WindDirection,WindDirectionFlag,ValueForWindCharacter,ValueForWindCharacterFlag,StationPressure,StationPressureFlag,PressureTendency,PressureTendencyFlag,PressureChange,PressureChangeFlag,SeaLevelPressure,SeaLevelPressureFlag,RecordType,RecordTypeFlag,HourlyPrecip,HourlyPrecipFlag,Altimeter,AltimeterFlag";
            const string csvLine =
                "00102,20150302,1201,0,BKN100, ,10.00, , , ,14, ,-10.0, ,12, ,-11.3, ,3, ,-16.1, , 61, ,11, ,100, ,21, ,29.95, , , , , ,30.26, ,AA, , , ,30.13,";

            const int numLinesToGenerate = 4496263;

            var testFilePath = GetTestFilePath();

            using (var fileStream = File.Create(testFilePath))
            {
                using (var streamWriter = new StreamWriter(fileStream, Encoding.ASCII))
                {
                    streamWriter.WriteLine(csvHeader);

                    for (int i = 0; i < numLinesToGenerate; i++)
                    {
                        streamWriter.WriteLine(csvLine);
                    }
                }
            }
        }

        [Benchmark]
        public void LocalWeatherRead()
        {
            var csvParserOptions = new CsvParserOptions(true, ',', 4, false);
            var csvMapper = new LocalWeatherDataMapper();
            var csvParser = new CsvParser<LocalWeatherData>(csvParserOptions, csvMapper);

            float result = csvParser.ReadFromFile(GetTestFilePath(), Encoding.ASCII)
                .Where(x => x.IsValid)
                .Select(x => x.Result)
                .Average(x => x.DryBulbCelsius);

            Console.WriteLine($"Average Temperature {result}");
        }

        [GlobalCleanup]
        public void CleanupBenchmarkData()
        {
            var testFilePath = GetTestFilePath();

            File.Delete(testFilePath);
        }

        private string GetTestFilePath()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test_file.txt");
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
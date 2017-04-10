// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using System.Text;
using TinyCsvParser.Mapping;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.Test.Benchmarks
{

  [TestFixture, Explicit, Description("Requires weather data from https://www.ncdc.noaa.gov/orders/qclcd/QCLCD201503.zip")]
  public class LocalWeatherDataBenchmark
  {
    public class LocalWeatherData
    {
      public string WBAN { get; set; }
      public DateTime Date { get; set; }
      public string SkyCondition { get; set; }
    }

    public class LocalWeatherDataMapper : CsvMapping<LocalWeatherData>
    {
      public LocalWeatherDataMapper()
      {
        MapProperty(0, x => x.WBAN);
        MapProperty(1, x => x.Date).WithCustomConverter(new DateTimeConverter("yyyyMMdd"));
        MapProperty(4, x => x.SkyCondition);
      }
    }

    [Test, Explicit("Performance Test for a Sequential Read")]
    public void SeqReadTest()
    {
      MeasurementUtils.MeasureElapsedTime(string.Format("Sequential Read"), () =>
      {
        var a = File.ReadLines(@"c:\filepathtoweatherdata\201503hourly.txt", Encoding.ASCII)
                  .AsParallel()
                  .Where(line => !string.IsNullOrWhiteSpace(line))
                  .Select(line => line.Trim().Split(new[] { ';' })).ToList();
      });
    }


    [Test, Explicit("Performance test with a large dataset with Parallel Options")]
    public void LocalWeatherReadTest()
    {
      bool[] keepOrder = new bool[] { true, false };
      int[] degreeOfParallelismList = new[] { 4, 3, 2, 1 };

      foreach (var order in keepOrder)
      {
        foreach (var degreeOfParallelism in degreeOfParallelismList)
        {
          CsvParserOptions csvParserOptions = new CsvParserOptions(true, new[] { ',' }, degreeOfParallelism, order);
          LocalWeatherDataMapper csvMapper = new LocalWeatherDataMapper();
          CsvParser<LocalWeatherData> csvParser = new CsvParser<LocalWeatherData>(csvParserOptions, csvMapper);

          MeasurementUtils.MeasureElapsedTime(string.Format("LocalWeather (DegreeOfParallelism = {0}, KeepOrder = {1})", degreeOfParallelism, order),
                () =>
                {
                  var a = csvParser
                        .ReadFromFile(@"c:\filepathtoweatherdata\201503hourly.txt", Encoding.ASCII)
                        .ToList();
                });
        }
      }
    }

  }
}
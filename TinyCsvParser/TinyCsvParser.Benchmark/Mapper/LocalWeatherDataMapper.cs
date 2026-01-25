// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using TinyCsvParser.Benchmark.Model;
using TinyCsvParser.Core;

namespace TinyCsvParser.Benchmark.Mapper
{
    public class LocalWeatherDataMapper : CsvMapping<LocalWeatherData>
    {
        public LocalWeatherDataMapper()
        {
            MapProperty(0, x => x.WBAN);
            MapProperty(1, x => x.Date, new DateTimeConverter("yyyyMMdd"));
            MapProperty(2, x => x.Time, new TimeSpanConverter("hhmm"));
            MapProperty(4, x => x.SkyCondition);
            MapProperty(12, x => x.DryBulbCelsius);
            MapProperty(24, x => x.WindSpeed);
            MapProperty(30, x => x.StationPressure);
        }
    }
}

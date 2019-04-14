using System;

namespace TinyCsvParser.Benchmark.Model
{
    public class LocalWeatherData
    {
        public string WBAN { get; set; }

        public DateTime Date { get; set; }

        public TimeSpan Time { get; set; }

        public string SkyCondition { get; set; }

        public float DryBulbCelsius { get; set; }

        public float WindSpeed { get; set; }

        public float StationPressure { get; set; }
    }
}

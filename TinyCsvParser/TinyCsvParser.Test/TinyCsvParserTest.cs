using NUnit.Framework;
using System;
using System.Linq;
using System.Globalization;
using System.Text;
using TinyCsvParser;
using TinyCsvParser.Mapping;
using TinyCsvParser.TypeConverter;
using System.Diagnostics;
using System.IO;
using System.Collections.Concurrent;

namespace TinyCsvParser.Test
{
    [TestFixture]
    public class TinyCsvParserTest
    {
        private class Person
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public DateTime BirthDate { get; set; }
        }

        private class CsvPersonMapping : CsvMapping<Person>
        {
            public CsvPersonMapping()
            {
                MapProperty(0, x => x.FirstName);
                MapProperty(1, x => x.LastName);
                MapProperty(2, x => x.BirthDate);
            }
        }

        private class CsvPersonMappingWithTypeConverterProvider : CsvMapping<Person>
        {
            public CsvPersonMappingWithTypeConverterProvider()
                : this(new TypeConverterProvider())
            {
            }
            public CsvPersonMappingWithTypeConverterProvider(ITypeConverterProvider typeConverterProvider)
                : base(typeConverterProvider)
            {
                MapProperty(0, x => x.FirstName);
                MapProperty(1, x => x.LastName);
                MapProperty(2, x => x.BirthDate);
            }
        }

        private class CsvPersonMappingWithCustomConverter : CsvMapping<Person>
        {
            public CsvPersonMappingWithCustomConverter()
            {
                MapProperty(0, x => x.FirstName);
                MapProperty(1, x => x.LastName);
                MapProperty(2, x => x.BirthDate)
                    .WithCustomConverter(new DateTimeConverter("yyyy###MM###dd"));
            }
        }

        [Test]
        public void TinyCsvTest()
        {
            CsvParserOptions csvParserOptions = new CsvParserOptions(true, new[] { ';' });
            CsvReaderOptions csvReaderOptions = new CsvReaderOptions(new[] { Environment.NewLine });
            CsvPersonMapping csvMapper = new CsvPersonMapping();
            CsvParser<Person> csvParser = new CsvParser<Person>(csvParserOptions, csvMapper);

            var stringBuilder = new StringBuilder()
                .AppendLine("FirstName;LastName;BirthDate")
                .AppendLine("Philipp;Wagner;1986/05/12")
                .AppendLine("Max;Mustermann;2014/01/01");

            var result = csvParser
                .ReadFromString(csvReaderOptions, stringBuilder.ToString())
                .ToList();

            Assert.AreEqual(2, result.Count);

            Assert.IsTrue(result.All(x => x.IsValid));

            // Asserts ...
        }

        public class NegativeValueEntity
        {
            public Double Value { get; set; }
        }

        private class NegativeValueEntityMapping : CsvMapping<NegativeValueEntity>
        {
            public NegativeValueEntityMapping()
            {
                MapProperty(0, x => x.Value);
            }
        }

        [Test]
        public void NegativeValueTest()
        {
            CsvParserOptions csvParserOptions = new CsvParserOptions(true, new[] { ';' });
            CsvReaderOptions csvReaderOptions = new CsvReaderOptions(new[] { Environment.NewLine });
            NegativeValueEntityMapping csvMapper = new NegativeValueEntityMapping();
            CsvParser<NegativeValueEntity> csvParser = new CsvParser<NegativeValueEntity>(csvParserOptions, csvMapper);

            var stringBuilder = new StringBuilder()
                .AppendLine("Value")
                .AppendLine("-1");

            var result = csvParser
                .ReadFromString(csvReaderOptions, stringBuilder.ToString())
                .ToList();

            Assert.AreEqual(1, result.Count);
            
            Assert.IsTrue(result.All(x => x.IsValid));

            Assert.AreEqual(-1, result.First().Result.Value);

            // Asserts ...
        }

        [Test]
        public void SkipHeaderTest()
        {
            CsvParserOptions csvParserOptions = new CsvParserOptions(true, new[] { ';' });
            CsvReaderOptions csvReaderOptions = new CsvReaderOptions(new[] { Environment.NewLine });
            CsvPersonMapping csvMapper = new CsvPersonMapping();
            CsvParser<Person> csvParser = new CsvParser<Person>(csvParserOptions, csvMapper);

            var stringBuilder = new StringBuilder()
                .AppendLine("FirstName;LastName;BirthDate")
                .AppendLine("Philipp;Wagner;1986/05/12")
                .AppendLine("Max;Mustermann;2014/01/01");

            var result = csvParser
                .ReadFromString(csvReaderOptions, stringBuilder.ToString())
                .ToList();

            Assert.AreEqual(2, result.Count);

            Assert.IsTrue(result.All(x => x.IsValid));

            Assert.AreEqual("Philipp", result[0].Result.FirstName);
            Assert.AreEqual("Wagner", result[0].Result.LastName);

            Assert.AreEqual(1986, result[0].Result.BirthDate.Year);
            Assert.AreEqual(5, result[0].Result.BirthDate.Month);
            Assert.AreEqual(12, result[0].Result.BirthDate.Day);

            Assert.AreEqual("Max", result[1].Result.FirstName);
            Assert.AreEqual("Mustermann", result[1].Result.LastName);

            Assert.AreEqual(2014, result[1].Result.BirthDate.Year);
            Assert.AreEqual(1, result[1].Result.BirthDate.Month);
            Assert.AreEqual(1, result[1].Result.BirthDate.Day);
        }

        [Test]
        public void EmptyDataTest()
        {
            CsvParserOptions csvParserOptions = new CsvParserOptions(true, new[] { ';' });
            CsvReaderOptions csvReaderOptions = new CsvReaderOptions(new[] { Environment.NewLine });

            CsvPersonMapping csvMapper = new CsvPersonMapping();
            CsvParser<Person> csvParser = new CsvParser<Person>(csvParserOptions, csvMapper);

            var stringBuilder = new StringBuilder()
                .AppendLine("FirstName;LastName;BirthDate");

            var result = csvParser
                .ReadFromString(csvReaderOptions, stringBuilder.ToString())
                .ToList();

            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void TrimLineTest()
        {
            CsvParserOptions csvParserOptions = new CsvParserOptions(true, new[] { ';' }, 1, true);
            CsvReaderOptions csvReaderOptions = new CsvReaderOptions(new[] { Environment.NewLine });
            CsvPersonMapping csvMapper = new CsvPersonMapping();
            CsvParser<Person> csvParser = new CsvParser<Person>(csvParserOptions, csvMapper);

            var stringBuilder = new StringBuilder()
                .AppendLine("FirstName;LastName;BirthDate")
                .AppendLine("     Philipp;Wagner;1986/05/12       ")
                .AppendLine("Max;Mustermann;2014/01/01");

            var result = csvParser
                .ReadFromString(csvReaderOptions, stringBuilder.ToString())
                .ToList();

            Assert.AreEqual(2, result.Count);

            Assert.IsTrue(result.All(x => x.IsValid));

            Assert.AreEqual("Philipp", result[0].Result.FirstName);
            Assert.AreEqual("Wagner", result[0].Result.LastName);

            Assert.AreEqual(1986, result[0].Result.BirthDate.Year);
            Assert.AreEqual(5, result[0].Result.BirthDate.Month);
            Assert.AreEqual(12, result[0].Result.BirthDate.Day);

            Assert.AreEqual("Max", result[1].Result.FirstName);
            Assert.AreEqual("Mustermann", result[1].Result.LastName);
            Assert.AreEqual(2014, result[1].Result.BirthDate.Year);
            Assert.AreEqual(1, result[1].Result.BirthDate.Month);
            Assert.AreEqual(1, result[1].Result.BirthDate.Day);
        }

        [Test]
        public void WeirdDateTimeTest_ProviderBased()
        {
            // Instantiate the TypeConverterProvider and override the DateTimeConverter:
            ITypeConverterProvider csvTypeConverterProvider =
                new TypeConverterProvider().Add(new DateTimeConverter("yyyy###MM###dd"));

            // Make sure you pass the custom provider into the mapping!
            var csvMapper = new CsvPersonMappingWithTypeConverterProvider(csvTypeConverterProvider);

            // And this is business as usual!
            CsvParserOptions csvParserOptions = new CsvParserOptions(true, new[] { ';' });
            CsvReaderOptions csvReaderOptions = new CsvReaderOptions(new[] { Environment.NewLine });
            CsvParser<Person> csvParser = new CsvParser<Person>(csvParserOptions, csvMapper);

            var stringBuilder = new StringBuilder()
                .AppendLine("FirstName;LastName;BirthDate")
                .AppendLine("Philipp;Wagner;1986###05###12");

            var result = csvParser
                .ReadFromString(csvReaderOptions, stringBuilder.ToString())
                .ToList();

            Assert.AreEqual("Philipp", result[0].Result.FirstName);
            Assert.AreEqual("Wagner", result[0].Result.LastName);

            Assert.AreEqual(1986, result[0].Result.BirthDate.Year);
            Assert.AreEqual(5, result[0].Result.BirthDate.Month);
            Assert.AreEqual(12, result[0].Result.BirthDate.Day);
        }

        [Test]
        public void WeirdDateTimeTest_CustomConverterBased()
        {
            CsvParserOptions csvParserOptions = new CsvParserOptions(true, new[] { ';' });
            CsvReaderOptions csvReaderOptions = new CsvReaderOptions(new[] { Environment.NewLine });
            CsvPersonMappingWithCustomConverter csvMapper = new CsvPersonMappingWithCustomConverter();
            CsvParser<Person> csvParser = new CsvParser<Person>(csvParserOptions, csvMapper);

            var stringBuilder = new StringBuilder()
                .AppendLine("FirstName;LastName;BirthDate")
                .AppendLine("Philipp;Wagner;1986###05###12");

            var result = csvParser
                .ReadFromString(csvReaderOptions, stringBuilder.ToString())
                .ToList();

            Assert.AreEqual("Philipp", result[0].Result.FirstName);
            Assert.AreEqual("Wagner", result[0].Result.LastName);

            Assert.AreEqual(1986, result[0].Result.BirthDate.Year);
            Assert.AreEqual(5, result[0].Result.BirthDate.Month);
            Assert.AreEqual(12, result[0].Result.BirthDate.Day);
        }
        
        [Test]
        public void ParallelLinqTest()
        {
            CsvParserOptions csvParserOptions = new CsvParserOptions(true, new[] { ';' }, 3, true);
            CsvReaderOptions csvReaderOptions = new CsvReaderOptions(new[] { Environment.NewLine });
            CsvPersonMapping csvMapper = new CsvPersonMapping();
            CsvParser<Person> csvParser = new CsvParser<Person>(csvParserOptions, csvMapper);

            var stringBuilder = new StringBuilder()
                .AppendLine("FirstName;LastName;BirthDate")
                .AppendLine("Philipp;Wagner;1986/05/12")
                .AppendLine("Max;Mustermann;2014/01/01");

            var result = csvParser
                .ReadFromString(csvReaderOptions, stringBuilder.ToString())
                .Where(x => x.IsValid)
                .Where(x => x.Result.FirstName == "Philipp")
                .ToList();

            Assert.AreEqual(1, result.Count);

            Assert.AreEqual("Philipp", result[0].Result.FirstName);
            Assert.AreEqual("Wagner", result[0].Result.LastName);

            Assert.AreEqual(1986, result[0].Result.BirthDate.Year);
            Assert.AreEqual(5, result[0].Result.BirthDate.Month);
            Assert.AreEqual(12, result[0].Result.BirthDate.Day);
        }

        [Test, Explicit("Performance Test creating a string with 1000000 lines")]
        public void DegreeOfParallelismTest()
        {
            int csvDataLines = 1000000;
            int[] degreeOfParallelismList = new[] { 1, 2, 4 };

            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < csvDataLines; i++)
            {
                stringBuilder.AppendLine("Philipp;Wagner;1986/05/12");
            }
            var csvData = stringBuilder.ToString();

            foreach (var degreeOfParallelism in degreeOfParallelismList)
            {
                CsvParserOptions csvParserOptions = new CsvParserOptions(true, new[] { ';' }, degreeOfParallelism, true);
                CsvReaderOptions csvReaderOptions = new CsvReaderOptions(new[] { Environment.NewLine });
                CsvPersonMapping csvMapper = new CsvPersonMapping();
                CsvParser<Person> csvParser = new CsvParser<Person>(csvParserOptions, csvMapper);

                MeasureElapsedTime(string.Format("DegreeOfParallelismTest (DegreeOfParallelism = {0})", degreeOfParallelism), () => csvParser.ReadFromString(csvReaderOptions, csvData).ToList());
            }
        }

        private void MeasureElapsedTime(string description, Action action)
        {
            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = MeasureElapsedTime(action);

            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);

            Console.WriteLine("[{0}] Elapsed Time = {1}", description, elapsedTime);
        }

        private TimeSpan MeasureElapsedTime(Action action)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            action();
            stopWatch.Stop();

            return stopWatch.Elapsed;
        }

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
            MeasureElapsedTime(string.Format("Sequential Read"), () =>
            {
                var a = File.ReadLines(@"C:\Users\philipp\Downloads\csv\201503hourly.txt", Encoding.ASCII)
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
                    CsvReaderOptions csvReaderOptions = new CsvReaderOptions(new[] { Environment.NewLine });
                    LocalWeatherDataMapper csvMapper = new LocalWeatherDataMapper();
                    CsvParser<LocalWeatherData> csvParser = new CsvParser<LocalWeatherData>(csvParserOptions, csvMapper);

                    MeasureElapsedTime(string.Format("LocalWeather (DegreeOfParallelism = {0}, KeepOrder = {1})", degreeOfParallelism, order),
                        () =>
                        {
                            var a = csvParser
                                .ReadFromFile(@"C:\Users\philipp\Downloads\csv\201503hourly.txt", Encoding.ASCII)
                                .ToList();
                        });
                }
            }
        }
    }
}

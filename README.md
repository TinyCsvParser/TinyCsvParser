# TinyCsvParser [![NuGet Version](http://img.shields.io/nuget/v/TinyCsvParser.svg?style=flat)](https://www.nuget.org/packages/TinyCsvParser/) [![NuGet Downloads](http://img.shields.io/nuget/dt/TinyCsvParser.svg?style=flat)](https://www.nuget.org/packages/TinyCsvParser/) #

[TinyCsvParser]: https://github.com/bytefish/TinyCsvParser
[MIT License]: https://opensource.org/licenses/MIT

[TinyCsvParser] is an easy to use and high-performing library for CSV parsing in C#.

I have released it under terms of the [MIT License]:

* [https://github.com/bytefish/TinyCsvParser](https://github.com/bytefish/TinyCsvParser)

## Installing TinyCsvParser ##

You can use [NuGet](https://www.nuget.org) to install [TinyCsvParser]. Run the following command 
in the [Package Manager Console](http://docs.nuget.org/consume/package-manager-console).

```
PM> Install-Package TinyCsvParser
```

## Blog Posts ##

There are several blog posts on using [TinyCsvParser]:

* [TinyCsvParser - Parsing CSV Data with C#](http://bytefish.de/blog/tinycsvparser/)
* [Using TinyCsvParser and FluentValidation](http://bytefish.de/blog/fluent_validation/)
* [Benchmarking TinyCsvParser](http://bytefish.de/blog/tinycsvparser_benchmark/)
* [PostgreSQL and TinyCsvParser](http://bytefish.de/blog/tinycsvparser_postgresql/)

## Basic Usage ##

Imagine we have list of Persons in a CSV file with their first name, last name and birthdate.

```
Philipp;Wagner;1986/05/12
Max;Musterman;2014/01/02
```

The corresponding domain model in our system might look like this.

```csharp
private class Person
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime BirthDate { get; set; }
}
```

When using [TinyCsvParser] you have to define the mapping between the columns in the CSV data and the property in you domain model.

```csharp
private class CsvPersonMapping : CsvMapping<Person>
{
    public CsvPersonMapping()
        : base()
    {
        MapProperty(0, x => x.FirstName);
        MapProperty(1, x => x.LastName);
        MapProperty(2, x => x.BirthDate);
    }
}
```

And then we can use the mapping to parse the CSV data with a ``CsvParser``.

```csharp
namespace TinyCsvParser.Test
{
    [TestFixture]
    public class TinyCsvParserTest
    {
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
    }
}
```
   
And that's it! The options in this example are set to skip the header, use ``Environment.NewLine`` as line separator 
and ``;`` as column delimiter.

## Advanced Usage (File Reading, Parallel Processing) ##

### Dataset ###

In this example we are parsing a real life dataset. It's the local weather data in March 2015 gathered by 
all weather stations in the USA. You can obtain the data  ``QCLCD201503.zip`` from:
 
* [http://www.ncdc.noaa.gov/orders/qclcd](http://www.ncdc.noaa.gov/orders/qclcd)

The File size is ``557 MB`` and it has ``4,496,262`` lines.

### Benchmark Results ###

Without further explanation, here are the Benchmark results for parsing the dataset.

```
[TinyCsvParser (DegreeOfParallelism = 4, KeepOrder = True)] Elapsed Time = 00:00:10.48
[CsvHelper] Elapsed Time = 00:00:32.60
[FileHelpers] Crash
```

You can see, that [TinyCsvParser] is able to parse the file in ``10.5`` seconds only. Even if you don't 
process the data in parallel (``DegreeOfParallelism = 1``) it is still faster, than CsvHelper. The 
FileHelpers implementation crashed with an OutOfMemory Exception.

Here are the full benchmark results of [TinyCsvParser]. You can see, that increasing the number of threads 
helps when processing the data. Keeping the order doesn't have impact on the processing time, but it may 
lead to a much higher memory consumption. This may be a subject for a future article.

```
[TinyCsvParser (DegreeOfParallelism = 4, KeepOrder = True)] Elapsed Time = 00:00:10.48
[TinyCsvParser (DegreeOfParallelism = 3, KeepOrder = True)] Elapsed Time = 00:00:10.65
[TinyCsvParser (DegreeOfParallelism = 2, KeepOrder = True)] Elapsed Time = 00:00:12.26
[TinyCsvParser (DegreeOfParallelism = 1, KeepOrder = True)] Elapsed Time = 00:00:17.04
[TinyCsvParser (DegreeOfParallelism = 4, KeepOrder = False)] Elapsed Time = 00:00:10.50
[TinyCsvParser (DegreeOfParallelism = 3, KeepOrder = False)] Elapsed Time = 00:00:10.31
[TinyCsvParser (DegreeOfParallelism = 2, KeepOrder = False)] Elapsed Time = 00:00:11.71
[TinyCsvParser (DegreeOfParallelism = 1, KeepOrder = False)] Elapsed Time = 00:00:16.70
```

### Benchmark Code ###

The elapsed time of the import can be easily measured by using the ``System.Diagnostics.Stopwatch``.

```csharp
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
```

### TinyCsvParser Code ###

```csharp
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

[Test]
public void TinyCsvParserBenchmark()
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

            MeasureElapsedTime(string.Format("TinyCsvParser (DegreeOfParallelism = {0}, KeepOrder = {1})", degreeOfParallelism, order),
                () =>
                {
                    var a = csvParser
                        .ReadFromFile(@"C:\Users\philipp\Downloads\csv\201503hourly.txt", Encoding.ASCII)
                        .ToList();
                });
        }
    }
}
```
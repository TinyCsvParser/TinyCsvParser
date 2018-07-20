.. _benchmark:

Benchmark
=========

.. highlight: csharp

In this section I want to show how you to parse large CSV files with `TinyCsvParser`_ and process them in 
parallel. You will see how fast `TinyCsvParser`_ is compared to other popular .NET libraries for CSV 
parsing. This post is not meant to discredit the `FileHelpers`_ or `CsvHelper`_ projects.

Dataset
~~~~~~~

In this tutorial a real life dataset is parsed. It's the local weather data in March 2015 gathered by 
all weather stations in the USA. You can obtain the data  ``QCLCD201503.zip`` from:

* `http://www.ncdc.noaa.gov/orders/qclcd <http://www.ncdc.noaa.gov/orders/qclcd>`_

The File size is `557 MB` and it has `4,496,262` lines.

Benchmark Results
~~~~~~~~~~~~~~~~~

Without further explanation, here are the Benchmark results for parsing the dataset.

:: 
    
    [TinyCsvParser (DegreeOfParallelism = 4, KeepOrder = True)] Elapsed Time = 00:00:10.48
    [CsvHelper] Elapsed Time = 00:00:32.60
    [FileHelpers] Crash

You can see, that `TinyCsvParser`_ is able to parse the file in `10.5` seconds only. Even if you don't 
process the data in parallel (`DegreeOfParallelism = 1`, which means it will utilize one thread only), 
it is still faster, than the `CsvHelper`_ library. The `FileHelpers`_ implementation crashed with an 
`OutOfMemory Exception`.

Benchmark Code
~~~~~~~~~~~~~~

Measuring the Execution Time
""""""""""""""""""""""""""""

The elapsed time of the import can be easily measured by using the :code:`System.Diagnostics.Stopwatch`.

.. code-block:: csharp

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


TinyCsvParser
"""""""""""""

.. code-block:: csharp

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
            MapProperty(1, x => x.Date, new DateTimeConverter("yyyyMMdd"));
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
                CsvParserOptions csvParserOptions = new CsvParserOptions(true, ',', degreeOfParallelism, order);
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
    

CsvHelper
"""""""""

.. code-block:: csharp

    public class CustomDateConverter : CsvHelper.TypeConversion.DefaultTypeConverter
    {
        private const string CustomDateFormat = @"yyyyMMdd";
    
        public override bool CanConvertFrom(Type type)
        {
            return typeof(String) == type;
        }
    
        public override bool CanConvertTo(Type type)
        {
            return typeof(DateTime) == type;
        }
    
        public override object ConvertFromString(CsvHelper.TypeConversion.TypeConverterOptions options, string text)
        {
            DateTime newDate = default(DateTime);
    
            try
            {
                newDate = DateTime.ParseExact(text, CustomDateFormat, CultureInfo.GetCultureInfo("en-US"));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format(@"Error parsing date '{0}': {1}", text, ex.Message));
            }
    
            return newDate;
        }
    }
    
    public sealed class CsvHelperMapping : CsvHelper.Configuration.CsvClassMap<LocalWeatherData>
    {
        public CsvHelperMapping()
        {
            Map(m => m.WBAN).Index(0);
            Map(m => m.Date).Index(1).TypeConverter<CustomDateConverter>();;
            Map(m => m.SkyCondition).Index(4);
        }
    }
    
    [Test]
    public void CsvHelperBenchmark()
    {
        MeasureElapsedTime("CsvHelper", () =>
        {
            using (TextReader reader = File.OpenText(@"C:\Users\philipp\Downloads\csv\201503hourly.txt"))
            {
                var csv = new CsvHelper.CsvReader(reader);
                csv.Configuration.RegisterClassMap<CsvHelperMapping>();
                csv.Configuration.Delimiter = ",";
                csv.Configuration.HasHeaderRecord = true;
    
                var usersFromCsv = csv.GetRecords<LocalWeatherData>().ToList();
            }
        });
    }


FileHelpers
"""""""""""

Sadly I was not able to figure out, how to select only the three columns in the mapping. Probably I am 
mistaken here and you should feel free to comment below, if you have a different solution to parse the 
file without writing the whole amount of columns.

.. code-block:: csharp

    [FileHelpers.IgnoreFirst(1)] 
    [FileHelpers.DelimitedRecord(",")]
    public class LocalWeatherDataFileHelper
    {
        public string WBAN;
    
        [FileHelpers.FieldConverter(FileHelpers.ConverterKind.Date, "yyyyMMdd")]
        public DateTime Date;
    
        private string dummyFieldTime;
    
        private string dummyFieldStationType;
    
        public string SkyCondition;
    
        private string[] mDummyField;
    }
    
    [Test]
    public void FileHelperBenchmark()
    {
        var engine = new FileHelpers.FileHelperEngine<LocalWeatherDataFileHelper>();
        MeasureElapsedTime("FileHelper", () =>
        {
            var result = engine.ReadFile(@"C:\Users\philipp\Downloads\csv\201503hourly.txt", 900000);
        });
    }

    
Detailed Benchmark Results
~~~~~~~~~~~~~~~~~~~~~~~~~~~

Here are the full benchmark results of [TinyCsvParser]. You can see, that increasing the number of threads 
helps when processing the data. Keeping the order doesn't have impact on the processing time, but it may 
lead to a much higher memory consumption. This may be a subject for a future article.

::

    [TinyCsvParser (DegreeOfParallelism = 4, KeepOrder = True)] Elapsed Time = 00:00:10.48
    [TinyCsvParser (DegreeOfParallelism = 3, KeepOrder = True)] Elapsed Time = 00:00:10.65
    [TinyCsvParser (DegreeOfParallelism = 2, KeepOrder = True)] Elapsed Time = 00:00:12.26
    [TinyCsvParser (DegreeOfParallelism = 1, KeepOrder = True)] Elapsed Time = 00:00:17.04
    [TinyCsvParser (DegreeOfParallelism = 4, KeepOrder = False)] Elapsed Time = 00:00:10.50
    [TinyCsvParser (DegreeOfParallelism = 3, KeepOrder = False)] Elapsed Time = 00:00:10.31
    [TinyCsvParser (DegreeOfParallelism = 2, KeepOrder = False)] Elapsed Time = 00:00:11.71
    [TinyCsvParser (DegreeOfParallelism = 1, KeepOrder = False)] Elapsed Time = 00:00:16.70
    
.. _CsvHelper: https://github.com/JoshClose/CsvHelper
.. _FileHelpers: http://www.filehelpers.net
.. _TinyCsvParser: https://github.com/bytefish/TinyCsvParser
.. _NUnit: http://www.nunit.org
.. MIT License: https://opensource.org/licenses/MIT
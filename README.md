# TinyCsvParser #

[TinyCsvParser]: https://github.com/bytefish/TinyCsvParser
[MIT License]: https://opensource.org/licenses/MIT

A lot of us have to import CSV data into their systems. What sounds simple often turns into a huge mess. 

There is no schema for CSV data. Suddenly a client changes some columns, or the format of numbers change, or the 
format of dates change or your requirements change. If you don't come up with a strategy for coping with uncertanity 
your code is going to suffer.

I have been there. I have written all this unmaintainable code myself.

Here is [TinyCsvParser], which is my attempt to build a clean, easy to use and high-performing library for CSV parsing in C#.

I have released it under terms of the [MIT License]:

* [https://github.com/bytefish/TinyCsvParser](https://github.com/bytefish/TinyCsvParser)

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
   
And that's it! The ``CsvParserOptions`` in this example are set to skip the header, use ``\n`` as line separator and ``;`` as column delimiter.

## Advanced Usage ##

### Custom TypeConverters ###

Now imagine your client suddenly changes a persons birthdate into a weird format and writes dates like this ``2004###01###25``. We can't parse such 
a date format with the default converters, but in [TinyCsvParser] we can easily define a ``DateTimeConverter`` with a custom date time format.

First of all extend the ``CsvPersonMapping`` to take a ``ITypeConverterProvider``.

```csharp
private class CsvPersonMapping : CsvMapping<Person>
{
    public CsvPersonMapping()
        : this(new TypeConverterProvider())
    {
    }
    public CsvPersonMapping(ITypeConverterProvider typeConverterProvider)
        : base(typeConverterProvider)
    {
        MapProperty(0, x => x.FirstName);
        MapProperty(1, x => x.LastName);
        MapProperty(2, x => x.BirthDate);
    }
}
```

And then let's write a Unit Test and override the default ``DateTime`` converter with a custom ``DateTimeConverter``, which takes the custom format string.

```csharp
[Test]
public void WeirdDateTimeTest()
{
    // Instantiate the TypeConverterProvider and override the DateTimeConverter:
    ITypeConverterProvider csvTypeConverterProvider = 
        new TypeConverterProvider().Add(new DateTimeConverter("yyyy###MM###dd"));
    
    // Make sure you pass the custom provider into the mapping!
    CsvPersonMapping csvMapper = new CsvPersonMapping(csvTypeConverterProvider);

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
```

And that's it!

### Getting functional with PLINQ ###

[ParallelQuery]: https://msdn.microsoft.com/en-us/library/system.linq.parallelquery(v=vs.100).aspx

The parser returns a [ParallelQuery], which can be used to perform additional processing on the data. I think LINQ is one of the most amazing things in C#! I was 
able to parallelize the whole parsing without dealing with locks or threads at all. It's 2015! Know the language you work in and you can make your code better and 
your life easier.

In the example we are going to parse the data and search for all records with the first name ``Philipp``. See how we don't need to write an if statement at all?

```csharp
[Test]
public void ParallelLinqTest()
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
```
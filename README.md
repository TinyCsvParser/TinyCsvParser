# TinyCsvParser #

[![NuGet Package](https://img.shields.io/nuget/v/TinyCsvParser.svg)](https://www.nuget.org/packages/TinyCsvParser/)
![Build status](https://github.com/TinyCsvParser/TinyCsvParser/actions/workflows/ci.yml/badge.svg)
![Build status](https://github.com/TinyCsvParser/TinyCsvParser/actions/workflows/release.yml/badge.svg)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](https://opensource.org/licenses/MIT)

[TinyCsvParser]: https://github.com/TinyCsvParser/TinyCsvParser
[MIT License]: https://opensource.org/licenses/MIT

[TinyCsvParser] is a .NET library to parse CSV data in an easy and *fun way*, while offering very high performance and a very clean API. It is highly configurable to provide maximum flexibility.

To get started quickly, follow the [Quickstart](https://tinycsvparser.github.io/TinyCsvParser/sections/quickstart.html).

[TinyCsvParser] supports .NET Core.

## Installing TinyCsvParser ##

You can use [NuGet](https://www.nuget.org) to install [TinyCsvParser]. Run the following command
in the [Package Manager Console](http://docs.nuget.org/consume/package-manager-console).

```powershell
PM> Install-Package TinyCsvParser
```

## Basic Usage ##

This is only an example for the most common use of TinyCsvParser. For more detailed information on custom formats and more advanced use-cases,
please consult the full [User Guide](https://tinycsvparser.github.io/TinyCsvParser/sections/userguide.html) of the official documentation.

Imagine we have list of Persons in a CSV file ``persons.csv`` with their first name, last name and birthdate.

```csv
FirstName;LastName;BirthDate
Philipp;Wagner;1986/05/12
Max;Musterman;2014/01/02
```

The corresponding domain model in our system might look like this.

```csharp
public class Person
{
    public string FirstName { get; set; }
    
    public string LastName { get; set; }
    
    public DateTime BirthDate { get; set; }
}
```

When using [TinyCsvParser] you have to define the mapping between the columns in the CSV data and the property in you domain model.

```csharp
public class CsvPersonMapping : CsvMapping<Person>
{
    public CsvPersonMapping()
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
        public void DoNotSkipHeaderTest()
        {
            // Build the CsvOptions with SkipHeader = true, so that the first line is skipped and is not parsed as data.
            CsvOptions csvOptions = CsvOptions.Rfc4180 with
            {
                SkipHeader = true
            };
    
            // Instantiate the CsvPersonMapping, which maps the CSV columns to the Person properties.
            CsvPersonMapping csvMapper = new CsvPersonMapping();
    
            // Create the CsvParser with the specified options and mapping.
            CsvParser<Person> csvParser = new CsvParser<Person>(csvOptions, csvMapper);
    
            // Reads the CSV data from the file "people.csv" and converts it to a list of CsvMappingResult<Person>.
            List<CsvMappingResult<Person>> result = csvParser
                .ReadFromFile("people.csv")
                .ToList();
    
            Assert.AreEqual(2, result.Count);
    
            Assert.IsTrue(result.All(x => x.IsSuccess));
    
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
    }
}
```

## TinyCsvParser 2.x ##

### Community Packages (TinyCsvParser 2.x) ###

[@Miista]: https://github.com/Miista

[@Miista] provides several community packages to simplify working with TinyCsvParser:

* [TinyCsvParser.Optional](https://github.com/Miista/TinyCsvParser.Optional)
* [TinyCsvParser.Collections](https://github.com/Miista/TinyCsvParser.Collections)
* [TinyCsvParser.ImmutableCollections](https://github.com/Miista/TinyCsvParser.ImmutableCollections)
* [TinyCsvParser.Enums](<https://github.com/Miista/TinyCsvParser.Enums>)

### Documentation and Changelog (TinyCsvParser 2.x) ###

[TinyCsvParser] comes with an official documentation and changelog:

* [https://tinycsvparser.github.io/TinyCsvParser/](https://tinycsvparser.github.io/TinyCsvParser/)


## License ##

The library is released under terms of the [MIT License]:

* [https://github.com/TinyCsvParser/TinyCsvParser](https://github.com/TinyCsvParser/TinyCsvParser)

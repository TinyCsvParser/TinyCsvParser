# TinyCsvParser #

[![NuGet Package](https://img.shields.io/nuget/v/TinyCsvParser.svg)](https://www.nuget.org/packages/TinyCsvParser/)
![Build status](https://github.com/TinyCsvParser/TinyCsvParser/actions/workflows/ci.yml/badge.svg)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](https://opensource.org/licenses/MIT)

TinyCsvParser is a high-performance CSV parsing library for .NET. This documentation explains the usage,
configuration, and extensibility of the library through practical examples.

> Upgrading from a previous version? Check out the [Migration Guide from 2.x to 3.x](#8-migration-from-2x-to-3x)

## Table of Contents ##

* [1. Setup](#1-setup)
* [2. Quick Start](#2-quick-start)
  * [2.1 The Model](#21-the-model)
  * [2.2 The Mapping](#22-the-mapping)
  * [2.3 Handling Quotes](#23-handling-quotes)
* [3. Configuring and Running the Parser](#3-configuring-and-running-the-parser)
  * [3.1 Setup the Parser](#31-setup-the-parser)
  * [3.2 Execute the Parsing](#32-execute-the-parsing)
* [4. Core Concept: Record and Line Tracking](#4-core-concept-record-and-line-tracking)
  * [4.1 LineNumber vs. RecordIndex](#41-linenumber-vs-recordindex)
  * [4.2 Reasoning for the Distinction](#42-reasoning-for-the-distinction)
* [5. Result Handling: Success, Error, and Comment](#5-result-handling-success-error-and-comment)
* [6. Dynamic Mapping (Dictionary & ExpandoObject)](#6-dynamic-mapping-dictionary--expandoobject)
  * [6.1 Inline Configuration](#61-inline-configuration)
  * [6.2 Explicit Converters & Custom Providers](#62-explicit-converters--custom-providers)
  * [6.3 Fallback Behavior](#63-fallback-behavior)
* [7. Advanced Usage: Accessing CsvRow](#7-advanced-usage-accessing-csvrow)
* [8. TypeConverters](#8-typeconverters)
  * [8.1 Configuring Existing Converters](#81-configuring-existing-converters)
  * [8.2 Writing a Custom Converter](#82-writing-a-custom-converter)
* [9. Migration from 2.x to 3.x](#9-migration-from-2x-to-3x)
  * [9.1 Data Access](#91-data-access)
  * [9.2 Result Pattern](#92-result-pattern)
  * [9.3 Error Metadata](#93-error-metadata)

## 1. Setup ##

To include TinyCsvParser in your project, install the NuGet package using the .NET CLI:

```powershell
dotnet add package TinyCsvParser
```

Alternatively, you can use the NuGet Package Manager in Visual Studio:

```bash
Install-Package TinyCsvParser
```

## 2. Quick Start ##

To parse a CSV file, you need a target model and a mapping definition.

### 2.1 The Model ###

The target of your parsing operation should be a class with a parameterless constructor.

```csharp
public class Person
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
```

### 2.2 The Mapping ###

Create a class inheriting from `CsvMapping<T>` and define the relationship between CSV columns and model properties.

#### Option A: Mapping by Header Name (Recommended) ####

This approach is flexible as it doesn't depend on the order of columns in the CSV file. The
parser automatically resolves the names to indices.

```csharp
public class PersonMapping : CsvMapping<Person>
{
    public PersonMapping()
    {
        MapProperty("ID", x => x.Id);
        MapProperty("Full Name", x => x.Name);
    }
}
```

#### Option B: Mapping by Index ####

Use this for files without headers or for maximum performance.

```csharp
public class PersonMappingByIndex : CsvMapping<Person>
{
    public PersonMappingByIndex()
    {
        // 0-based index: ID is column 0, Name is column 1
        MapProperty(0, x => x.Id);
        MapProperty(1, x => x.Name);
    }
}
```

### 2.3 Handling Quotes ###

TinyCsvParser automatically handles fields wrapped in quotes. This is essential when your data
or your header names contain the delimiter character or line breaks.

```csharp
// Example CSV: "ID";"Full Name"
// The parser strips the quotes automatically. 
// You map using the clean name:
MapProperty("Full Name", x => x.Name);
```

Quoted fields can contain the delimiter (e.g., "Doe, John") or even escaped quotes (e.g., "The ""Great"" Gatsby"), which
the parser resolves before passing the value to the mapping.

## 3. Configuring and Running the Parser ##

The CsvParser is the central engine. It is stateless and can be reused for multiple parsing operations.

### 3.1 Setup the Parser ###

First, you combine your `CsvOptions` and your `CsvMapping` to create the parser instance.

```csharp
// 1. Define the technical format
CsvOptions options = new(
    Delimiter: ';', 
    QuoteChar: '"', 
    EscapeChar: '"', 
    SkipHeader: true,
    CommentCharacter: '#'
);

// 2. Instantiate your mapping logic
PersonMapping mapping = new();

// 3. Create the parser (Stateless and reusable)
CsvParser<Person> parser = new(options, mapping);
```

### 3.2 Execute the Parsing ###

You can read the CSV data synchronously or asynchronously. TinyCsvParser provides full support for `IAsyncEnumerable`
for maximum
performance with asynchronous data streams. Crucially, the parsing process uses deferred execution (lazy loading). The
file is
read and parsed one record at a time as you iterate.

#### Synchronous Reading ####

The parser supports reading from strings, streams, or files.

```csharp
// Calling ReadFromFile does NOT start the parsing yet.
// It returns an Enumerable that waits for a foreach loop.
IEnumerable<CsvMappingResult<Person>> results = parser.ReadFromFile("data.csv");

// The actual parsing happens here, one record at a time.
foreach (CsvMappingResult<Person> result in results)
{
    // Every result encapsulates Success, Error, or Comment states.
    if (result.IsSuccess)
    {
        Person person = result.Result;
        Console.WriteLine($"Parsed: {person.Name}");
    }
}
```

#### Asynchronous Reading ####

For modern applications, `ReadFromFileAsync` can be used to minimize buffer copying and memory pressure.

```csharp
// Returns an IAsyncEnumerable
var resultsAsync = parser.ReadFromFileAsync("data.csv");

// Iterate asynchronously using await foreach
await foreach (var result in resultsAsync.ConfigureAwait(false))
{
    if (result.IsSuccess)
    {
        Person person = result.Result;
        Console.WriteLine($"Async Parsed: {person.Name}");
    }
}
```

## 4. Core Concept: Record and Line Tracking ##

TinyCsvParser distinguishes between two types of indices. This distinction is necessary because CSV
files often deviate from a simple "one line equals one record" structure.

### 4.1 LineNumber vs. RecordIndex ###

* `LineNumber`: Refers to the physical line in the source file (1-based).
* `RecordIndex`: Refers to the logical data entity (0-based).

### 4.2 Reasoning for the Distinction ###

* **Quoted Newlines**: If a CSV field contains a newline (e.g., a description field), a single logical record spans
  multiple physical lines. In this case, the LineNumber will point to the start of the record, but the next record's
  LineNumber will jump several lines ahead.
* **Comments**: If CommentCharacter is set, comment lines occupy a physical LineNumber but do not increment the
  RecordIndex.
* **Header**: The header row consumes a LineNumber but is not counted as a data RecordIndex.

**Usage Tip**: Always use LineNumber when reporting errors to users, as it corresponds directly to what they see in a
text editor!

## 5. Result Handling: Success, Error, and Comment ##

The `CsvMappingResult<T>` captures every possible state of a row. The `Switch` method ensures
all states are handled correctly.

```csharp
foreach (CsvMappingResult<Person> item in parser.ReadFromStream(stream))
{
    item.Switch(
        onSuccess: (Person entity) => 
            Console.WriteLine($"[Record {item.RecordIndex}] Imported: {entity.Name}"),
            
        onFailure: (CsvMappingError error) => 
            Console.WriteLine($"[Line {item.LineNumber}] Error in Column {error.ColumnIndex}: {error.Value}"),
            
        onComment: (string comment) => 
            Console.WriteLine($"[Line {item.LineNumber}] Meta-Info: {comment}")
    );
}
```

## 6. Dynamic Mapping (Dictionary & ExpandoObject) ##

When the CSV schema is only known at runtime, or you want to avoid creating dedicated classes for
simple scripts, you can parse rows directly into dynamic structures (`Dictionary<string, object?>` or
`ExpandoObject`).

For performance it's maybe better to map to a `Dictionary`, as it avoids Dynamic Language Runtime overhead.

### 6.1 Inline Configuration ###

Use the static factory methods on the `CsvParser` class. The schema is configured inline using a delegate.

```csharp
using TinyCsvParser;

CsvOptions options = new(Delimiter: ';', QuoteChar: '"', EscapeChar: '"', SkipHeader: false);

// Create the parser and configure the schema in one go
var parser = CsvParser.CreateDictionaryParser(options, schema => 
{
    schema.Add<int>("Id");       // Resolves Int32Converter automatically
    schema.Add<double>("Price"); // Resolves DoubleConverter automatically
});

foreach (var result in parser.ReadFromFile("products.csv"))
{
    if (result.IsSuccess)
    {
        Dictionary<string, object?> row = result.Result;
        Console.WriteLine($"Item {row["Id"]} costs {row["Price"]}");
    }
}
```

> Note: Use `CsvParser.CreateExpandoParser(...)` if you prefer to access fields via the dynamic keyword like `row.Id`.

### 6.2 Explicit Converters & Custom Providers ###

While `Add<T>` is the most convenient method, you can pass explicit converter instances if you need special
configurations
(e.g., `date formats`).

```csharp
var parser = CsvParser.CreateDictionaryParser(options, schema => 
{
    schema.Add<int>("Id");
    schema.Add("BirthDate", new DateTimeConverter("yyyy-MM-dd")); // Explicit Converter
});
```

### 6.3 Fallback Behavior ###

Any column present in the CSV header that is not mapped in your `CsvSchema` will automatically be
parsed as a raw `string`. This prevents data loss while maintaining strict typing for the columns
you care about.

## 7. Advanced Usage: Accessing CsvRow ##

For complex logic, `MapUsing` provides direct access to the `ref struct CsvRow`. To ensure errors are handled
properly, the delegate returns a `MapUsingResult`.

```csharp
public class AdvancedMapping : CsvMapping<Person>
{
    public AdvancedMapping()
    {
        MapUsing((Person entity, ref CsvRow row) =>
        {
            if (row.Count < 2) 
                return MapUsingResult.Failure("Too few columns.");

            if (!int.TryParse(row.GetSpan(0), out int id)) 
                return MapUsingResult.Failure($"Invalid ID: {row.GetString(0)}");

            entity.Id = id;
            entity.Name = row.GetString(1);

            return MapUsingResult.Success();
        });
    }
}
```

## 8. TypeConverters ##

### 8.1 Configuring Existing Converters ###

You can pass specific parameters (like date formats) to built-in converters during mapping.

```csharp
DateTimeConverter dateConverter = new("yyyy-MM-dd");

MapProperty("BirthDate", x => x.BirthDate, dateConverter);
```

### 8.2 Writing a Custom Converter ###

Inherit from `NonNullableConverter<T>` to implement custom parsing logic directly on the memory spans.

```csharp
public class YesNoConverter : NonNullableConverter<bool>
{
    protected override bool InternalConvert(ReadOnlySpan<char> value, out bool result)
    {
        if (value.Equals("Yes".AsSpan(), StringComparison.OrdinalIgnoreCase))
        {
            result = true;
            return true;
        }
        result = false;
        return false;
    }
}
```

## 9. Migration from 2.x to 3.x ##

### 9.1 Data Access ###

In Version 2.x, custom logic used a `string[]`. In Version 3.0, it uses `ref CsvRow`. This allows the library to work
with `ReadOnlySpan<char>`, significantly reducing memory allocations.

### 9.3 Error Metadata ###

Error objects in Version 3.0 now contain both `RecordIndex` and `LineNumber`. If you previously relied on indices for
debugging, ensure
you switch to `LineNumber` for file-based troubleshooting. This is what the user sees in their CSV file.

using NUnit.Framework;
using System.Linq;
using TinyCsvParser.Models;
using TinyCsvParser.TypeConverters;

namespace TinyCsvParser.Test.Parsing;

[TestFixture]
public class CsvParserFactoryTests
{
    private const string CsvData = "Id;Price\n1;42.5\n2;99.9";
    private CsvOptions _options;

    [SetUp]
    public void Setup()
    {
        _options = new CsvOptions(';', '"', '"', SkipHeader: false);
    }

    [Test]
    public void CreateDictionary_WithSchemaInstance_ParsesCorrectly()
    {
        // Arrange
        var schema = new CsvSchema();
        schema.Add<int>("Id");
        schema.Add<double>("Price");

        // Act
        var parser = CsvParser.CreateDictionaryParser(_options, schema);
        var results = parser.ReadFromString(CsvData).ToList();

        // Assert
        Assert.That(results, Has.Count.EqualTo(2));
        Assert.That(results[0].IsSuccess, Is.True);

        var firstRow = results[0].Result;
        Assert.That(firstRow["Id"], Is.EqualTo(1));
        Assert.That(firstRow["Price"], Is.EqualTo(42.5d));
    }

    [Test]
    public void CreateDictionary_WithDelegate_ParsesCorrectly()
    {
        // Act
        var parser = CsvParser.CreateDictionaryParser(_options, schema =>
        {
            schema.Add<int>("Id");
            schema.Add<double>("Price");
        });

        var results = parser.ReadFromString(CsvData).ToList();

        // Assert
        Assert.That(results, Has.Count.EqualTo(2));
        Assert.That(results[1].IsSuccess, Is.True);

        var secondRow = results[1].Result;
        Assert.That(secondRow["Id"], Is.EqualTo(2));
        Assert.That(secondRow["Price"], Is.EqualTo(99.9d));
    }

    [Test]
    public void CreateDictionary_WithExplicitConverter_ParsesCorrectly()
    {
        // Arrange
        var schema = new CsvSchema();
        schema.Add("Id", new Int32Converter());
        schema.Add("Price", new DoubleConverter());

        // Act
        var parser = CsvParser.CreateDictionaryParser(_options, schema);
        var results = parser.ReadFromString(CsvData).ToList();

        // Assert
        Assert.That(results, Has.Count.EqualTo(2));
        Assert.That(results[0].IsSuccess, Is.True);

        var firstRow = results[0].Result;
        Assert.That(firstRow["Id"], Is.EqualTo(1));
        Assert.That(firstRow["Price"], Is.EqualTo(42.5d));
    }

    [Test]
    public void CreateExpando_WithSchemaInstance_ParsesCorrectly()
    {
        // Arrange
        var schema = new CsvSchema();
        schema.Add<int>("Id");
        schema.Add<double>("Price");

        // Act
        var parser = CsvParser.CreateExpandoParser(_options, schema);
        var results = parser.ReadFromString(CsvData).ToList();

        // Assert
        Assert.That(results, Has.Count.EqualTo(2));
        Assert.That(results[0].IsSuccess, Is.True);

        dynamic firstRow = results[0].Result;
        Assert.That(firstRow.Id, Is.EqualTo(1));
        Assert.That(firstRow.Price, Is.EqualTo(42.5d));
    }

    [Test]
    public void CreateExpando_WithDelegate_ParsesCorrectly()
    {
        // Act
        var parser = CsvParser.CreateExpandoParser(_options, schema =>
        {
            schema.Add<int>("Id");
            schema.Add<double>("Price");
        });

        var results = parser.ReadFromString(CsvData).ToList();

        // Assert
        Assert.That(results, Has.Count.EqualTo(2));
        Assert.That(results[1].IsSuccess, Is.True);

        dynamic secondRow = results[1].Result;
        Assert.That(secondRow.Id, Is.EqualTo(2));
        Assert.That(secondRow.Price, Is.EqualTo(99.9d));
    }

    [Test]
    public void Factory_ParsesUnmappedColumnsAsString()
    {
        // Arrange
        string csvWithExtraCol = "Id;Name\n1;John Doe";

        var parser = CsvParser.CreateDictionaryParser(_options, schema =>
        {
            schema.Add<int>("Id");
        });

        // Act
        var results = parser.ReadFromString(csvWithExtraCol).ToList();

        // Assert
        Assert.That(results[0].IsSuccess, Is.True);
        var row = results[0].Result;

        Assert.That(row["Id"], Is.TypeOf<int>());
        Assert.That(row["Id"], Is.EqualTo(1));

        Assert.That(row["Name"], Is.TypeOf<string>());
        Assert.That(row["Name"], Is.EqualTo("John Doe"));
    }
}
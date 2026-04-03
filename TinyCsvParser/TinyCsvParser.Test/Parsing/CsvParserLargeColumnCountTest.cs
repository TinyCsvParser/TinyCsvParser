using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;
using TinyCsvParser.Core;
using TinyCsvParser.Models;

namespace TinyCsvParser.Test.Parsing;

public class DummyEntity
{
    public string DummyField { get; set; }
}

public class DummyMapping : ICsvMapping<DummyEntity>
{
    public int ParsedTokensCount { get; private set; }

    public CsvMappingResult<DummyEntity> Map(ref CsvRow row, CsvHeaderResolution? headerResolution = null)
    {
        ParsedTokensCount = row.Count;

        return default;
    }
}

[TestFixture]
public class CsvParserTests
{
    [TestCase(10)]    
    [TestCase(255)]   
    [TestCase(256)]   
    [TestCase(257)]   
    [TestCase(600)]   
    [TestCase(1500)]  
    [TestCase(5000)]  
    public void ReadFromString_WithVariousColumnSizes_ShouldResizeBufferAndParseSuccessfully(int columnCount)
    {
        // Arrange
        var options = new CsvOptions { Delimiter = ',', QuoteChar = '"', SkipHeader = false };
        var mapping = new DummyMapping();
        var parser = new CsvParser<DummyEntity>(options, mapping);

        // Create a CSV line with the specified number of columns
        var csvLine = string.Join(",", Enumerable.Repeat("data", columnCount));

        // Act
        var results = parser.ReadFromString(csvLine).ToList();

        // Assert
        Assert.That(results.Count, Is.EqualTo(1), "Should successfully read one logical record.");
        Assert.That(mapping.ParsedTokensCount, Is.EqualTo(columnCount), "The parser should correctly split and allocate all columns without throwing an overflow error.");
    }

    [TestCase(10)]
    [TestCase(256)]
    [TestCase(1000)]
    public async Task ReadFromStringAsync_WithVariousColumnSizes_ShouldResizeBufferAndParseSuccessfully(int columnCount)
    {
        // Arrange
        var options = new CsvOptions { Delimiter = ',', QuoteChar = '"', SkipHeader = false };
        var mapping = new DummyMapping();
        var parser = new CsvParser<DummyEntity>(options, mapping);

        var csvLine = string.Join(",", Enumerable.Repeat("data", columnCount));

        // Act
        int resultCount = 0;
        await foreach (var result in parser.ReadFromStringAsync(csvLine))
        {
            resultCount++;
        }

        // Assert
        Assert.That(resultCount, Is.EqualTo(1), "Should successfully read one logical record asynchronously.");
        Assert.That(mapping.ParsedTokensCount, Is.EqualTo(columnCount), "The async parser should correctly split and allocate all columns without throwing an overflow error.");
    }
}
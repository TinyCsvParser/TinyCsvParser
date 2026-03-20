using NUnit.Framework;
using System.Linq;
using TinyCsvParser.Models;

namespace TinyCsvParser.Test.CsvParser;

public class NoteEntity
{
    public int Id { get; set; }

    public string Text { get; set; } = string.Empty;
}

public sealed class NoteMapping : CsvMapping<NoteEntity>
{
    public NoteMapping()
    {
        MapUsing((NoteEntity entity, ref CsvRow row) =>
        {
            if (row.Count < 2) return false;

            if (!int.TryParse(row.GetSpan(0), out int id))
            {
                return false;
            }

            entity.Id = id;
            entity.Text = row.GetString(1);

            return true;
        });
    }
}

[TestFixture]
public class LineAndRecordIndexTests
{
    private CsvParser<NoteEntity> CreateParser(bool skipHeader)
    {
        var options = new CsvOptions(
            Delimiter: ';',
            QuoteChar: '"',
            EscapeChar: '"',
            SkipHeader: skipHeader
        );

        return new CsvParser<NoteEntity>(options, new NoteMapping());
    }

    [Test]
    public void Parser_SimpleLines_RecordAndLineNumberMatchExactly()
    {
        // Arrange
        string csvData =
            "1;First Note\n" +
            "2;Second Note\n" +
            "3;Third Note";

        var parser = CreateParser(skipHeader: false);

        // Act
        var results = parser.ReadFromString(csvData).ToList();

        // Assert
        Assert.That(results, Has.Count.EqualTo(3));
        
        Assert.Multiple(() =>
        {
            // Record Index is 0-based, line-number is 1-based
            Assert.That(results[0].RecordIndex, Is.EqualTo(0));
            Assert.That(results[0].LineNumber, Is.EqualTo(1));

            Assert.That(results[1].RecordIndex, Is.EqualTo(1));
            Assert.That(results[1].LineNumber, Is.EqualTo(2));

            Assert.That(results[2].RecordIndex, Is.EqualTo(2));
            Assert.That(results[2].LineNumber, Is.EqualTo(3));
        });
    }

    [Test]
    public void Parser_QuotedMultilineString_LineNumberJumpsCorrectly()
    {
        // Arrange
        // Line 1: ID 1, Text in Line 1 and 2
        // Line 3: ID 2, Normal text
        string csvData =
            "1;\"Multi\nLine\"\n" +
            "2;Single Line";

        var parser = CreateParser(skipHeader: false);

        // Act
        var results = parser.ReadFromString(csvData).ToList();

        // Assert
        Assert.That(results, Has.Count.EqualTo(2));
        
        Assert.Multiple(() =>
        {
            Assert.That(results[0].RecordIndex, Is.EqualTo(0));
            Assert.That(results[0].LineNumber, Is.EqualTo(1));
            Assert.That(results[0].Result.Text, Is.EqualTo("Multi\nLine"));

            Assert.That(results[1].RecordIndex, Is.EqualTo(1));
            Assert.That(results[1].LineNumber, Is.EqualTo(3));
        });
    }

    [Test]
    public void Parser_MappingError_ProvidesCorrectRowAndLineIndices()
    {
        // Arrange
        string csvData =
            "1;Valid\n" +
            "invalid_id;Error here\n" +
            "3;Valid again";

        var parser = CreateParser(skipHeader: false);

        // Act
        var results = parser.ReadFromString(csvData).ToList();

        // Assert
        Assert.That(results, Has.Count.EqualTo(3));

        Assert.That(results[0].IsSuccess, Is.True);
        Assert.That(results[2].IsSuccess, Is.True);

        var errorResult = results[1];

        Assert.That(errorResult.IsSuccess, Is.False);
        
        Assert.Multiple(() =>
        {
            Assert.That(errorResult.RecordIndex, Is.EqualTo(1));
            Assert.That(errorResult.LineNumber, Is.EqualTo(2));

            var error = errorResult.Error;

            Assert.That(error.RecordIndex, Is.EqualTo(1));
            Assert.That(error.LineNumber, Is.EqualTo(2));
            Assert.That(error.Value, Contains.Substring("Custom MapUsing validation/mapping failed."));
        });
    }

    [Test]
    public void Parser_WithSkipHeader_StartsRecordIndexAtZeroButLineNumberAtTwo()
    {
        // Arrange
        string csvData =
            "Id;Text\n" +
            "1;First Note\n" +
            "2;Second Note";

        var parser = CreateParser(skipHeader: true);

        // Act
        var results = parser.ReadFromString(csvData).ToList();

        // Assert
        Assert.That(results, Has.Count.EqualTo(2));

        Assert.Multiple(() =>
        {
            Assert.That(results[0].RecordIndex, Is.EqualTo(0));
            Assert.That(results[0].LineNumber, Is.EqualTo(2));

            Assert.That(results[1].RecordIndex, Is.EqualTo(1));
            Assert.That(results[1].LineNumber, Is.EqualTo(3));
        });
    }
}
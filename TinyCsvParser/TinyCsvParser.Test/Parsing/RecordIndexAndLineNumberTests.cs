using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using TinyCsvParser.Models;

namespace TinyCsvParser.Test.Parsing;

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
            if (row.Count < 2)
            {
                return MapUsingResult.Failure($"Not enough columns. Expected at least 2, but got {row.Count}.");
            }

            if (!int.TryParse(row.GetSpan(0), out int id))
            {
                return MapUsingResult.Failure($"Failed to parse ID '{row.GetString(0)}' to an integer.");
            }

            entity.Id = id;
            entity.Text = row.GetString(1);

            return MapUsingResult.Success();
        });
    }
}

[TestFixture]
public class LineAndRecordIndexTests
{
    private CsvParser<NoteEntity> CreateParser(bool skipHeader)
    {
        CsvOptions options = new(
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

        CsvParser<NoteEntity> parser = CreateParser(skipHeader: false);

        // Act
        List<CsvMappingResult<NoteEntity>> results = parser.ReadFromString(csvData).ToList();

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
        string csvData =
            "1;\"Multi\nLine\"\n" +
            "2;Single Line";

        CsvParser<NoteEntity> parser = CreateParser(skipHeader: false);

        // Act
        List<CsvMappingResult<NoteEntity>> results = parser.ReadFromString(csvData).ToList();

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

        CsvParser<NoteEntity> parser = CreateParser(skipHeader: false);

        // Act
        List<CsvMappingResult<NoteEntity>> results = parser.ReadFromString(csvData).ToList();

        // Assert
        Assert.That(results, Has.Count.EqualTo(3));

        Assert.That(results[0].IsSuccess, Is.True);
        Assert.That(results[2].IsSuccess, Is.True);

        CsvMappingResult<NoteEntity> errorResult = results[1];

        Assert.That(errorResult.IsSuccess, Is.False);

        Assert.Multiple(() =>
        {
            Assert.That(errorResult.RecordIndex, Is.EqualTo(1));
            Assert.That(errorResult.LineNumber, Is.EqualTo(2));

            CsvMappingError error = errorResult.Error;

            Assert.That(error.RecordIndex, Is.EqualTo(1));
            Assert.That(error.LineNumber, Is.EqualTo(2));
            // Assert auf die spezifische Fehlermeldung aus dem MapUsingResult
            Assert.That(error.Value, Contains.Substring("Failed to parse ID 'invalid_id'"));
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

        CsvParser<NoteEntity> parser = CreateParser(skipHeader: true);

        // Act
        List<CsvMappingResult<NoteEntity>> results = parser.ReadFromString(csvData).ToList();

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
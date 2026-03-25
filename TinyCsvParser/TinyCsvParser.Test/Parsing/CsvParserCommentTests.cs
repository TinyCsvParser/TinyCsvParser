// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TinyCsvParser.Models;

namespace TinyCsvParser.Test.Parsing;

public class CommentTestEntity
{
    public int Id { get; set; }
    public string Data { get; set; } = string.Empty;
}

public sealed class CommentTestMapping : CsvMapping<CommentTestEntity>
{
    public CommentTestMapping()
    {
        MapProperty(0, x => x.Id);
        MapProperty(1, x => x.Data);
    }
}

[TestFixture]
public class CsvParserCommentTests
{
    private CsvParser<CommentTestEntity> CreateParser(bool skipHeader, char? commentChar)
    {
        CsvOptions options = new(
            Delimiter: ';',
            QuoteChar: '"',
            EscapeChar: '"',
            Encoding: System.Text.Encoding.UTF8,
            SkipHeader: skipHeader,
            CommentCharacter: commentChar
        );

        return new CsvParser<CommentTestEntity>(options, new CommentTestMapping());
    }

    [Test]
    public void Parser_WithComments_YieldsCommentsAndMaintainsDataIndices()
    {
        // Arrange
        string csvData =
            "# File generated automatically\n" +   // Line 1: Comment
            "Id;Data\n" +                        // Line 2: Header
            "1;First\n" +                        // Line 3: Data
            "   # Indented comment works too\n" +// Line 4: Comment
            "2;Second\n" +                       // Line 5: Data
            "# End of file";                     // Line 6: Comment

        CsvParser<CommentTestEntity> parser = CreateParser(skipHeader: true, commentChar: '#');

        // Act
        List<CsvMappingResult<CommentTestEntity>> results = parser.ReadFromString(csvData).ToList();

        // Assert
        Assert.That(results, Has.Count.EqualTo(5)); // 3 Comments + 2 Data Rows (Header skipped)

        Assert.Multiple(() =>
        {
            // 1. File Header Comment
            Assert.That(results[0].IsComment, Is.True);
            Assert.That(results[0].Comment, Is.EqualTo("# File generated automatically"));
            Assert.That(results[0].LineNumber, Is.EqualTo(1));
            Assert.That(results[0].RecordIndex, Is.EqualTo(-1));

            // 2. First Data Row
            Assert.That(results[1].IsSuccess, Is.True);
            Assert.That(results[1].Result.Id, Is.EqualTo(1));
            Assert.That(results[1].LineNumber, Is.EqualTo(3));
            Assert.That(results[1].RecordIndex, Is.EqualTo(0));

            // 3. Indented Comment
            Assert.That(results[2].IsComment, Is.True);
            Assert.That(results[2].Comment, Is.EqualTo("   # Indented comment works too"));
            Assert.That(results[2].LineNumber, Is.EqualTo(4));
            Assert.That(results[2].RecordIndex, Is.EqualTo(-1));

            // 4. Second Data Row
            Assert.That(results[3].IsSuccess, Is.True);
            Assert.That(results[3].Result.Id, Is.EqualTo(2));
            Assert.That(results[3].LineNumber, Is.EqualTo(5));
            Assert.That(results[3].RecordIndex, Is.EqualTo(1));

            // 5. End of file comment
            Assert.That(results[4].IsComment, Is.True);
            Assert.That(results[4].Comment, Is.EqualTo("# End of file"));
            Assert.That(results[4].LineNumber, Is.EqualTo(6));
            Assert.That(results[4].RecordIndex, Is.EqualTo(-1));
        });
    }

    [Test]
    public void CsvMappingResult_ThreeParameterMatch_WorksCorrectlyForComments()
    {
        // Arrange
        CsvMappingResult<CommentTestEntity> result = new("# My Comment", -1, 10);

        // Act
        string matchedType = result.Match(
            onSuccess: entity => "Success",
            onFailure: error => "Error",
            onComment: comment => $"Comment: {comment}"
        );

        // Assert
        Assert.That(matchedType, Is.EqualTo("Comment: # My Comment"));
    }

    [Test]
    public void CsvMappingResult_Properties_ThrowOnWrongState()
    {
        // Arrange
        CsvMappingResult<CommentTestEntity> result = new("# My Comment", -1, 10);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _ = result.Result);
        Assert.Throws<InvalidOperationException>(() => _ = result.Error);
        Assert.DoesNotThrow(() => _ = result.Comment);
    }
}
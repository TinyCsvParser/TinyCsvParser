// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using NUnit.Framework;
using TinyCsvParser.Mapping;
using TinyCsvParser.Model;

namespace TinyCsvParser.Test.Mapping;

[TestFixture]
public class CsvMappingTests
{
    private class SampleEntity
    {
        public int PropertyInt { get; set; }
        public int GetOnlyPropertyInt { get; }
    }

    private class DuplicateMapping : CsvMapping<SampleEntity>
    {
        public DuplicateMapping()
        {
            MapProperty(0, x => x.PropertyInt);
            MapProperty(0, x => x.PropertyInt);
        }
    }
        
    [Test]
    public void DuplicateMappingTest()
    {
        NUnit.Framework.Assert.Throws<InvalidOperationException>(() => new DuplicateMapping());
    }

    private class WrongColumnMapping : CsvMapping<SampleEntity>
    {
        public WrongColumnMapping()
        {
            MapProperty(2, x => x.PropertyInt);
        }
    }

    [Test]
    public void MapEntity_Invalid_Column_Test()
    {
        var mapping = new WrongColumnMapping();

        var result = mapping.Map(new TokenizedRow(1, ["A", "1"]));

        Assert.IsFalse(result.IsValid);
        Assert.IsNotNull(result.Error);
        Assert.AreEqual("A|1", result.Error.UnmappedRow);
    }

    private class CorrectColumnMapping : CsvMapping<SampleEntity>
    {
        public CorrectColumnMapping()
        {
            MapProperty(0, x => x.PropertyInt);
        }
    }
        

    [Test]
    public void MapEntity_ConversionError_Test()
    {
        var mapping = new CorrectColumnMapping();

        var result = mapping.Map(new TokenizedRow(1, [string.Empty]));

        Assert.IsFalse(result.IsValid);

        Assert.AreEqual("Column 0 with Value '' cannot be converted", result.Error.Value);
        Assert.AreEqual(0, result.Error.ColumnIndex);
        Assert.AreEqual(string.Empty, result.Error.UnmappedRow);

        NUnit.Framework.Assert.DoesNotThrow(() => result.ToString());
    }

    [Test]
    public void MapEntity_ConversionSuccess_Test()
    {
        var mapping = new CorrectColumnMapping();

        var result = mapping.Map(new TokenizedRow(1, ["1"]));

        Assert.IsTrue(result.IsValid);
        Assert.AreEqual(1, result.Result.PropertyInt);

        NUnit.Framework.Assert.DoesNotThrow(() => result.ToString());
    }

    private class GetOnlyIntColumnMapping : CsvMapping<SampleEntity>
    {
        public GetOnlyIntColumnMapping()
        {
            MapProperty(0, x => x.GetOnlyPropertyInt);
        }
    }

    [Test]
    public void MapEntity_GetOnlyError_Test()
    {
        NUnit.Framework.Assert.Throws<InvalidOperationException>(() => new GetOnlyIntColumnMapping());
    }
}
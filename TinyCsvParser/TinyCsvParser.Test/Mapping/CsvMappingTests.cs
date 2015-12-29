using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyCsvParser.Mapping;

namespace TinyCsvParser.Test.Issues
{

    [TestFixture]
    public class CsvMappingTests
    {
        private class SampleEntity
        {
            public int PropertyInt { get; set; }
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
            Assert.Throws<InvalidOperationException>(() => new DuplicateMapping());
        }

        private class WrongColumnMapping : CsvMapping<SampleEntity>
        {
            public WrongColumnMapping()
            {
                MapProperty(1, x => x.PropertyInt);
            }
        }

        [Test]
        public void MapEntity_Invalid_Column_Test()
        {
            var mapping = new WrongColumnMapping();

            var result = mapping.Map(new []{"1"});

            Assert.IsFalse(result.IsValid);
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

            var result = mapping.Map(new[] { string.Empty });

            Assert.IsFalse(result.IsValid);

            Assert.AreEqual(string.Empty, result.Error.Value);
            Assert.AreEqual(0, result.Error.ColumnIndex);
        }

        [Test]
        public void MapEntity_ConversionSuccess_Test()
        {
            var mapping = new CorrectColumnMapping();

            var result = mapping.Map(new[] { "1" });

            Assert.IsTrue(result.IsValid);
            Assert.AreEqual(1, result.Result.PropertyInt);
        }
    }
}

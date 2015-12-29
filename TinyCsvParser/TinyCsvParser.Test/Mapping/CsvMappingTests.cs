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
    }
}

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyCsvParser.Test.CsvParser
{
    [TestFixture]
    public class CsvReaderOptionsTests
    {
        [Test]
        public void ToStringTest()
        {
            var csvReaderOptions = new CsvReaderOptions(new string[] { Environment.NewLine });

            Assert.DoesNotThrow(() => csvReaderOptions.ToString());
        }
    }
}

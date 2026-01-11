// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;

namespace TinyCsvParser.Test.CsvParser;

[TestFixture]
public class CsvReaderOptionsTests
{
    [Test]
    public void ToStringTest()
    {
        var csvReaderOptions = new CsvReaderOptions([Environment.NewLine]);

        NUnit.Framework.Assert.DoesNotThrow(() => csvReaderOptions.ToString());
    }
}
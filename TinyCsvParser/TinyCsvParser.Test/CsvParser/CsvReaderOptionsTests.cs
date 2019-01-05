// Copyright (c) Philipp Wagner and Joel Mueller. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using NUnit.Framework;

namespace TinyCsvParser.Test.CsvParser
{
    [TestFixture]
    public class CsvReaderOptionsTests
    {
        [Test]
        public void ToStringTest()
        {
            var csvReaderOptions = new CsvReaderOptions(Environment.NewLine);

            Assert.DoesNotThrow(() => csvReaderOptions.ToString());
        }
    }
}

// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace TinyCsvParser.Test.CsvParser
{
    [TestClass]
    public class CsvReaderOptionsTests
    {
        [TestMethod]
        public void ToStringTest()
        {
            var csvReaderOptions = new CsvReaderOptions(new string[] { Environment.NewLine });

            try
            {
                csvReaderOptions.ToString();
            }
            catch
            {
                Assert.IsTrue(false);
            }
        }
    }
}

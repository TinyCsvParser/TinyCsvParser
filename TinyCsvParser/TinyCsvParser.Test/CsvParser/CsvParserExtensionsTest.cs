// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyCsvParser.Mapping;

namespace TinyCsvParser.Test.CsvParser
{
    [TestClass]
    public class CsvParserExtensionsTest
    {
        private class Person
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public DateTime BirthDate { get; set; }
        }

        private class CsvPersonMapping : CsvMapping<Person>
        {
            public CsvPersonMapping()
            {
                MapProperty(0, x => x.FirstName);
                MapProperty(1, x => x.LastName);
                MapProperty(2, x => x.BirthDate);
            }
        }


        [TestMethod]
        public void ReadFromFile_null_Test()
        {
            CsvParserOptions csvParserOptions = new CsvParserOptions(true, ';');
            CsvPersonMapping csvMapper = new CsvPersonMapping();
            CsvParser<Person> csvParser = new CsvParser<Person>(csvParserOptions, csvMapper);

            Assert.ThrowsExceptionAsync<ArgumentNullException>(() => csvParser.ReadFromFileAsync(null, Encoding.UTF8).ToListAsync().AsTask());
        }

        [TestMethod]
        public async Task ReadFromFileTest()
        {
            CsvParserOptions csvParserOptions = new CsvParserOptions(true, ';');
            CsvPersonMapping csvMapper = new CsvPersonMapping();
            CsvParser<Person> csvParser = new CsvParser<Person>(csvParserOptions, csvMapper);

            var stringBuilder = new StringBuilder()
                .AppendLine("FirstName;LastName;BirthDate")
                .AppendLine("     Philipp;Wagner;1986/05/12       ")
                .AppendLine("Max;Mustermann;2014/01/01");

            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            var filePath = Path.Combine(basePath, "test_file.txt");

            File.WriteAllText(filePath, stringBuilder.ToString(), Encoding.UTF8);

            var result = await csvParser
                .ReadFromFileAsync(filePath.ToString(), Encoding.UTF8)
                .ToListAsync();

            Assert.AreEqual(2, result.Count);

            Assert.IsTrue(result.All(x => x.IsValid));

            Assert.AreEqual("Philipp", result[0].Result.FirstName);
            Assert.AreEqual("Wagner", result[0].Result.LastName);

            Assert.AreEqual(1986, result[0].Result.BirthDate.Year);
            Assert.AreEqual(5, result[0].Result.BirthDate.Month);
            Assert.AreEqual(12, result[0].Result.BirthDate.Day);

            Assert.AreEqual("Max", result[1].Result.FirstName);
            Assert.AreEqual("Mustermann", result[1].Result.LastName);
            Assert.AreEqual(2014, result[1].Result.BirthDate.Year);
            Assert.AreEqual(1, result[1].Result.BirthDate.Month);
            Assert.AreEqual(1, result[1].Result.BirthDate.Day);
        }

        [TestMethod]
        public void ReadFromStream_null_Test()
        {
            CsvParserOptions csvParserOptions = new CsvParserOptions(true, ';');
            CsvPersonMapping csvMapper = new CsvPersonMapping();
            CsvParser<Person> csvParser = new CsvParser<Person>(csvParserOptions, csvMapper);

            Assert.ThrowsExceptionAsync<ArgumentNullException>(() => csvParser.ReadFromStreamAsync(null, Encoding.UTF8).ToListAsync().AsTask());
        }

        [TestMethod]
        public async Task ReadFromStreamTest()
        {
            CsvParserOptions csvParserOptions = new CsvParserOptions(true, ';');
            CsvPersonMapping csvMapper = new CsvPersonMapping();
            CsvParser<Person> csvParser = new CsvParser<Person>(csvParserOptions, csvMapper);

            var stringBuilder = new StringBuilder()
                .AppendLine("FirstName;LastName;BirthDate")
                .AppendLine("     Philipp;Wagner;1986/05/12       ")
                .AppendLine("Max;Mustermann;2014/01/01");

            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            var filePath = Path.Combine(basePath, "test_file.txt");

            File.WriteAllText(filePath, stringBuilder.ToString(), Encoding.UTF8);

            using (var stream = File.OpenRead(filePath))
            {
                var result = await csvParser
                    .ReadFromStreamAsync(stream, Encoding.UTF8)
                    .ToListAsync();

                Assert.AreEqual(2, result.Count);

                Assert.IsTrue(result.All(x => x.IsValid));

                Assert.AreEqual("Philipp", result[0].Result.FirstName);
                Assert.AreEqual("Wagner", result[0].Result.LastName);

                Assert.AreEqual(1986, result[0].Result.BirthDate.Year);
                Assert.AreEqual(5, result[0].Result.BirthDate.Month);
                Assert.AreEqual(12, result[0].Result.BirthDate.Day);

                Assert.AreEqual("Max", result[1].Result.FirstName);
                Assert.AreEqual("Mustermann", result[1].Result.LastName);
                Assert.AreEqual(2014, result[1].Result.BirthDate.Year);
                Assert.AreEqual(1, result[1].Result.BirthDate.Month);
                Assert.AreEqual(1, result[1].Result.BirthDate.Day);
            }
        }
    }
}

// Copyright (c) Philipp Wagner and Joel Mueller. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Text;
using NUnit.Framework;
using TinyCsvParser.Mapping;

namespace TinyCsvParser.Test.Benchmarks
{

    [TestFixture(Description="Performance Test"), Explicit("Could take too much time...")]
    public class TinyCsvParserTest
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

        [Test, Explicit("Performance Test creating a string with 1000000 lines")]
        public void DegreeOfParallelismTest()
        {
            int csvDataLines = 1000000;
            int[] degreeOfParallelismList = new[] { 1, 2, 4 };

            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < csvDataLines; i++)
            {
                stringBuilder.AppendLine("Philipp;Wagner;1986/05/12");
            }
            var csvData = stringBuilder.ToString();

            foreach (var degreeOfParallelism in degreeOfParallelismList)
            {
                CsvParserOptions csvParserOptions = new CsvParserOptions(true, ';' , degreeOfParallelism, true);
                CsvReaderOptions csvReaderOptions = new CsvReaderOptions(Environment.NewLine);
                CsvPersonMapping csvMapper = new CsvPersonMapping();
                CsvParser<Person> csvParser = new CsvParser<Person>(csvParserOptions, csvMapper);

                MeasurementUtils.MeasureElapsedTime(string.Format("DegreeOfParallelismTest (DegreeOfParallelism = {0})", degreeOfParallelism), () => csvParser.ReadFromString(csvReaderOptions, csvData).ToList());
            }
        }
    }
}
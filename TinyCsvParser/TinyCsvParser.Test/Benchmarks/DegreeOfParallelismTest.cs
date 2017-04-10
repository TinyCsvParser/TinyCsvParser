﻿// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using System.Linq;
using System.Text;
using TinyCsvParser.Mapping;

namespace TinyCsvParser.Test.Benchmarks
{

    [TestFixture(Description="Performance Test (takes around 10 seconds)")]
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

        [Test]
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
                CsvParserOptions csvParserOptions = new CsvParserOptions(true, new[] { ';' }, degreeOfParallelism, true);
                CsvReaderOptions csvReaderOptions = new CsvReaderOptions(new[] { Environment.NewLine });
                CsvPersonMapping csvMapper = new CsvPersonMapping();
                CsvParser<Person> csvParser = new CsvParser<Person>(csvParserOptions, csvMapper);

                MeasurementUtils.MeasureElapsedTime($"DegreeOfParallelismTest (DegreeOfParallelism = {degreeOfParallelism})", () => csvParser.ReadFromString(csvReaderOptions, csvData).ToList());
            }
        }
    }
}
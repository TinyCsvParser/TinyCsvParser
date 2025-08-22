﻿// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using System.Linq;
using System.Text;
using TinyCsvParser.Mapping;
using TinyCsvParser.Ranges;

namespace TinyCsvParser.Test.CsvParser
{
    [TestFixture]
    public class CsvParserImmutableTest
    {
        private class Measurement
        {
            public string Id { get; set; }

            public float[] Values { get; set; }

            public string ReadOnlyId { get; }

            public int[] ReadOnlyArray { get; }

            public Measurement(string readOnlyId, int[] readOnlyArray)
            {
                ReadOnlyId = readOnlyId;
				ReadOnlyArray = readOnlyArray;
			}
        }

        private class CsvMeasurementMapping : CsvMapping<Measurement>
        {
            public CsvMeasurementMapping()
            {
                MapProperty(0, x => x.Id);
                MapProperty(new RangeDefinition(1, 2), x => x.Values);
                MapConstructorParameter<string>(3, 0);
                MapConstructorParameter<int[]>(new RangeDefinition(4, 6), 1);
            }
        }

        [Test]
        public void MapConstructorParametersTest()
        {
            CsvParserOptions csvParserOptions = new CsvParserOptions(false, ';');
            CsvReaderOptions csvReaderOptions = new CsvReaderOptions(new[] { Environment.NewLine });
            CsvMeasurementMapping csvMapper = new CsvMeasurementMapping();
            CsvParser<Measurement> csvParser = new CsvParser<Measurement>(csvParserOptions, csvMapper);


            var stringBuilder = new StringBuilder()
                .AppendLine("Device1;1.0;2.0;DeviceBrand1;0;1;2")
                .AppendLine("Device2;3.0;4.0;DeviceBrand2;0;3;4");

            var result = csvParser
                .ReadFromString(csvReaderOptions, stringBuilder.ToString())
                .ToList();

            Assert.AreEqual(2, result.Count);

            Assert.IsTrue(result.All(x => x.IsValid));

            Assert.AreEqual("Device1", result[0].Result.Id);
            Assert.IsNotNull(result[0].Result.Values);
            Assert.AreEqual(2, result[0].Result.Values.Length);
            Assert.AreEqual(1.0, result[0].Result.Values[0]);
            Assert.AreEqual(2.0, result[0].Result.Values[1]);
            Assert.AreEqual("DeviceBrand1", result[0].Result.ReadOnlyId);
            Assert.AreEqual(3, result[0].Result.ReadOnlyArray.Length);
            Assert.AreEqual(0, result[0].Result.ReadOnlyArray[0]);
            Assert.AreEqual(1, result[0].Result.ReadOnlyArray[1]);
            Assert.AreEqual(2, result[0].Result.ReadOnlyArray[2]);

            Assert.AreEqual("Device2", result[1].Result.Id);
            Assert.IsNotNull(result[1].Result.Values);
            Assert.AreEqual(2, result[1].Result.Values.Length);
            Assert.AreEqual(3.0, result[1].Result.Values[0]);
            Assert.AreEqual(4.0, result[1].Result.Values[1]);
            Assert.AreEqual("DeviceBrand2", result[1].Result.ReadOnlyId);
            Assert.AreEqual(3, result[1].Result.ReadOnlyArray.Length);
            Assert.AreEqual(0, result[1].Result.ReadOnlyArray[0]);
            Assert.AreEqual(3, result[1].Result.ReadOnlyArray[1]);
            Assert.AreEqual(4, result[1].Result.ReadOnlyArray[2]);
        }
    }
}

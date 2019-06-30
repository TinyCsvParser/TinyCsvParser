.. _tutorials_mapping_arrays:

Mapping Arrays
==============

To parse a range of values into an array you can use an overload of the :code:`MapProperty` method, that takes a :code:`RangeDefinition` object. The :code:`RangeDefinition(int startIndex, int endIndex)` takes the start index and end index of the array (zero-based inclusive indices).

Example
~~~~~~~

.. code-block:: csharp

    // Copyright (c) Philipp Wagner. All rights reserved.
    // Licensed under the MIT license. See LICENSE file in the project root for full license information.

    using NUnit.Framework;
    using System;
    using System.Linq;
    using System.Text;
    using TinyCsvParser.Mapping;
    using TinyCsvParser.Ranges;

    namespace TinyCsvParser.Test.CsvParser
    {
        [TestFixture]
        public class CsvParserArrayTest
        {
            private class Measurement
            {
                public string Id { get; set; }

                public float[] Values { get; set; }
            }

            private class CsvMeasurementMapping : CsvMapping<Measurement>
            {
                public CsvMeasurementMapping()
                {
                    MapProperty(0, x => x.Id);
                    MapProperty(new RangeDefinition(1, 2), x => x.Values);
                }
            }

            [Test]
            public void FloatArraysTest()
            {
                CsvParserOptions csvParserOptions = new CsvParserOptions(false, ';' );
                CsvReaderOptions csvReaderOptions = new CsvReaderOptions(new[] { Environment.NewLine });
                CsvMeasurementMapping csvMapper = new CsvMeasurementMapping();
                CsvParser<Measurement> csvParser = new CsvParser<Measurement>(csvParserOptions, csvMapper);


                var stringBuilder = new StringBuilder()
                    .AppendLine("Device1;1.0;2.0")
                    .AppendLine("Device2;3.0;4.0");

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

                Assert.AreEqual("Device2", result[1].Result.Id);
                Assert.IsNotNull(result[1].Result.Values);
                Assert.AreEqual(2, result[1].Result.Values.Length);
                Assert.AreEqual(3.0, result[1].Result.Values[0]);
                Assert.AreEqual(4.0, result[1].Result.Values[1]);
            }
        }
    }

How **easy** was that?

.. _TinyCsvParser: https://github.com/bytefish/TinyCsvParser
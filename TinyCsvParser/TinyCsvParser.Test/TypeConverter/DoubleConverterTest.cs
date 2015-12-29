// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.Test.TypeConverter
{
    [TestFixture]
    public class DoubleConverterTest : BaseConverterTest<Double>
    {
        protected override ITypeConverter<Double> Converter
        {
            get { return new DoubleConverter(); }
        }

        protected override Tuple<string, Double>[] SuccessTestData
        {
            get
            {
                return new[] {
                    MakeTuple(Double.MinValue.ToString("R"), Double.MinValue),
                    MakeTuple(Double.MaxValue.ToString("R"), Double.MaxValue),
                    MakeTuple("0", 0),
                    MakeTuple("-1000", -1000),
                    MakeTuple("1000", 1000),
                    MakeTuple("5e2", 500),
                };
            }
        }

        public override void AssertAreEqual(Double expected, Double actual)
        {
            Assert.AreEqual(expected, actual, Double.Epsilon);
        }

        protected override string[] FailTestData
        {
            get { return new[] { "a", string.Empty, "  ", null }; }
        }
    }
}

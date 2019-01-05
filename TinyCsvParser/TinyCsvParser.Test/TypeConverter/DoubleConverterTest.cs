// Copyright (c) Philipp Wagner and Joel Mueller. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using NUnit.Framework;
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

        protected override (string, Double)[] SuccessTestData
        {
            get
            {
                return new[] {
                    (Double.MinValue.ToString("R"), Double.MinValue),
                    (Double.MaxValue.ToString("R"), Double.MaxValue),
                    ("0", 0),
                    ("-1000", -1000),
                    ("1000", 1000),
                    ("5e2", 500),
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

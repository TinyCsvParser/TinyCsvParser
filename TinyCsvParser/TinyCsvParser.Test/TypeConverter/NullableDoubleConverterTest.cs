// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using System.Globalization;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.Test.TypeConverter
{
    [TestFixture]
    public class NullableDoubleConverterTest : BaseConverterTest<Double?>
    {
        protected override ITypeConverter<Double?> Converter
        {
            get { return new NullableDoubleConverter(); }
        }

        protected override (string, Double?)[] SuccessTestData
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
                    (" ", default(Double?)),
                    (null, default(Double?)),
                    (string.Empty, default(Double?))
                };
            }
        }

        public override void AssertAreEqual(Double? expected, Double? actual)
        {
            if (expected == default(Double?))
            {
                Assert.AreEqual(expected, actual);
            }
            else
            {
                Assert.AreEqual(expected.Value, actual, Double.Epsilon);
            }
        }

        protected override string[] FailTestData
        {
            get { return new[] { "a", }; }
        }
    }

    [TestFixture]
    public class NullableDoubleConverterWithFormatProviderTest : NullableDoubleConverterTest
    {
        protected override ITypeConverter<Double?> Converter
        {
            get { return new NullableDoubleConverter(CultureInfo.InvariantCulture); }
        }
    }

    [TestFixture]
    public class NullableDoubleConverterWithFormatProviderAndNumberStyleTest : NullableDoubleConverterTest
    {
        protected override ITypeConverter<Double?> Converter
        {
            get { return new NullableDoubleConverter(CultureInfo.InvariantCulture, NumberStyles.Float | NumberStyles.AllowThousands); }
        }
    }
}

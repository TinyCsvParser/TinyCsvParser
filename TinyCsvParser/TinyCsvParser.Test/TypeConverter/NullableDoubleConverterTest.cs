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

        protected override Tuple<string, Double?>[] SuccessTestData
        {
            get
            {
                return new[] {
                    MakeTuple(double.MinValue.ToString("R"), double.MinValue),
                    MakeTuple(double.MaxValue.ToString("R"), double.MaxValue),
                    MakeTuple("0", 0),
                    MakeTuple("-1000", -1000),
                    MakeTuple("1000", 1000),
                    MakeTuple("5e2", 500),
                    MakeTuple(" ", default(Double?)),
                    MakeTuple(null, default(Double?)),
                    MakeTuple(string.Empty, default(Double?))
                };
            }
        }

        public override void AssertAreEqual(double? expected, double? actual)
        {
            if (expected == default(double?))
            {
                Assert.AreEqual(expected, actual);
            }
            else
            {
                Assert.That(actual.Value, Is.EqualTo(expected.Value).Within(double.Epsilon));
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

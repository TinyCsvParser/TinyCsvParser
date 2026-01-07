// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using System.Globalization;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.Test.TypeConverter
{
    [TestFixture]
    public class NullableDoubleConverterTest : BaseConverterTest<double?>
    {
        protected override ITypeConverter<double?> Converter => new NullableDoubleConverter();

        protected override Tuple<string, double?>[] SuccessTestData =>
        [
            MakeTuple(double.MinValue.ToString("R"), double.MinValue),
            MakeTuple(double.MaxValue.ToString("R"), double.MaxValue),
            MakeTuple("0", 0),
            MakeTuple("-1000", -1000),
            MakeTuple("1000", 1000),
            MakeTuple("5e2", 500),
            MakeTuple(" ", null),
            MakeTuple(null, null),
            MakeTuple(string.Empty, null)
        ];

        protected override void AssertAreEqual(double? expected, double? actual)
        {
            if (expected is null)
            {
                Assert.AreEqual(null, actual);
            }
            else
            {
                NUnit.Framework.Assert.That(actual, Is.EqualTo(expected.Value).Within(double.Epsilon));
            }
        }

        protected override string[] FailTestData => ["a"];
    }

    [TestFixture]
    public class NullableDoubleConverterWithFormatProviderTest : NullableDoubleConverterTest
    {
        protected override ITypeConverter<double?> Converter =>
            new NullableDoubleConverter(CultureInfo.InvariantCulture);
    }

    [TestFixture]
    public class NullableDoubleConverterWithFormatProviderAndNumberStyleTest : NullableDoubleConverterTest
    {
        protected override ITypeConverter<double?> Converter =>
            new NullableDoubleConverter(CultureInfo.InvariantCulture, NumberStyles.Float | NumberStyles.AllowThousands);
    }
}
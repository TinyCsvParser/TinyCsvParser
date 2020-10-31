// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using System.Globalization;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.Test.TypeConverter
{
    [TestFixture]
    public class NullableSingleConverterTest : BaseConverterTest<Single?>
    {
        protected override ITypeConverter<Single?> Converter
        {
            get { return new NullableSingleConverter(); }
        }

        protected override Tuple<string, Single?>[] SuccessTestData
        {
            get
            {
                return new[] {
                    MakeTuple(Single.MinValue.ToString("R"), Single.MinValue),
                    MakeTuple(Single.MaxValue.ToString("R"), Single.MaxValue),
                    MakeTuple("0", 0),
                    MakeTuple("-1000", -1000),
                    MakeTuple("1000", 1000),
                    MakeTuple("5e2", 500),
                    MakeTuple(" ", default(Single?)),
                    MakeTuple(null, default(Single?)),
                    MakeTuple(string.Empty, default(Single?))
                };
            }
        }

        public override void AssertAreEqual(float? expected, float? actual)
        {
            if (expected == default(float?))
            {
                Assert.AreEqual(expected, actual);
            }
            else
            {
                Assert.AreEqual(expected.Value, actual, float.Epsilon);
            }
        }

        protected override string[] FailTestData
        {
            get { return new[] { "a" }; }
        }
    }

    [TestFixture]
    public class NullableSingleConverterWithFormatProviderTest : NullableSingleConverterTest
    {
        protected override ITypeConverter<Single?> Converter
        {
            get { return new NullableSingleConverter(CultureInfo.InvariantCulture); }
        }
    }

    [TestFixture]
    public class NullableSingleConverterWithFormatProviderAndNumberStyleTest : NullableSingleConverterTest
    {
        protected override ITypeConverter<Single?> Converter
        {
            get { return new NullableSingleConverter(CultureInfo.InvariantCulture, NumberStyles.Float | NumberStyles.AllowThousands); }
        }
    }

}

using NUnit.Framework;
using System;
using System.Globalization;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.Test.TypeConverter
{
    [TestFixture]
    public class NullableDecimalConverterTest : BaseConverterTest<Decimal?>
    {
        protected override ITypeConverter<Decimal?> Converter
        {
            get { return new NullableDecimalConverter(); }
        }

        protected override Tuple<string, Decimal?>[] SuccessTestData
        {
            get
            {
                return new[] {
                    MakeTuple(Decimal.MinValue.ToString(), Decimal.MinValue),
                    MakeTuple(Decimal.MaxValue.ToString(), Decimal.MaxValue),
                    MakeTuple("0", 0),
                    MakeTuple("-1000", -1000),
                    MakeTuple("1000", 1000),
                    MakeTuple(" ", default(Decimal?)),
                    MakeTuple(null, default(Decimal?)),
                    MakeTuple(string.Empty, default(Decimal?))
                };
            }
        }

        protected override string[] FailTestData
        {
            get { return new[] { "a" }; }
        }
    }

    [TestFixture]
    public class NullableDecimalConverterWithFormatProviderTest : NullableDecimalConverterTest
    {
        protected override ITypeConverter<Decimal?> Converter
        {
            get { return new NullableDecimalConverter(CultureInfo.InvariantCulture); }
        }
    }

    [TestFixture]
    public class NullableDecimalConverterWithFormatProviderAndNumberStylesTest : NullableDecimalConverterTest
    {
        protected override ITypeConverter<Decimal?> Converter
        {
            get { return new NullableDecimalConverter(CultureInfo.InvariantCulture, NumberStyles.Number); }
        }
    }
}

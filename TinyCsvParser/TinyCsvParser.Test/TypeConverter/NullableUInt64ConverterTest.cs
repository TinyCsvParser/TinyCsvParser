using NUnit.Framework;
using System;
using System.Globalization;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.Test.TypeConverter
{
    [TestFixture]
    public class NullableUInt64ConverterTest : BaseConverterTest<UInt64?>
    {
        protected override ITypeConverter<UInt64?> Converter
        {
            get { return new NullableUInt64Converter(); }
        }

        protected override Tuple<string, UInt64?>[] SuccessTestData
        {
            get
            {
                return new[] {
                    MakeTuple(UInt64.MinValue.ToString(), UInt64.MinValue),
                    MakeTuple(UInt64.MaxValue.ToString(), UInt64.MaxValue),
                    MakeTuple("0", 0),
                    MakeTuple("1000", 1000),
                    MakeTuple(" ", default(UInt64?)),
                    MakeTuple(null, default(UInt64?)),
                    MakeTuple(string.Empty, default(UInt64?))
                };
            }
        }

        protected override string[] FailTestData
        {
            get { return new[] { "a", "-1000", Int16.MinValue.ToString(), Int64.MinValue.ToString() }; }
        }
    }

    [TestFixture]
    public class NullableUInt64ConverterWithFormatProviderTest : NullableUInt64ConverterTest
    {
        protected override ITypeConverter<UInt64?> Converter
        {
            get { return new NullableUInt64Converter(CultureInfo.InvariantCulture); }
        }
    }

    [TestFixture]
    public class NullableUInt64ConverterWithFormatProviderAndNumberStylesTest : NullableUInt64ConverterTest
    {
        protected override ITypeConverter<UInt64?> Converter
        {
            get { return new NullableUInt64Converter(CultureInfo.InvariantCulture, NumberStyles.Integer); }
        }
    }
}

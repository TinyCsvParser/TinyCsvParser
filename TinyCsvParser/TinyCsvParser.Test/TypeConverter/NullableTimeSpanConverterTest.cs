using NUnit.Framework;
using System;
using System.Globalization;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.Test.TypeConverter
{
    [TestFixture]
    public class NullableTimeSpanConverterTest : BaseConverterTest<TimeSpan?>
    {
        protected override ITypeConverter<TimeSpan?> Converter
        {
            get { return new NullableTimeSpanConverter(); }
        }

        protected override Tuple<string, TimeSpan?>[] SuccessTestData
        {
            get
            {
                return new[] {
                    MakeTuple(TimeSpan.MinValue.ToString(), TimeSpan.MinValue),
                    MakeTuple("14", TimeSpan.FromDays(14)),
                    MakeTuple("1:2:3", TimeSpan.FromHours(1).Add(TimeSpan.FromMinutes(2)).Add(TimeSpan.FromSeconds(3))),
                    MakeTuple(" ", default(TimeSpan?)),
                    MakeTuple(null, default(TimeSpan?)),
                    MakeTuple(string.Empty, default(TimeSpan?))
                };
            }
        }

        protected override string[] FailTestData
        {
            get { return new[] { "a" }; }
        }
    }

    [TestFixture]
    public class NullableTimeSpanConverterWithFormatProviderTest : NullableTimeSpanConverterTest
    {
        protected override ITypeConverter<TimeSpan?> Converter
        {
            get { return new NullableTimeSpanConverter(string.Empty); }
        }
    }

    [TestFixture]
    public class NullableTimeSpanConverterWithFormatAndFormatProviderTest : NullableTimeSpanConverterTest
    {
        protected override ITypeConverter<TimeSpan?> Converter
        {
            get { return new NullableTimeSpanConverter(string.Empty, CultureInfo.InvariantCulture); }
        }
    }

    [TestFixture]
    public class NullableTimeSpanConverterWithFormatAndFormatProviderAndTimeSpanStyleTest : NullableTimeSpanConverterTest
    {
        protected override ITypeConverter<TimeSpan?> Converter
        {
            get { return new NullableTimeSpanConverter(string.Empty, CultureInfo.InvariantCulture, TimeSpanStyles.None); }
        }
    }

}

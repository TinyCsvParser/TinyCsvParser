using NUnit.Framework;
using System;
using System.Globalization;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.Test.TypeConverter
{
    [TestFixture]
    public class NullableGuidConverterTest : BaseConverterTest<Guid?>
    {
        protected override ITypeConverter<Guid?> Converter
        {
            get { return new NullableGuidConverter(); }
        }

        protected override Tuple<string, Guid?>[] SuccessTestData
        {
            get
            {
                return new [] {
                    MakeTuple("02001000-0010-0000-0000-003200000000", Guid.Parse("02001000-0010-0000-0000-003200000000")),
                    MakeTuple(null, default(Guid?)),
                    MakeTuple(string.Empty, default(Guid?)),
                };
            }
        }

        protected override string[] FailTestData
        {
            get { return new[] { "a", Int32.MinValue.ToString(), Int32.MaxValue.ToString() }; }
        }
    }

    [TestFixture]
    public class NullableGuidConverterWithFormatTest : NullableGuidConverterTest
    {
        protected override ITypeConverter<Guid?> Converter
        {
            get { return new NullableGuidConverter("D"); }
        }
    }
}

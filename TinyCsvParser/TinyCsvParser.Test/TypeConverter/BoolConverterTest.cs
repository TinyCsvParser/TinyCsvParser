using NUnit.Framework;
using System;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.Test.TypeConverter
{
    [TestFixture]
    public class BoolConverterTest : BaseConverterTest<bool>
    {
        protected override ITypeConverter<bool> Converter
        {
            get { return new BoolConverter(); }
        }

        protected override Tuple<string, bool>[] SuccessTestData
        {
            get
            {
                return new[] {
                    MakeTuple("true", true),
                    MakeTuple("false", false),
                    MakeTuple("True", true),
                    MakeTuple("False", false),
                };
            }
        }

        protected override string[] FailTestData
        {
            get { return new[] { "a", string.Empty, "  ", null }; }
        }
    }

    [TestFixture]
    public class BoolConverterNonDefaultTest : BaseConverterTest<bool>
    {
        protected override ITypeConverter<bool> Converter
        {
            get { return new BoolConverter("ThisIsTrue", "ThisIsFalse", StringComparison.InvariantCulture); }
        }

        protected override Tuple<string, bool>[] SuccessTestData
        {
            get
            {
                return new[] {
                    MakeTuple("ThisIsTrue", true),
                    MakeTuple("ThisIsFalse", false),
                };
            }
        }

        protected override string[] FailTestData
        {
            get { return new[] { "a", string.Empty, "  ", null, "thisistrue", "thisisfalse" }; }
        }
    }
}

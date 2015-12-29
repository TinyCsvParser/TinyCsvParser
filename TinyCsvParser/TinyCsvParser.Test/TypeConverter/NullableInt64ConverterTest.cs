using NUnit.Framework;
using System;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.Test.TypeConverter
{
    [TestFixture]
    public class NullableInt64ConverterTest : BaseConverterTest<Int64?>
    {
        protected override ITypeConverter<Int64?> Converter
        {
            get { return new NullableInt64Converter(); }
        }

        protected override Tuple<string, Int64?>[] SuccessTestData
        {
            get
            {
                return new[] {
                    MakeTuple(Int64.MinValue.ToString(), Int64.MinValue),
                    MakeTuple(Int64.MaxValue.ToString(), Int64.MaxValue),
                    MakeTuple("0", 0),
                    MakeTuple("-1000", -1000),
                    MakeTuple("1000", 1000),
                    MakeTuple(" ", default(Int64?)),
                    MakeTuple(null, default(Int64?)),
                    MakeTuple(string.Empty, default(Int64?))
                };
            }
        }

        protected override string[] FailTestData
        {
            get { return new[] { "a" }; }
        }
    }
}

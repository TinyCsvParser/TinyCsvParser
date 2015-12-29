using NUnit.Framework;
using System;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.Test.TypeConverter
{
    [TestFixture]
    public class NullableSByteConverterTest : BaseConverterTest<SByte?>
    {
        protected override ITypeConverter<SByte?> Converter
        {
            get { return new NullableSByteConverter(); }
        }

        protected override Tuple<string, SByte?>[] SuccessTestData
        {
            get
            {
                return new[] {
                    MakeTuple(SByte.MinValue.ToString(), SByte.MinValue),
                    MakeTuple(SByte.MaxValue.ToString(), SByte.MaxValue),
                    MakeTuple("0", 0),
                    MakeTuple("-128", -128),
                    MakeTuple("127", 127),
                    MakeTuple(" ", default(SByte?)),
                    MakeTuple(null, default(SByte?)),
                    MakeTuple(string.Empty, default(SByte?))
                };
            }
        }

        protected override string[] FailTestData
        {
            get { return new[] { "a", "-129", "128" }; }
        }
    }
}

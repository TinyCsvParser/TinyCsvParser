using NUnit.Framework;
using System;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.Test.TypeConverter
{
    [TestFixture]
    public class NullableByteConverterTest : BaseConverterTest<Byte?>
    {
        protected override ITypeConverter<Byte?> Converter
        {
            get { return new NullableByteConverter(); }
        }

        protected override Tuple<string, Byte?>[] SuccessTestData
        {
            get
            {
                return new[] {
                    MakeTuple(Byte.MinValue.ToString(), Byte.MinValue),
                    MakeTuple(Byte.MaxValue.ToString(), Byte.MaxValue),
                    MakeTuple("0", 0),
                    MakeTuple("255", 255),
                    MakeTuple(" ", default(Byte?)),
                    MakeTuple(null, default(Byte?)),
                    MakeTuple(string.Empty, default(Byte?))
                };
            }
        }

        protected override string[] FailTestData
        {
            get { return new[] { "a", "-1", "256" }; }
        }
    }
}

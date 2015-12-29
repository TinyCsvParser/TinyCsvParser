using NUnit.Framework;
using System;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.Test.TypeConverter
{
    [TestFixture]
    public class ByteConverterTest : BaseConverterTest<Byte>
    {
        protected override ITypeConverter<Byte> Converter
        {
            get { return new ByteConverter(); }
        }

        protected override Tuple<string, Byte>[] SuccessTestData
        {
            get
            {
                return new[] {
                    MakeTuple(Byte.MinValue.ToString(), Byte.MinValue),
                    MakeTuple(Byte.MaxValue.ToString(), Byte.MaxValue),
                    MakeTuple("0", 0),
                    MakeTuple("255", 255)
                };
            }
        }

        protected override string[] FailTestData
        {
            get { return new[] { "a", string.Empty, "  ", null, "-1", "256" }; }
        }
    }
}

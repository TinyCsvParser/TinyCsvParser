using NUnit.Framework;
using System;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.Test.TypeConverter
{
    [TestFixture]
    public class NullableDateTimeConverterTest : BaseConverterTest<DateTime?>
    {
        protected override ITypeConverter<DateTime?> Converter
        {
            get { return new NullableDateTimeConverter(); }
        }

        protected override Tuple<string, DateTime?>[] SuccessTestData
        {
            get
            {
                return new[] {
                    MakeTuple("2014/01/01", new DateTime(2014, 1, 1)),
                    MakeTuple("9999/12/31", new DateTime(9999, 12, 31)),
                    MakeTuple(" ", default(DateTime?)),
                    MakeTuple(null, default(DateTime?)),
                    MakeTuple(string.Empty, default(DateTime?))
                };
            }
        }

        protected override string[] FailTestData
        {
            get { return new[] { "a", "10000/01/01", "1753/01/32", "0/0/0" }; }
        }
    }
}

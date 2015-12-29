using NUnit.Framework;
using System;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.Test.TypeConverter
{
    [TestFixture]
    public class StringConverterTest : BaseConverterTest<String>
    {
        protected override ITypeConverter<String> Converter
        {
            get { return new StringConverter(); }
        }

        protected override Tuple<string, String>[] SuccessTestData
        {
            get
            {
                return new[] {
                    MakeTuple(string.Empty, string.Empty),
                    MakeTuple(" ", " "),
                    MakeTuple("Abc", "Abc"),
                    MakeTuple(null, null)
                };
            }
        }

        protected override string[] FailTestData
        {
            get { return new String[] {  }; } // Should never fail, because values are passed through.
        }
    }
}

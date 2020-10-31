// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.Test.TypeConverter
{
    [TestFixture]
    public class UInt64ConverterTest : BaseConverterTest<UInt64>
    {
        protected override ITypeConverter<ulong> Converter
        {
            get { return new UInt64Converter(); }
        }

        protected override Tuple<string, ulong>[] SuccessTestData
        {
            get
            {
                return new[] {
                    MakeTuple(UInt64.MinValue.ToString(), UInt64.MinValue),
                    MakeTuple(UInt64.MaxValue.ToString(), UInt64.MaxValue),
                    MakeTuple("0", 0),
                    MakeTuple("1000", 1000)
                };
            }
        }

        protected override string[] FailTestData
        {
            get { return new[] { "a", "-1000", string.Empty, "  ", null, Int16.MinValue.ToString(), Int64.MinValue.ToString() }; }
        }
    }
}

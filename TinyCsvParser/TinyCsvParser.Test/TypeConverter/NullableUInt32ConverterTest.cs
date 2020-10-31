// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using System.Globalization;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.Test.TypeConverter
{
    [TestFixture]
    public class NullableUInt32ConverterTest : BaseConverterTest<UInt32?>
    {
        protected override ITypeConverter<UInt32?> Converter
        {
            get { return new NullableUInt32Converter(); }
        }

        protected override Tuple<string, UInt32?>[] SuccessTestData
        {
            get
            {
                return new[] {
                    MakeTuple(UInt32.MinValue.ToString(), UInt32.MinValue),
                    MakeTuple(UInt32.MaxValue.ToString(), UInt32.MaxValue),
                    MakeTuple("0", 0),
                    MakeTuple("1000", 1000),
                    MakeTuple(" ", default(UInt32?)),
                    MakeTuple(null, default(UInt32?)),
                    MakeTuple(string.Empty, default(UInt32?))
                };
            }
        }

        protected override string[] FailTestData
        {
            get { return new[] { "a", "-1000", Int16.MinValue.ToString() }; }
        }
    }

    [TestFixture]
    public class NullableUInt32ConverterWithFormatProviderTest : NullableUInt32ConverterTest
    {
        protected override ITypeConverter<UInt32?> Converter
        {
            get { return new NullableUInt32Converter(CultureInfo.InvariantCulture); }
        }
    }

    [TestFixture]
    public class NullableUInt32ConverterWithFormatProviderAndNumberStylesTest : NullableUInt32ConverterTest
    {
        protected override ITypeConverter<UInt32?> Converter
        {
            get { return new NullableUInt32Converter(CultureInfo.InvariantCulture, NumberStyles.Integer); }
        }
    }
}

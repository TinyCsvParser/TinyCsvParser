// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using System.Globalization;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.Test.TypeConverter
{
    [TestFixture]
    public class NullableUInt16ConverterTest : BaseConverterTest<UInt16?>
    {
        protected override ITypeConverter<UInt16?> Converter
        {
            get { return new NullableUInt16Converter(); }
        }

        protected override Tuple<string, UInt16?>[] SuccessTestData
        {
            get
            {
                return new[] {
                    MakeTuple(UInt16.MinValue.ToString(), UInt16.MinValue),
                    MakeTuple(UInt16.MaxValue.ToString(), UInt16.MaxValue),
                    MakeTuple("0", 0),
                    MakeTuple("1000", 1000),
                    MakeTuple(" ", default(UInt16?)),
                    MakeTuple(null, default(UInt16?)),
                    MakeTuple(string.Empty, default(UInt16?))
                };
            }
        }

        protected override string[] FailTestData
        {
            get { return new[] { "a", "-1000", Int16.MinValue.ToString() }; }
        }
    }

    [TestFixture]
    public class NullableUInt16ConverterWithFormatProviderTest : NullableUInt16ConverterTest
    {
        protected override ITypeConverter<UInt16?> Converter
        {
            get { return new NullableUInt16Converter(CultureInfo.InvariantCulture); }
        }
    }

    [TestFixture]
    public class NullableUInt16ConverterWithFormatProviderAndNumberStylesTest : NullableUInt16ConverterTest
    {
        protected override ITypeConverter<UInt16?> Converter
        {
            get { return new NullableUInt16Converter(CultureInfo.InvariantCulture, NumberStyles.Integer); }
        }
    }
}

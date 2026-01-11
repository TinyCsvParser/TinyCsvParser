// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using System.Globalization;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.Test.TypeConverter;

[TestFixture]
public class NullableUInt32ConverterTest : BaseConverterTest<uint?>
{
    protected override ITypeConverter<uint?> Converter => new NullableUInt32Converter();

    protected override Tuple<string, uint?>[] SuccessTestData =>
    [
        MakeTuple(uint.MinValue.ToString(), uint.MinValue),
        MakeTuple(uint.MaxValue.ToString(), uint.MaxValue),
        MakeTuple("0", 0),
        MakeTuple("1000", 1000),
        MakeTuple(" ", null),
        MakeTuple(null, null),
        MakeTuple(string.Empty, null)
    ];

    protected override string[] FailTestData => ["a", "-1000", short.MinValue.ToString()];
}

[TestFixture]
public class NullableUInt32ConverterWithFormatProviderTest : NullableUInt32ConverterTest
{
    protected override ITypeConverter<uint?> Converter => new NullableUInt32Converter(CultureInfo.InvariantCulture);
}

[TestFixture]
public class NullableUInt32ConverterWithFormatProviderAndNumberStylesTest : NullableUInt32ConverterTest
{
    protected override ITypeConverter<uint?> Converter => new NullableUInt32Converter(CultureInfo.InvariantCulture, NumberStyles.Integer);
}
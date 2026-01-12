// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using System.Globalization;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.Test.TypeConverter;

[TestFixture]
public class NullableUInt64ConverterTest : BaseConverterTest<ulong?>
{
    protected override ITypeConverter<ulong?> Converter => new NullableUInt64Converter();

    protected override Tuple<string, ulong?>[] SuccessTestData =>
    [
        MakeTuple(ulong.MinValue.ToString(), ulong.MinValue),
        MakeTuple(ulong.MaxValue.ToString(), ulong.MaxValue),
        MakeTuple("0", 0),
        MakeTuple("1000", 1000),
        MakeTuple(" ", null),
        MakeTuple(null, null),
        MakeTuple(string.Empty, null)
    ];

    protected override string[] FailTestData => ["a", "-1000", short.MinValue.ToString(), long.MinValue.ToString()];
}

[TestFixture]
public class NullableUInt64ConverterWithFormatProviderTest : NullableUInt64ConverterTest
{
    protected override ITypeConverter<ulong?> Converter => new NullableUInt64Converter(CultureInfo.InvariantCulture);
}

[TestFixture]
public class NullableUInt64ConverterWithFormatProviderAndNumberStylesTest : NullableUInt64ConverterTest
{
    protected override ITypeConverter<ulong?> Converter => new NullableUInt64Converter(CultureInfo.InvariantCulture, NumberStyles.Integer);
}
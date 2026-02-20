// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using System.Globalization;
using TinyCsvParser.TypeConverters;

namespace TinyCsvParser.Test.TypeConverter;

[TestFixture]
public class NullableInt64ConverterTest : BaseConverterTest<long?>
{
    protected override ITypeConverter<long?> Converter => new NullableInt64Converter();

    protected override Tuple<string, long?>[] SuccessTestData =>
    [
        MakeTuple(long.MinValue.ToString(), long.MinValue),
        MakeTuple(long.MaxValue.ToString(), long.MaxValue),
        MakeTuple("0", 0),
        MakeTuple("-1000", -1000),
        MakeTuple("1000", 1000),
        MakeTuple(" ", null),
        MakeTuple(null, null),
        MakeTuple(string.Empty, null)
    ];

    protected override string[] FailTestData => ["a"];
}

[TestFixture]
public class NullableInt64ConverterWithFormatProviderTest : NullableInt64ConverterTest
{
    protected override ITypeConverter<long?> Converter => new NullableInt64Converter(CultureInfo.InvariantCulture);
}

[TestFixture]
public class NullableInt64ConverterWithFormatProviderAndNumberStylesTest : NullableInt64ConverterTest
{
    protected override ITypeConverter<long?> Converter => new NullableInt64Converter(CultureInfo.InvariantCulture, NumberStyles.Integer);
}
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using System.Globalization;

namespace TinyCsvParser.Test.TypeConverter;

[TestFixture]
public class NullableDecimalConverterTest : BaseConverterTest<decimal?>
{
    protected override ITypeConverter<decimal?> Converter => new NullableDecimalConverter();

    protected override Tuple<string, decimal?>[] SuccessTestData =>
    [
        MakeTuple(decimal.MinValue.ToString(CultureInfo.InvariantCulture), decimal.MinValue),
        MakeTuple(decimal.MaxValue.ToString(CultureInfo.InvariantCulture), decimal.MaxValue),
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
public class NullableDecimalConverterWithFormatProviderTest : NullableDecimalConverterTest
{
    protected override ITypeConverter<decimal?> Converter => new NullableDecimalConverter(CultureInfo.InvariantCulture);
}

[TestFixture]
public class NullableDecimalConverterWithFormatProviderAndNumberStylesTest : NullableDecimalConverterTest
{
    protected override ITypeConverter<decimal?> Converter => new NullableDecimalConverter(CultureInfo.InvariantCulture, NumberStyles.Number);
}
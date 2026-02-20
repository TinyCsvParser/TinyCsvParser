// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using System.Globalization;
using TinyCsvParser.TypeConverters;

namespace TinyCsvParser.Test.TypeConverter;

[TestFixture]
public class NullableInt32ConverterTest : BaseConverterTest<int?>
{
    protected override ITypeConverter<int?> Converter => new NullableInt32Converter();

    protected override Tuple<string, int?>[] SuccessTestData =>
    [
        MakeTuple(int.MinValue.ToString(), int.MinValue),
        MakeTuple(int.MaxValue.ToString(), int.MaxValue),
        MakeTuple(null, null),
        MakeTuple(string.Empty, null),
        MakeTuple("0", 0),
        MakeTuple("-1000", -1000),
        MakeTuple("1000", 1000)
    ];

    protected override string[] FailTestData => ["a"];
}

[TestFixture]
public class NullableInt32ConverterWithFormatProviderTest : NullableInt32ConverterTest
{
    protected override ITypeConverter<int?> Converter => new NullableInt32Converter(CultureInfo.InvariantCulture);
}

[TestFixture]
public class NullableInt32ConverterWithFormatProviderAndNumberStylesTest : NullableInt32ConverterTest
{
    protected override ITypeConverter<int?> Converter => new NullableInt32Converter(CultureInfo.InvariantCulture, NumberStyles.Integer);
}
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using System.Globalization;
using TinyCsvParser.TypeConverters;

namespace TinyCsvParser.Test.TypeConverter;

[TestFixture]
public class NullableInt16ConverterTest : BaseConverterTest<short?>
{
    protected override ITypeConverter<short?> Converter => new NullableInt16Converter();

    protected override Tuple<string, short?>[] SuccessTestData =>
    [
        MakeTuple(short.MinValue.ToString(), short.MinValue),
        MakeTuple(short.MaxValue.ToString(), short.MaxValue),
        MakeTuple(null, null),
        MakeTuple(string.Empty, null),
        MakeTuple("0", 0),
        MakeTuple("-1000", -1000),
        MakeTuple("1000", 1000)
    ];

    protected override string[] FailTestData => ["a", int.MinValue.ToString(), int.MaxValue.ToString()];
}

[TestFixture]
public class NullableInt16ConverterWithFormatProviderTest : NullableInt16ConverterTest
{
    protected override ITypeConverter<short?> Converter => new NullableInt16Converter(CultureInfo.InvariantCulture);
}

[TestFixture]
public class NullableInt16ConverterWithFormatProviderAndNumberStylesTest : NullableInt16ConverterTest
{
    protected override ITypeConverter<short?> Converter => new NullableInt16Converter(CultureInfo.InvariantCulture, NumberStyles.Integer);
}
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using System.Globalization;
using TinyCsvParser.Core;

namespace TinyCsvParser.Test.TypeConverter;

[TestFixture]
public class NullableUInt16ConverterTest : BaseConverterTest<ushort?>
{
    protected override ITypeConverter<ushort?> Converter => new NullableUInt16Converter();

    protected override Tuple<string, ushort?>[] SuccessTestData =>
    [
        MakeTuple(ushort.MinValue.ToString(), ushort.MinValue),
        MakeTuple(ushort.MaxValue.ToString(), ushort.MaxValue),
        MakeTuple("0", 0),
        MakeTuple("1000", 1000),
        MakeTuple(" ", null),
        MakeTuple(null, null),
        MakeTuple(string.Empty, null)
    ];

    protected override string[] FailTestData => ["a", "-1000", short.MinValue.ToString()];
}

[TestFixture]
public class NullableUInt16ConverterWithFormatProviderTest : NullableUInt16ConverterTest
{
    protected override ITypeConverter<ushort?> Converter => new NullableUInt16Converter(CultureInfo.InvariantCulture);
}

[TestFixture]
public class NullableUInt16ConverterWithFormatProviderAndNumberStylesTest : NullableUInt16ConverterTest
{
    protected override ITypeConverter<ushort?> Converter => new NullableUInt16Converter(CultureInfo.InvariantCulture, NumberStyles.Integer);
}
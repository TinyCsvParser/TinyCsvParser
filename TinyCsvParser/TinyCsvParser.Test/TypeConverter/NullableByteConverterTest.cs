// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using System.Globalization;
using TinyCsvParser.Core;

namespace TinyCsvParser.Test.TypeConverter;

[TestFixture]
public class NullableByteConverterTest : BaseConverterTest<byte?>
{
    protected override ITypeConverter<byte?> Converter => new NullableByteConverter();

    protected override Tuple<string, byte?>[] SuccessTestData =>
    [
        MakeTuple(byte.MinValue.ToString(), byte.MinValue),
        MakeTuple(byte.MaxValue.ToString(), byte.MaxValue),
        MakeTuple("0", 0),
        MakeTuple("255", 255),
        MakeTuple(" ", null),
        MakeTuple(null, null),
        MakeTuple(string.Empty, null)
    ];

    protected override string[] FailTestData => ["a", "-1", "256"];
}

[TestFixture]
public class NullableByteConverterWithFormatProviderTest : NullableByteConverterTest
{
    protected override ITypeConverter<byte?> Converter => new NullableByteConverter(CultureInfo.InvariantCulture);
}

[TestFixture]
public class NullableByteConverterWithFormatProviderAndNumberStylesTest : NullableByteConverterTest
{
    protected override ITypeConverter<byte?> Converter => new NullableByteConverter(CultureInfo.InvariantCulture, NumberStyles.Integer);
}
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using System.Globalization;
using TinyCsvParser.Core;

namespace TinyCsvParser.Test.TypeConverter;

[TestFixture]
public class NullableSByteConverterTest : BaseConverterTest<sbyte?>
{
    protected override ITypeConverter<sbyte?> Converter => new NullableSByteConverter();

    protected override Tuple<string, sbyte?>[] SuccessTestData =>
    [
        MakeTuple(sbyte.MinValue.ToString(), sbyte.MinValue),
        MakeTuple(sbyte.MaxValue.ToString(), sbyte.MaxValue),
        MakeTuple("0", 0),
        MakeTuple("-128", -128),
        MakeTuple("127", 127),
        MakeTuple(" ", null),
        MakeTuple(null, null),
        MakeTuple(string.Empty, null)
    ];

    protected override string[] FailTestData => ["a", "-129", "128"];
}

[TestFixture]
public class NullableSByteConverterWithFormatProviderTest : NullableSByteConverterTest
{
    protected override ITypeConverter<sbyte?> Converter => new NullableSByteConverter(CultureInfo.InvariantCulture);
}

[TestFixture]
public class NullableSByteConverterWithFormatProviderAndNumberStylesTest : NullableSByteConverterTest
{
    protected override ITypeConverter<sbyte?> Converter => new NullableSByteConverter(CultureInfo.InvariantCulture, NumberStyles.Integer);
}
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using System.Globalization;
using TinyCsvParser.TypeConverters;

namespace TinyCsvParser.Test.TypeConverter;

[TestFixture]
public class NullableSingleConverterTest : BaseConverterTest<float?>
{
    protected override ITypeConverter<float?> Converter => new NullableSingleConverter();

    protected override Tuple<string, float?>[] SuccessTestData =>
    [
        MakeTuple(float.MinValue.ToString("R"), float.MinValue),
        MakeTuple(float.MaxValue.ToString("R"), float.MaxValue),
        MakeTuple("0", 0),
        MakeTuple("-1000", -1000),
        MakeTuple("1000", 1000),
        MakeTuple("5e2", 500),
        MakeTuple(" ", null),
        MakeTuple(null, null),
        MakeTuple(string.Empty, null)
    ];

    protected override void AssertAreEqual(float? expected, float? actual)
    {
        if (expected is null)
        {
            Assert.AreEqual(null, actual);
        }
        else
        {
            NUnit.Framework.Assert.That(actual, Is.EqualTo(expected.Value).Within(float.Epsilon));
        }
    }

    protected override string[] FailTestData => ["a"];
}

[TestFixture]
public class NullableSingleConverterWithFormatProviderTest : NullableSingleConverterTest
{
    protected override ITypeConverter<float?> Converter =>
        new NullableSingleConverter(CultureInfo.InvariantCulture);
}

[TestFixture]
public class NullableSingleConverterWithFormatProviderAndNumberStyleTest : NullableSingleConverterTest
{
    protected override ITypeConverter<float?> Converter => new NullableSingleConverter(CultureInfo.InvariantCulture,
        NumberStyles.Float | NumberStyles.AllowThousands);
}
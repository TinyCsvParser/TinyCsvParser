// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.Test.TypeConverter;

public enum TestNullableEnum
{
    A = 1
}

[TestFixture]
public class NullableEnumConverterCaseSensitiveTest : BaseConverterTest<TestNullableEnum?>
{

    protected override ITypeConverter<TestNullableEnum?> Converter => new NullableEnumConverter<TestNullableEnum>(false);

    protected override Tuple<string, TestNullableEnum?>[] SuccessTestData =>
    [
        MakeTuple("A", TestNullableEnum.A),
        MakeTuple(null, null),
        MakeTuple(string.Empty, null),
        MakeTuple(" ", null)
    ];

    protected override string[] FailTestData => ["B", "a"];
}

public class NullableEnumConverterCaseInsensitiveTest : BaseConverterTest<TestNullableEnum?>
{

    protected override ITypeConverter<TestNullableEnum?> Converter => new NullableEnumConverter<TestNullableEnum>(true);

    protected override Tuple<string, TestNullableEnum?>[] SuccessTestData =>
    [
        MakeTuple("A", TestNullableEnum.A),
        MakeTuple("a", TestNullableEnum.A),
        MakeTuple(null, null),
        MakeTuple(string.Empty, null),
        MakeTuple(" ", null)
    ];

    protected override string[] FailTestData => ["B"];
}

[TestFixture]
public class NullableEnumConverterGeneralTest
{
    private struct NoEnum;

    [Test]
    public void CouldNotInstantiateNonEnumTest()
    {
        NUnit.Framework.Assert.Throws<ArgumentException>(() => new EnumConverter<NoEnum>());
    }
}
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using TinyCsvParser.TypeConverters;

namespace TinyCsvParser.Test.TypeConverter;

public enum TestNullableEnum
{
    A = 1
}

[TestFixture]
public class NullableEnumConverterCaseSensitiveTest : BaseConverterTest<TestNullableEnum?>
{

    protected override ITypeConverter<TestNullableEnum?> Converter => new NullableEnumConverter<TestNullableEnum>(StringComparison.CurrentCulture);

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

    protected override ITypeConverter<TestNullableEnum?> Converter => new NullableEnumConverter<TestNullableEnum>();

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
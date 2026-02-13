// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;

namespace TinyCsvParser.Test.TypeConverter;

[TestFixture]
public class BoolConverterTest : BaseConverterTest<bool>
{
    protected override ITypeConverter<bool> Converter => new BoolConverter();

    protected override Tuple<string, bool>[] SuccessTestData =>
    [
        MakeTuple("true", true),
        MakeTuple("false", false),
        MakeTuple("True", true),
        MakeTuple("False", false)
    ];

    protected override string[] FailTestData => ["a", string.Empty, "  ", null];
}

[TestFixture]
public class BoolConverterNonDefaultTest : BaseConverterTest<bool>
{
    protected override ITypeConverter<bool> Converter =>
        new BoolConverter("ThisIsTrue", "ThisIsFalse", StringComparison.Ordinal);

    protected override Tuple<string, bool>[] SuccessTestData =>
    [
        MakeTuple("ThisIsTrue", true),
        MakeTuple("ThisIsFalse", false)
    ];

    protected override string[] FailTestData => ["a", string.Empty, "  ", null, "thisistrue", "thisisfalse"];
}
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;

namespace TinyCsvParser.Test.TypeConverter;

[TestFixture]
public class NullableGuidConverterTest : BaseConverterTest<Guid?>
{
    protected override ITypeConverter<Guid?> Converter => new NullableGuidConverter();

    protected override Tuple<string, Guid?>[] SuccessTestData =>
    [
        MakeTuple("02001000-0010-0000-0000-003200000000", Guid.Parse("02001000-0010-0000-0000-003200000000")),
        MakeTuple(null, null),
        MakeTuple(string.Empty, null)
    ];

    protected override string[] FailTestData => ["a", int.MinValue.ToString(), int.MaxValue.ToString()];
}

[TestFixture]
public class NullableGuidConverterWithFormatTest : NullableGuidConverterTest
{
    protected override ITypeConverter<Guid?> Converter => new NullableGuidConverter("D");
}
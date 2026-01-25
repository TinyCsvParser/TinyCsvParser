// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using TinyCsvParser.Core;

namespace TinyCsvParser.Test.TypeConverter;

[TestFixture]
public class GuidConverterTest : BaseConverterTest<Guid>
{
    protected override ITypeConverter<Guid> Converter => new GuidConverter();

    protected override Tuple<string, Guid>[] SuccessTestData =>
    [
        MakeTuple("02001000-0010-0000-0000-003200000000", Guid.Parse("02001000-0010-0000-0000-003200000000"))
    ];

    protected override string[] FailTestData => ["a", string.Empty, "  ", null];
}

[TestFixture]
public class GuidConverterWithFormatTest : GuidConverterTest
{
    protected override ITypeConverter<Guid> Converter => new GuidConverter("D");
}
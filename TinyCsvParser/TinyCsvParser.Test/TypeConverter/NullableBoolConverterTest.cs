// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using TinyCsvParser.Core;

namespace TinyCsvParser.Test.TypeConverter;

[TestFixture]
public class NullableBoolConverterTest : BaseConverterTest<bool?>
{
    protected override ITypeConverter<bool?> Converter => new NullableBoolConverter();

    protected override Tuple<string, bool?>[] SuccessTestData =>
    [
        MakeTuple("true", true),
        MakeTuple("false", false),
        MakeTuple(null, null),
        MakeTuple(string.Empty, null)
    ];

    protected override string[] FailTestData => ["a"];
}

[TestFixture]
public class NullableBoolConverterWithFormatConstructorTest : NullableBoolConverterTest
{
    protected override ITypeConverter<bool?> Converter => new NullableBoolConverter("true", "false", StringComparison.OrdinalIgnoreCase);
}
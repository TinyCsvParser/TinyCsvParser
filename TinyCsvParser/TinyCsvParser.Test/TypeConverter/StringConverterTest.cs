// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using TinyCsvParser.Core;

namespace TinyCsvParser.Test.TypeConverter;

[TestFixture]
public class StringConverterTest : BaseConverterTest<string>
{
    protected override ITypeConverter<string> Converter => new StringConverter();

    protected override Tuple<string, string>[] SuccessTestData =>
    [
        MakeTuple(string.Empty, string.Empty),
        MakeTuple(" ", " "),
        MakeTuple("Abc", "Abc"),
    ];

    protected override string[] FailTestData => []; // Should never fail, because values are passed through.
}
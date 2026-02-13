// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;

namespace TinyCsvParser.Test.TypeConverter;

[TestFixture]
public class UInt64ConverterTest : BaseConverterTest<ulong>
{
    protected override ITypeConverter<ulong> Converter => new UInt64Converter();

    protected override Tuple<string, ulong>[] SuccessTestData =>
    [
        MakeTuple(ulong.MinValue.ToString(), ulong.MinValue),
        MakeTuple(ulong.MaxValue.ToString(), ulong.MaxValue),
        MakeTuple("0", 0),
        MakeTuple("1000", 1000)
    ];

    protected override string[] FailTestData => ["a", "-1000", string.Empty, "  ", null, short.MinValue.ToString(), long.MinValue.ToString()];
}
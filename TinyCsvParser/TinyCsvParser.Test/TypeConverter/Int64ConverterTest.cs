// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using TinyCsvParser.Core;

namespace TinyCsvParser.Test.TypeConverter;

[TestFixture]
public class Int64ConverterTest : BaseConverterTest<long>
{
    protected override ITypeConverter<long> Converter => new Int64Converter();

    protected override Tuple<string, long>[] SuccessTestData =>
    [
        MakeTuple(long.MinValue.ToString(), long.MinValue),
        MakeTuple(long.MaxValue.ToString(), long.MaxValue),
        MakeTuple("0", 0),
        MakeTuple("-1000", -1000),
        MakeTuple("1000", 1000)
    ];

    protected override string[] FailTestData => ["a", string.Empty, "  ", null];
}
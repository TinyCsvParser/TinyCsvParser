// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using TinyCsvParser.TypeConverters;

namespace TinyCsvParser.Test.TypeConverter;

[TestFixture]
public class Int32ConverterTest : BaseConverterTest<int>
{
    protected override ITypeConverter<int> Converter => new Int32Converter();

    protected override Tuple<string, int>[] SuccessTestData =>
    [
        MakeTuple(int.MinValue.ToString(), int.MinValue),
        MakeTuple(int.MaxValue.ToString(), int.MaxValue),
        MakeTuple("0", 0),
        MakeTuple("-1000", -1000),
        MakeTuple("1000", 1000)
    ];

    protected override string[] FailTestData => ["a", string.Empty, "  ", null];
}
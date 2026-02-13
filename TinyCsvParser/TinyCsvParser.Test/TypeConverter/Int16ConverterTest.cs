// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;

namespace TinyCsvParser.Test.TypeConverter;

[TestFixture]
public class Int16ConverterTest : BaseConverterTest<short>
{
    protected override ITypeConverter<short> Converter => new Int16Converter();

    protected override Tuple<string, short>[] SuccessTestData =>
    [
        MakeTuple(short.MinValue.ToString(), short.MinValue),
        MakeTuple(short.MaxValue.ToString(), short.MaxValue),
        MakeTuple("0", 0),
        MakeTuple("-1000", -1000),
        MakeTuple("1000", 1000)
    ];

    protected override string[] FailTestData => ["a", string.Empty, "  ", int.MinValue.ToString(), int.MaxValue.ToString(), null];
}
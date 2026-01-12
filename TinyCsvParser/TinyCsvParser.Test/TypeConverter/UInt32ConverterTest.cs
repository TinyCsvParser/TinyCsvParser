// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.Test.TypeConverter;

[TestFixture]
public class UInt32ConverterTest : BaseConverterTest<uint>
{
    protected override ITypeConverter<uint> Converter => new UInt32Converter();

    protected override Tuple<string, uint>[] SuccessTestData =>
    [
        MakeTuple(uint.MinValue.ToString(), uint.MinValue),
        MakeTuple(uint.MaxValue.ToString(), uint.MaxValue),
        MakeTuple("0", 0),
        MakeTuple("1000", 1000)
    ];

    protected override string[] FailTestData => ["a", "-1000", string.Empty, "  ", null, int.MinValue.ToString()];
}
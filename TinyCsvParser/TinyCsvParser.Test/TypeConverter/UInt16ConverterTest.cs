// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using TinyCsvParser.TypeConverters;

namespace TinyCsvParser.Test.TypeConverter;

[TestFixture]
public class UInt16ConverterTest : BaseConverterTest<ushort>
{
    protected override ITypeConverter<ushort> Converter => new UInt16Converter();

    protected override Tuple<string, ushort>[] SuccessTestData =>
    [
        MakeTuple(ushort.MinValue.ToString(), ushort.MinValue),
        MakeTuple(ushort.MaxValue.ToString(), ushort.MaxValue),
        MakeTuple("0", 0),
        MakeTuple("1000", 1000)
    ];

    protected override string[] FailTestData => ["a", "-1000", string.Empty, "  ", null, short.MinValue.ToString()];
}
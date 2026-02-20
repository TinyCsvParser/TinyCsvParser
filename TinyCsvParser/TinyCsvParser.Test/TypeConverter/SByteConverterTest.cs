// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using TinyCsvParser.TypeConverters;

namespace TinyCsvParser.Test.TypeConverter;

[TestFixture]
public class SByteConverterTest : BaseConverterTest<sbyte>
{
    protected override ITypeConverter<sbyte> Converter => new SByteConverter();

    protected override Tuple<string, sbyte>[] SuccessTestData =>
    [
        MakeTuple(sbyte.MinValue.ToString(), sbyte.MinValue),
        MakeTuple(sbyte.MaxValue.ToString(), sbyte.MaxValue),
        MakeTuple("0", 0),
        MakeTuple("-128", -128),
        MakeTuple("127", 127)
    ];

    protected override string[] FailTestData => ["a", string.Empty, "  ", null, "-129", "128"];
}
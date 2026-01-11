// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.Test.TypeConverter;

[TestFixture]
public class ByteConverterTest : BaseConverterTest<byte>
{
    protected override ITypeConverter<byte> Converter => new ByteConverter();

    protected override Tuple<string, byte>[] SuccessTestData =>
    [
        MakeTuple(byte.MinValue.ToString(), byte.MinValue),
        MakeTuple(byte.MaxValue.ToString(), byte.MaxValue),
        MakeTuple("0", 0),
        MakeTuple("255", 255)
    ];

    protected override string[] FailTestData => ["a", string.Empty, "  ", null, "-1", "256"];
}
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using TinyCsvParser.Core;

namespace TinyCsvParser.Test.TypeConverter;

[TestFixture]
public class DoubleConverterTest : BaseConverterTest<double>
{
    protected override ITypeConverter<double> Converter => new DoubleConverter();

    protected override Tuple<string, double>[] SuccessTestData =>
    [
        MakeTuple(double.MinValue.ToString("R"), double.MinValue),
        MakeTuple(double.MaxValue.ToString("R"), double.MaxValue),
        MakeTuple("0", 0),
        MakeTuple("-1000", -1000),
        MakeTuple("1000", 1000),
        MakeTuple("5e2", 500)
    ];

    protected override void AssertAreEqual(double expected, double actual)
    {
        Assert.AreEqual(expected, actual, 1e-5);
    }

    protected override string[] FailTestData => ["a", string.Empty, "  ", null];
}
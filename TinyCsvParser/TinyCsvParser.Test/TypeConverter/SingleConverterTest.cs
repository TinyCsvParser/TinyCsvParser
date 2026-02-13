// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;

namespace TinyCsvParser.Test.TypeConverter;

[TestFixture]
public class SingleConverterTest : BaseConverterTest<float>
{
    protected override ITypeConverter<float> Converter => new SingleConverter();

    protected override Tuple<string, float>[] SuccessTestData =>
    [
        MakeTuple(float.MinValue.ToString("R"), float.MinValue),
        MakeTuple(float.MaxValue.ToString("R"), float.MaxValue),
        MakeTuple("0", 0),
        MakeTuple("-1000", -1000),
        MakeTuple("1000", 1000),
        MakeTuple("5e2", 500)
    ];

    protected override void AssertAreEqual(float expected, float actual)
    {
        NUnit.Framework.Assert.That(actual, Is.EqualTo(expected).Within(float.Epsilon));
    }

    protected override string[] FailTestData => ["a", string.Empty, "  ", null];
}
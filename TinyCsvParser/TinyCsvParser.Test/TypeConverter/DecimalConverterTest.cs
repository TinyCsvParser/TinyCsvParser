// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using System.Globalization;
using TinyCsvParser.TypeConverters;

namespace TinyCsvParser.Test.TypeConverter;

[TestFixture]
public class DecimalConverterTest : BaseConverterTest<decimal>
{
    protected override ITypeConverter<decimal> Converter => new DecimalConverter();

    protected override Tuple<string, decimal>[] SuccessTestData =>
    [
        MakeTuple(decimal.MinValue.ToString(CultureInfo.InvariantCulture), decimal.MinValue),
        MakeTuple(decimal.MaxValue.ToString(CultureInfo.InvariantCulture), decimal.MaxValue),
        MakeTuple("0", 0),
        MakeTuple("-1000", -1000),
        MakeTuple("1000", 1000)
    ];

    protected override string[] FailTestData => ["a", string.Empty, "  ", null];
}
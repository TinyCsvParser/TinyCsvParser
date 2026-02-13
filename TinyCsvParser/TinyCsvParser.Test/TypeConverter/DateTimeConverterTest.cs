// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;

namespace TinyCsvParser.Test.TypeConverter;

[TestFixture]
public class DateTimeConverterTest : BaseConverterTest<DateTime>
{
    protected override ITypeConverter<DateTime> Converter => new DateTimeConverter();

    protected override Tuple<string, DateTime>[] SuccessTestData =>
    [
        MakeTuple("2014/01/01", new DateTime(2014, 1, 1)),
        MakeTuple("9999/12/31", new DateTime(9999, 12, 31))
    ];

    protected override string[] FailTestData => ["a", string.Empty, "  ", null, "10000/01/01", "1753/01/32", "0/0/0"];
}
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using TinyCsvParser.TypeConverters;

namespace TinyCsvParser.Test.TypeConverter;

[TestFixture]
public class TimeSpanConverterTest : BaseConverterTest<TimeSpan>
{
    protected override ITypeConverter<TimeSpan> Converter => new TimeSpanConverter();

    protected override Tuple<string, TimeSpan>[] SuccessTestData =>
    [
        MakeTuple(TimeSpan.MinValue.ToString(), TimeSpan.MinValue),
        MakeTuple("14", TimeSpan.FromDays(14)),
        MakeTuple("1:2:3", TimeSpan.FromHours(1).Add(TimeSpan.FromMinutes(2)).Add(TimeSpan.FromSeconds(3)))
    ];

    protected override string[] FailTestData => ["a", string.Empty, "  ", null];
}

[TestFixture]
public class TimeSpanConverterCustomFormatTest : BaseConverterTest<TimeSpan>
{
    protected override ITypeConverter<TimeSpan> Converter => new TimeSpanConverter(@"hh\:mm\:ss");

    protected override Tuple<string, TimeSpan>[] SuccessTestData =>
    [
        MakeTuple("01:02:03", TimeSpan.FromHours(1).Add(TimeSpan.FromMinutes(2)).Add(TimeSpan.FromSeconds(3)))
    ];

    protected override string[] FailTestData => ["a", string.Empty, "  ", null];
}
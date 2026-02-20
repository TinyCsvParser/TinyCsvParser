// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using System.Globalization;
using TinyCsvParser.TypeConverters;

namespace TinyCsvParser.Test.TypeConverter;

[TestFixture]
public class NullableTimeSpanConverterTest : BaseConverterTest<TimeSpan?>
{
    protected override ITypeConverter<TimeSpan?> Converter => new NullableTimeSpanConverter();

    protected override Tuple<string, TimeSpan?>[] SuccessTestData =>
    [
        MakeTuple(TimeSpan.MinValue.ToString(), TimeSpan.MinValue),
        MakeTuple("14", TimeSpan.FromDays(14)),
        MakeTuple("1:2:3", TimeSpan.FromHours(1).Add(TimeSpan.FromMinutes(2)).Add(TimeSpan.FromSeconds(3))),
        MakeTuple(" ", null),
        MakeTuple(null, null),
        MakeTuple(string.Empty, null)
    ];

    protected override string[] FailTestData => ["a"];
}

[TestFixture]
public class NullableTimeSpanConverterWithFormatProviderTest : NullableTimeSpanConverterTest
{
    protected override ITypeConverter<TimeSpan?> Converter => new NullableTimeSpanConverter(string.Empty);
}

[TestFixture]
public class NullableTimeSpanConverterWithFormatAndFormatProviderTest : NullableTimeSpanConverterTest
{
    protected override ITypeConverter<TimeSpan?> Converter => new NullableTimeSpanConverter(string.Empty, CultureInfo.InvariantCulture);
}

[TestFixture]
public class NullableTimeSpanConverterWithFormatAndFormatProviderAndTimeSpanStyleTest : NullableTimeSpanConverterTest
{
    protected override ITypeConverter<TimeSpan?> Converter => new NullableTimeSpanConverter(string.Empty, CultureInfo.InvariantCulture, TimeSpanStyles.None);
}
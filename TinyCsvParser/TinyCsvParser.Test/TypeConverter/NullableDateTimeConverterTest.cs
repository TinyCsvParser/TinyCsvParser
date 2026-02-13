// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using System.Globalization;

namespace TinyCsvParser.Test.TypeConverter;

[TestFixture]
public class NullableDateTimeConverterTest : BaseConverterTest<DateTime?>
{
    protected override ITypeConverter<DateTime?> Converter => new NullableDateTimeConverter();

    protected override Tuple<string, DateTime?>[] SuccessTestData =>
    [
        MakeTuple("2014/01/01", new DateTime(2014, 1, 1)),
        MakeTuple("9999/12/31", new DateTime(9999, 12, 31)),
        MakeTuple(" ", null),
        MakeTuple(null, null),
        MakeTuple(string.Empty, null)
    ];

    protected override string[] FailTestData => ["a", "10000/01/01", "1753/01/32", "0/0/0"];
}

[TestFixture]
public class NullableDateTimeConverterWithFormatTest : NullableDateTimeConverterTest
{
    protected override ITypeConverter<DateTime?> Converter => new NullableDateTimeConverter(string.Empty);
}

[TestFixture]
public class NullableDateTimeConverterWithFormatAndCultureInfoTest : NullableDateTimeConverterTest
{
    protected override ITypeConverter<DateTime?> Converter => new NullableDateTimeConverter(string.Empty, CultureInfo.InvariantCulture);
}

[TestFixture]
public class NullableDateTimeConverterWithFormatAndCultureInfoAndDateTimeStylesTest : NullableDateTimeConverterTest
{
    protected override ITypeConverter<DateTime?> Converter => new NullableDateTimeConverter(string.Empty, CultureInfo.InvariantCulture, DateTimeStyles.None);
}
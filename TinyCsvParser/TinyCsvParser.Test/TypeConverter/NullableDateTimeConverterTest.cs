// Copyright (c) Philipp Wagner and Joel Mueller. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using NUnit.Framework;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.Test.TypeConverter
{
    [TestFixture]
    public class NullableDateTimeConverterTest : BaseConverterTest<DateTime?>
    {
        protected override ITypeConverter<DateTime?> Converter
        {
            get { return new NullableDateTimeConverter(); }
        }

        protected override (string, DateTime?)[] SuccessTestData
        {
            get
            {
                return new[] {
                    ("2014/01/01", new DateTime(2014, 1, 1)),
                    ("9999/12/31", new DateTime(9999, 12, 31)),
                    (" ", default(DateTime?)),
                    (null, default(DateTime?)),
                    (string.Empty, default(DateTime?))
                };
            }
        }

        protected override string[] FailTestData
        {
            get { return new[] { "a", "10000/01/01", "1753/01/32", "0/0/0" }; }
        }
    }

    [TestFixture]
    public class NullableDateTimeConverterWithFormatTest : NullableDateTimeConverterTest
    {
        protected override ITypeConverter<DateTime?> Converter
        {
            get { return new NullableDateTimeConverter(string.Empty); }
        }
    }

    [TestFixture]
    public class NullableDateTimeConverterWithFormatAndCultureInfoTest : NullableDateTimeConverterTest
    {
        protected override ITypeConverter<DateTime?> Converter
        {
            get { return new NullableDateTimeConverter(string.Empty, CultureInfo.InvariantCulture); }
        }
    }

    [TestFixture]
    public class NullableDateTimeConverterWithFormatAndCultureInfoAndDateTimeStylesTest : NullableDateTimeConverterTest
    {
        protected override ITypeConverter<DateTime?> Converter
        {
            get { return new NullableDateTimeConverter(string.Empty, CultureInfo.InvariantCulture, DateTimeStyles.None); }
        }
    }

}

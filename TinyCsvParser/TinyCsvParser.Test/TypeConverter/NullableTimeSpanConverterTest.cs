// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.Test.TypeConverter
{
    [TestClass]
    public class NullableTimeSpanConverterTest : BaseConverterTest<TimeSpan?>
    {
        protected override ITypeConverter<TimeSpan?> Converter
        {
            get { return new NullableTimeSpanConverter(); }
        }

        protected override Tuple<string, TimeSpan?>[] SuccessTestData
        {
            get
            {
                return new[] {
                    MakeTuple(TimeSpan.MinValue.ToString(), TimeSpan.MinValue),
                    MakeTuple("14", TimeSpan.FromDays(14)),
                    MakeTuple("1:2:3", TimeSpan.FromHours(1).Add(TimeSpan.FromMinutes(2)).Add(TimeSpan.FromSeconds(3))),
                    MakeTuple(" ", default(TimeSpan?)),
                    MakeTuple(null, default(TimeSpan?)),
                    MakeTuple(string.Empty, default(TimeSpan?))
                };
            }
        }

        protected override string[] FailTestData
        {
            get { return new[] { "a" }; }
        }
    }

    [TestClass]
    public class NullableTimeSpanConverterWithFormatProviderTest : NullableTimeSpanConverterTest
    {
        protected override ITypeConverter<TimeSpan?> Converter
        {
            get { return new NullableTimeSpanConverter(string.Empty); }
        }
    }

    [TestClass]
    public class NullableTimeSpanConverterWithFormatAndFormatProviderTest : NullableTimeSpanConverterTest
    {
        protected override ITypeConverter<TimeSpan?> Converter
        {
            get { return new NullableTimeSpanConverter(string.Empty, CultureInfo.InvariantCulture); }
        }
    }

    [TestClass]
    public class NullableTimeSpanConverterWithFormatAndFormatProviderAndTimeSpanStyleTest : NullableTimeSpanConverterTest
    {
        protected override ITypeConverter<TimeSpan?> Converter
        {
            get { return new NullableTimeSpanConverter(string.Empty, CultureInfo.InvariantCulture, TimeSpanStyles.None); }
        }
    }

}

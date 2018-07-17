// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using System.Globalization;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.Test.TypeConverter
{
    [TestFixture]
    public class NullableDecimalConverterTest : BaseConverterTest<Decimal?>
    {
        protected override ITypeConverter<Decimal?> Converter
        {
            get { return new NullableDecimalConverter(); }
        }

        protected override (string, Decimal?)[] SuccessTestData
        {
            get
            {
                return new[] {
                    (Decimal.MinValue.ToString(), Decimal.MinValue),
                    (Decimal.MaxValue.ToString(), Decimal.MaxValue),
                    ("0", 0),
                    ("-1000", -1000),
                    ("1000", 1000),
                    (" ", default(Decimal?)),
                    (null, default(Decimal?)),
                    (string.Empty, default(Decimal?))
                };
            }
        }

        protected override string[] FailTestData
        {
            get { return new[] { "a" }; }
        }
    }

    [TestFixture]
    public class NullableDecimalConverterWithFormatProviderTest : NullableDecimalConverterTest
    {
        protected override ITypeConverter<Decimal?> Converter
        {
            get { return new NullableDecimalConverter(CultureInfo.InvariantCulture); }
        }
    }

    [TestFixture]
    public class NullableDecimalConverterWithFormatProviderAndNumberStylesTest : NullableDecimalConverterTest
    {
        protected override ITypeConverter<Decimal?> Converter
        {
            get { return new NullableDecimalConverter(CultureInfo.InvariantCulture, NumberStyles.Number); }
        }
    }
}

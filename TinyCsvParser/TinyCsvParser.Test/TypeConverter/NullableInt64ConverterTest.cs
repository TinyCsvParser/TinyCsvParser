// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using System.Globalization;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.Test.TypeConverter
{
    [TestFixture]
    public class NullableInt64ConverterTest : BaseConverterTest<Int64?>
    {
        protected override ITypeConverter<Int64?> Converter
        {
            get { return new NullableInt64Converter(); }
        }

        protected override (string, Int64?)[] SuccessTestData
        {
            get
            {
                return new[] {
                    (Int64.MinValue.ToString(), Int64.MinValue),
                    (Int64.MaxValue.ToString(), Int64.MaxValue),
                    ("0", 0),
                    ("-1000", -1000),
                    ("1000", 1000),
                    (" ", default(Int64?)),
                    (null, default(Int64?)),
                    (string.Empty, default(Int64?))
                };
            }
        }

        protected override string[] FailTestData
        {
            get { return new[] { "a" }; }
        }
    }

    [TestFixture]
    public class NullableInt64ConverterWithFormatProviderTest : NullableInt64ConverterTest
    {
        protected override ITypeConverter<Int64?> Converter
        {
            get { return new NullableInt64Converter(CultureInfo.InvariantCulture); }
        }
    }

    [TestFixture]
    public class NullableInt64ConverterWithFormatProviderAndNumberStylesTest : NullableInt64ConverterTest
    {
        protected override ITypeConverter<Int64?> Converter
        {
            get { return new NullableInt64Converter(CultureInfo.InvariantCulture, NumberStyles.Integer); }
        }
    }
}

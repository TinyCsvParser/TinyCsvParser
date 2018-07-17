// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using System.Globalization;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.Test.TypeConverter
{
    [TestFixture]
    public class NullableInt32ConverterTest : BaseConverterTest<Int32?>
    {
        protected override ITypeConverter<Int32?> Converter
        {
            get { return new NullableInt32Converter(); }
        }

        protected override (string, Int32?)[] SuccessTestData
        {
            get
            {
                return new [] {
                    (Int32.MinValue.ToString(), Int32.MinValue),
                    (Int32.MaxValue.ToString(), Int32.MaxValue),
                    (null, default(Int32?)),
                    (string.Empty, default(Int32?)),
                    ("0", 0),
                    ("-1000", -1000),
                    ("1000", 1000)
                };
            }
        }

        protected override string[] FailTestData
        {
            get { return new[] { "a" }; }
        }
    }

    [TestFixture]
    public class NullableInt32ConverterWithFormatProviderTest : NullableInt32ConverterTest
    {
        protected override ITypeConverter<Int32?> Converter
        {
            get { return new NullableInt32Converter(CultureInfo.InvariantCulture); }
        }
    }

    [TestFixture]
    public class NullableInt32ConverterWithFormatProviderAndNumberStylesTest : NullableInt32ConverterTest
    {
        protected override ITypeConverter<Int32?> Converter
        {
            get { return new NullableInt32Converter(CultureInfo.InvariantCulture, NumberStyles.Integer); }
        }
    }
}

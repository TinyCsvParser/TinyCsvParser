// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using System.Globalization;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.Test.TypeConverter
{
    [TestFixture]
    public class NullableUInt32ConverterTest : BaseConverterTest<UInt32?>
    {
        protected override ITypeConverter<UInt32?> Converter
        {
            get { return new NullableUInt32Converter(); }
        }

        protected override (string, UInt32?)[] SuccessTestData
        {
            get
            {
                return new (string, UInt32?)[] {
                    (UInt32.MinValue.ToString(), UInt32.MinValue),
                    (UInt32.MaxValue.ToString(), UInt32.MaxValue),
                    ("0", 0),
                    ("1000", 1000),
                    (" ", default(UInt32?)),
                    (null, default(UInt32?)),
                    (string.Empty, default(UInt32?))
                };
            }
        }

        protected override string[] FailTestData
        {
            get { return new[] { "a", "-1000", Int16.MinValue.ToString() }; }
        }
    }

    [TestFixture]
    public class NullableUInt32ConverterWithFormatProviderTest : NullableUInt32ConverterTest
    {
        protected override ITypeConverter<UInt32?> Converter
        {
            get { return new NullableUInt32Converter(CultureInfo.InvariantCulture); }
        }
    }

    [TestFixture]
    public class NullableUInt32ConverterWithFormatProviderAndNumberStylesTest : NullableUInt32ConverterTest
    {
        protected override ITypeConverter<UInt32?> Converter
        {
            get { return new NullableUInt32Converter(CultureInfo.InvariantCulture, NumberStyles.Integer); }
        }
    }
}

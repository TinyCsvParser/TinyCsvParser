// Copyright (c) Philipp Wagner and Joel Mueller. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using System.Globalization;
using CoreCsvParser.TypeConverter;

namespace CoreCsvParser.Test.TypeConverter
{
    [TestFixture]
    public class NullableByteConverterTest : BaseConverterTest<Byte?>
    {
        protected override ITypeConverter<Byte?> Converter
        {
            get { return new NullableByteConverter(); }
        }

        protected override (string, Byte?)[] SuccessTestData
        {
            get
            {
                return new (string, Byte?)[] {
                    (Byte.MinValue.ToString(), Byte.MinValue),
                    (Byte.MaxValue.ToString(), Byte.MaxValue),
                    ("0", 0),
                    ("255", 255),
                    (" ", default(Byte?)),
                    (null, default(Byte?)),
                    (string.Empty, default(Byte?))
                };
            }
        }

        protected override string[] FailTestData
        {
            get { return new[] { "a", "-1", "256" }; }
        }
    }

    [TestFixture]
    public class NullableByteConverterWithFormatProviderTest : NullableByteConverterTest
    {
        protected override ITypeConverter<Byte?> Converter
        {
            get { return new NullableByteConverter(CultureInfo.InvariantCulture); }
        }
    }

    [TestFixture]
    public class NullableByteConverterWithFormatProviderAndNumberStylesTest : NullableByteConverterTest
    {
        protected override ITypeConverter<Byte?> Converter
        {
            get { return new NullableByteConverter(CultureInfo.InvariantCulture, NumberStyles.Integer); }
        }
    }

}

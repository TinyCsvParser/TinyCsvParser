// Copyright (c) Philipp Wagner and Joel Mueller. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using System.Globalization;
using CoreCsvParser.TypeConverter;

namespace CoreCsvParser.Test.TypeConverter
{
    [TestFixture]
    public class NullableSByteConverterTest : BaseConverterTest<SByte?>
    {
        protected override ITypeConverter<SByte?> Converter
        {
            get { return new NullableSByteConverter(); }
        }

        protected override (string, SByte?)[] SuccessTestData
        {
            get
            {
                return new (string, SByte?)[] {
                    (SByte.MinValue.ToString(), SByte.MinValue),
                    (SByte.MaxValue.ToString(), SByte.MaxValue),
                    ("0", 0),
                    ("-128", -128),
                    ("127", 127),
                    (" ", default(SByte?)),
                    (null, default(SByte?)),
                    (string.Empty, default(SByte?))
                };
            }
        }

        protected override string[] FailTestData
        {
            get { return new[] { "a", "-129", "128" }; }
        }
    }

    [TestFixture]
    public class NullableSByteConverterWithFormatProviderTest : NullableSByteConverterTest
    {
        protected override ITypeConverter<SByte?> Converter
        {
            get { return new NullableSByteConverter(CultureInfo.InvariantCulture); }
        }
    }

    [TestFixture]
    public class NullableSByteConverterWithFormatProviderAndNumberStylesTest : NullableSByteConverterTest
    {
        protected override ITypeConverter<SByte?> Converter
        {
            get { return new NullableSByteConverter(CultureInfo.InvariantCulture, NumberStyles.Integer); }
        }
    }
}

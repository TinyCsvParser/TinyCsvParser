// Copyright (c) Philipp Wagner and Joel Mueller. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using System.Globalization;
using CoreCsvParser.TypeConverter;

namespace CoreCsvParser.Test.TypeConverter
{
    [TestFixture]
    public class NullableInt16ConverterTest : BaseConverterTest<Int16?>
    {
        protected override ITypeConverter<Int16?> Converter
        {
            get { return new NullableInt16Converter(); }
        }

        protected override (string, Int16?)[] SuccessTestData
        {
            get
            {
                return new (string, Int16?)[] {
                    (Int16.MinValue.ToString(), Int16.MinValue),
                    (Int16.MaxValue.ToString(), Int16.MaxValue),
                    (null, default(Int16?)),
                    (string.Empty, default(Int16?)),
                    ("0", 0),
                    ("-1000", -1000),
                    ("1000", 1000)
                };
            }
        }

        protected override string[] FailTestData
        {
            get { return new[] { "a", Int32.MinValue.ToString(), Int32.MaxValue.ToString() }; }
        }
    }

    [TestFixture]
    public class NullableInt16ConverterWithFormatProviderTest : NullableInt16ConverterTest
    {
        protected override ITypeConverter<Int16?> Converter
        {
            get { return new NullableInt16Converter(CultureInfo.InvariantCulture); }
        }
    }

    [TestFixture]
    public class NullableInt16ConverterWithFormatProviderAndNumberStylesTest : NullableInt16ConverterTest
    {
        protected override ITypeConverter<Int16?> Converter
        {
            get { return new NullableInt16Converter(CultureInfo.InvariantCulture, NumberStyles.Integer); }
        }
    }

}

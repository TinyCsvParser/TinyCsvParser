// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.Test.TypeConverter
{
    [TestClass]
    public class NullableByteConverterTest : BaseConverterTest<Byte?>
    {
        protected override ITypeConverter<Byte?> Converter
        {
            get { return new NullableByteConverter(); }
        }

        protected override Tuple<string, Byte?>[] SuccessTestData
        {
            get
            {
                return new[] {
                    MakeTuple(Byte.MinValue.ToString(), Byte.MinValue),
                    MakeTuple(Byte.MaxValue.ToString(), Byte.MaxValue),
                    MakeTuple("0", 0),
                    MakeTuple("255", 255),
                    MakeTuple(" ", default(Byte?)),
                    MakeTuple(null, default(Byte?)),
                    MakeTuple(string.Empty, default(Byte?))
                };
            }
        }

        protected override string[] FailTestData
        {
            get { return new[] { "a", "-1", "256" }; }
        }
    }

    [TestClass]
    public class NullableByteConverterWithFormatProviderTest : NullableByteConverterTest
    {
        protected override ITypeConverter<Byte?> Converter
        {
            get { return new NullableByteConverter(CultureInfo.InvariantCulture); }
        }
    }

    [TestClass]
    public class NullableByteConverterWithFormatProviderAndNumberStylesTest : NullableByteConverterTest
    {
        protected override ITypeConverter<Byte?> Converter
        {
            get { return new NullableByteConverter(CultureInfo.InvariantCulture, NumberStyles.Integer); }
        }
    }

}

// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.Test.TypeConverter
{
    [TestClass]
    public class NullableInt32ConverterTest : BaseConverterTest<Int32?>
    {
        protected override ITypeConverter<Int32?> Converter
        {
            get { return new NullableInt32Converter(); }
        }

        protected override Tuple<string, Int32?>[] SuccessTestData
        {
            get
            {
                return new [] {
                    MakeTuple(Int32.MinValue.ToString(), Int32.MinValue),
                    MakeTuple(Int32.MaxValue.ToString(), Int32.MaxValue),
                    MakeTuple(null, default(Int32?)),
                    MakeTuple(string.Empty, default(Int32?)),
                    MakeTuple("0", 0),
                    MakeTuple("-1000", -1000),
                    MakeTuple("1000", 1000)
                };
            }
        }

        protected override string[] FailTestData
        {
            get { return new[] { "a" }; }
        }
    }

    [TestClass]
    public class NullableInt32ConverterWithFormatProviderTest : NullableInt32ConverterTest
    {
        protected override ITypeConverter<Int32?> Converter
        {
            get { return new NullableInt32Converter(CultureInfo.InvariantCulture); }
        }
    }

    [TestClass]
    public class NullableInt32ConverterWithFormatProviderAndNumberStylesTest : NullableInt32ConverterTest
    {
        protected override ITypeConverter<Int32?> Converter
        {
            get { return new NullableInt32Converter(CultureInfo.InvariantCulture, NumberStyles.Integer); }
        }
    }
}

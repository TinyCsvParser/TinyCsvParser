// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.Test.TypeConverter
{
    [TestClass]
    public class NullableSingleConverterTest : BaseConverterTest<Single?>
    {
        protected override ITypeConverter<Single?> Converter
        {
            get { return new NullableSingleConverter(); }
        }

        protected override Tuple<string, Single?>[] SuccessTestData
        {
            get
            {
                return new[] {
                    MakeTuple(float.MinValue.ToString("R"), float.MinValue),
                    MakeTuple(float.MaxValue.ToString("R"), float.MaxValue),
                    MakeTuple("0", 0),
                    MakeTuple("-1000", -1000),
                    MakeTuple("1000", 1000),
                    MakeTuple("5e2", 500),
                    MakeTuple(" ", default(float?)),
                    MakeTuple(null, default(float?)),
                    MakeTuple(string.Empty, default(float?))
                };
            }
        }

        public override void AssertAreEqual(float? expected, float? actual)
        {
            if (expected == default(float?))
            {
                Assert.AreEqual(expected, actual);
            }
            else
            {
                Assert.AreEqual(expected.Value, actual.Value, float.Epsilon);
            }
        }

        protected override string[] FailTestData
        {
            get { return new[] { "a" }; }
        }
    }

    [TestClass]
    public class NullableSingleConverterWithFormatProviderTest : NullableSingleConverterTest
    {
        protected override ITypeConverter<Single?> Converter
        {
            get { return new NullableSingleConverter(CultureInfo.InvariantCulture); }
        }
    }

    [TestClass]
    public class NullableSingleConverterWithFormatProviderAndNumberStyleTest : NullableSingleConverterTest
    {
        protected override ITypeConverter<Single?> Converter
        {
            get { return new NullableSingleConverter(CultureInfo.InvariantCulture, NumberStyles.Float | NumberStyles.AllowThousands); }
        }
    }

}

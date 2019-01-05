// Copyright (c) Philipp Wagner and Joel Mueller. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using NUnit.Framework;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.Test.TypeConverter
{
    [TestFixture]
    public class NullableSingleConverterTest : BaseConverterTest<Single?>
    {
        protected override ITypeConverter<Single?> Converter
        {
            get { return new NullableSingleConverter(); }
        }

        protected override (string, Single?)[] SuccessTestData
        {
            get
            {
                return new[] {
                    (Single.MinValue.ToString("R"), Single.MinValue),
                    (Single.MaxValue.ToString("R"), Single.MaxValue),
                    ("0", 0),
                    ("-1000", -1000),
                    ("1000", 1000),
                    ("5e2", 500),
                    (" ", default(Single?)),
                    (null, default(Single?)),
                    (string.Empty, default(Single?))
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
                Assert.AreEqual(expected.Value, actual, float.Epsilon);
            }
        }

        protected override string[] FailTestData
        {
            get { return new[] { "a", Double.MinValue.ToString(), Double.MaxValue.ToString() }; }
        }
    }

    [TestFixture]
    public class NullableSingleConverterWithFormatProviderTest : NullableSingleConverterTest
    {
        protected override ITypeConverter<Single?> Converter
        {
            get { return new NullableSingleConverter(CultureInfo.InvariantCulture); }
        }
    }

    [TestFixture]
    public class NullableSingleConverterWithFormatProviderAndNumberStyleTest : NullableSingleConverterTest
    {
        protected override ITypeConverter<Single?> Converter
        {
            get { return new NullableSingleConverter(CultureInfo.InvariantCulture, NumberStyles.Float | NumberStyles.AllowThousands); }
        }
    }

}

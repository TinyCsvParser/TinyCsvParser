// Copyright (c) Philipp Wagner and Joel Mueller. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using NUnit.Framework;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.Test.TypeConverter
{
    [TestFixture]
    public class UInt32ConverterTest : BaseConverterTest<UInt32>
    {
        protected override ITypeConverter<UInt32> Converter
        {
            get { return new UInt32Converter(); }
        }

        protected override (string, UInt32)[] SuccessTestData
        {
            get
            {
                return new (string, UInt32)[] {
                    (UInt32.MinValue.ToString(), UInt32.MinValue),
                    (UInt32.MaxValue.ToString(), UInt32.MaxValue),
                    ("0", 0),
                    ("1000", 1000)
                };
            }
        }

        protected override string[] FailTestData
        {
            get { return new[] { "a", "-1000", string.Empty, "  ", null, Int32.MinValue.ToString() }; }
        }
    }
}

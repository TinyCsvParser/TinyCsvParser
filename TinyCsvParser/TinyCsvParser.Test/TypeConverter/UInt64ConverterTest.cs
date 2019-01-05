// Copyright (c) Philipp Wagner and Joel Mueller. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using NUnit.Framework;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.Test.TypeConverter
{
    [TestFixture]
    public class UInt64ConverterTest : BaseConverterTest<UInt64>
    {
        protected override ITypeConverter<UInt64> Converter
        {
            get { return new UInt64Converter(); }
        }

        protected override (string, UInt64)[] SuccessTestData
        {
            get
            {
                return new (string, UInt64)[] {
                    (UInt64.MinValue.ToString(), UInt64.MinValue),
                    (UInt64.MaxValue.ToString(), UInt64.MaxValue),
                    ("0", 0),
                    ("1000", 1000)
                };
            }
        }

        protected override string[] FailTestData
        {
            get { return new[] { "a", "-1000", string.Empty, "  ", null, Int16.MinValue.ToString(), Int64.MinValue.ToString() }; }
        }
    }
}

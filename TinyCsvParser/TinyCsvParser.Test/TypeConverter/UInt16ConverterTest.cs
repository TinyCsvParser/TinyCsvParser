// Copyright (c) Philipp Wagner and Joel Mueller. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using NUnit.Framework;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.Test.TypeConverter
{
    [TestFixture]
    public class UInt16ConverterTest : BaseConverterTest<ushort>
    {
        protected override ITypeConverter<UInt16> Converter
        {
            get { return new UInt16Converter(); }
        }

        protected override (string, UInt16)[] SuccessTestData
        {
            get
            {
                return new (string, UInt16)[] {
                    (UInt16.MinValue.ToString(), UInt16.MinValue),
                    (UInt16.MaxValue.ToString(), UInt16.MaxValue),
                    ("0", 0),
                    ("1000", 1000)
                };
            }
        }

        protected override string[] FailTestData
        {
            get { return new[] { "a", "-1000", string.Empty, "  ", null, Int16.MinValue.ToString() }; }
        }
    }
}

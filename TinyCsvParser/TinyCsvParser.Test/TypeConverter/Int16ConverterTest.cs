// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.Test.TypeConverter
{
    [TestFixture]
    public class Int16ConverterTest : BaseConverterTest<Int16>
    {
        protected override ITypeConverter<Int16> Converter
        {
            get { return new Int16Converter(); }
        }

        protected override (string, Int16)[] SuccessTestData
        {
            get
            {
                return new (string, Int16)[] {
                    (Int16.MinValue.ToString(), Int16.MinValue),
                    (Int16.MaxValue.ToString(), Int16.MaxValue),
                    ("0", 0),
                    ("-1000", -1000),
                    ("1000", 1000)
                };
            }
        }

        protected override string[] FailTestData
        {
            get { return new[] { "a", string.Empty, "  ", Int32.MinValue.ToString(), Int32.MaxValue.ToString(), null }; }
        }
    }
}

// Copyright (c) Philipp Wagner and Joel Mueller. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using NUnit.Framework;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.Test.TypeConverter
{
    [TestFixture]
    public class Int32ConverterTest : BaseConverterTest<Int32>
    {
        protected override ITypeConverter<Int32> Converter
        {
            get { return new Int32Converter(); }
        }

        protected override (string, int)[] SuccessTestData
        {
            get
            {
                return new[] {
                    (Int32.MinValue.ToString(), Int32.MinValue),
                    (Int32.MaxValue.ToString(), Int32.MaxValue),
                    ("0", 0),
                    ("-1000", -1000),
                    ("1000", 1000)
                };
            }
        }

        protected override string[] FailTestData
        {
            get { return new[] { "a", string.Empty, "  ", null }; }
        }
    }
}

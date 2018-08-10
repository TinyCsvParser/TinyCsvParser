﻿// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.Test.TypeConverter
{
    [TestFixture]
    public class Int64ConverterTest : BaseConverterTest<Int64>
    {
        protected override ITypeConverter<long> Converter
        {
            get { return new Int64Converter(); }
        }

        protected override (string, long)[] SuccessTestData
        {
            get
            {
                return new[] {
                    (Int64.MinValue.ToString(), Int64.MinValue),
                    (Int64.MaxValue.ToString(), Int64.MaxValue),
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

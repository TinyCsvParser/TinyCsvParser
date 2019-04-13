// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
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

        protected override Tuple<string, UInt32>[] SuccessTestData
        {
            get
            {
                return new[] {
                    MakeTuple(UInt32.MinValue.ToString(), UInt32.MinValue),
                    MakeTuple(UInt32.MaxValue.ToString(), UInt32.MaxValue),
                    MakeTuple("0", 0),
                    MakeTuple("1000", 1000)
                };
            }
        }

        protected override string[] FailTestData
        {
            get { return new[] { "a", "-1000", string.Empty, "  ", null, Int32.MinValue.ToString() }; }
        }
    }
}

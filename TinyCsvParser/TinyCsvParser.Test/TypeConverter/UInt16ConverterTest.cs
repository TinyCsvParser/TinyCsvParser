// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.Test.TypeConverter
{
    [TestFixture]
    public class UInt16ConverterTest : BaseConverterTest<UInt16>
    {
        protected override ITypeConverter<UInt16> Converter
        {
            get { return new UInt16Converter(); }
        }

        protected override Tuple<string, UInt16>[] SuccessTestData
        {
            get
            {
                return new[] {
                    MakeTuple(UInt16.MinValue.ToString(), UInt16.MinValue),
                    MakeTuple(UInt16.MaxValue.ToString(), UInt16.MaxValue),
                    MakeTuple("0", 0),
                    MakeTuple("1000", 1000)
                };
            }
        }

        protected override string[] FailTestData
        {
            get { return new[] { "a", "-1000", string.Empty, "  ", null, Int16.MinValue.ToString() }; }
        }
    }
}

// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.Test.TypeConverter
{
    [TestClass]
    public class Int16ConverterTest : BaseConverterTest<Int16>
    {
        protected override ITypeConverter<Int16> Converter
        {
            get { return new Int16Converter(); }
        }

        protected override Tuple<string, Int16>[] SuccessTestData
        {
            get
            {
                return new[] {
                    MakeTuple(Int16.MinValue.ToString(), Int16.MinValue),
                    MakeTuple(Int16.MaxValue.ToString(), Int16.MaxValue),
                    MakeTuple("0", 0),
                    MakeTuple("-1000", -1000),
                    MakeTuple("1000", 1000)
                };
            }
        }

        protected override string[] FailTestData
        {
            get { return new[] { "a", string.Empty, "  ", Int32.MinValue.ToString(), Int32.MaxValue.ToString(), null }; }
        }
    }
}

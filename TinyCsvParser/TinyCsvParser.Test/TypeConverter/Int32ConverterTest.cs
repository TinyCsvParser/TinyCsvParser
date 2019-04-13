// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.Test.TypeConverter
{
    [TestFixture]
    public class Int32ConverterTest : BaseConverterTest<Int32>
    {
        protected override ITypeConverter<int> Converter
        {
            get { return new Int32Converter(); }
        }

        protected override Tuple<string, int>[] SuccessTestData
        {
            get
            {
                return new[] {
                    MakeTuple(Int32.MinValue.ToString(), Int32.MinValue),
                    MakeTuple(Int32.MaxValue.ToString(), Int32.MaxValue),
                    MakeTuple("0", 0),
                    MakeTuple("-1000", -1000),
                    MakeTuple("1000", 1000)
                };
            }
        }

        protected override string[] FailTestData
        {
            get { return new[] { "a", string.Empty, "  ", null }; }
        }
    }
}

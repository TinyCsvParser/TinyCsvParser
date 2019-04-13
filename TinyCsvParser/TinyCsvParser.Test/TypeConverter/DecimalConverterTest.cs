// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.Test.TypeConverter
{
    [TestClass]
    public class DecimalConverterTest : BaseConverterTest<Decimal>
    {
        protected override ITypeConverter<Decimal> Converter
        {
            get { return new DecimalConverter(); }
        }

        protected override Tuple<string, Decimal>[] SuccessTestData
        {
            get
            {
                return new[] {
                    MakeTuple(Decimal.MinValue.ToString(), Decimal.MinValue),
                    MakeTuple(Decimal.MaxValue.ToString(), Decimal.MaxValue),
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

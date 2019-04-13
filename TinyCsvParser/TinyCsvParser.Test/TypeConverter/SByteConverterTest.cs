// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.Test.TypeConverter
{
    [TestClass]
    public class SByteConverterTest : BaseConverterTest<SByte>
    {
        protected override ITypeConverter<SByte> Converter
        {
            get { return new SByteConverter(); }
        }

        protected override Tuple<string, SByte>[] SuccessTestData
        {
            get
            {
                return new[] {
                    MakeTuple(SByte.MinValue.ToString(), SByte.MinValue),
                    MakeTuple(SByte.MaxValue.ToString(), SByte.MaxValue),
                    MakeTuple("0", 0),
                    MakeTuple("-128", -128),
                    MakeTuple("127", 127)
                };
            }
        }

        protected override string[] FailTestData
        {
            get { return new[] { "a", string.Empty, "  ", null, "-129", "128" }; }
        }
    }
}

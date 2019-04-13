// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.Test.TypeConverter
{
    [TestClass]
    public class GuidConverterTest : BaseConverterTest<Guid>
    {
        protected override ITypeConverter<Guid> Converter
        {
            get { return new GuidConverter(); }
        }

        protected override Tuple<string, Guid>[] SuccessTestData
        {
            get
            {
                return new[] {
                    MakeTuple("02001000-0010-0000-0000-003200000000", Guid.Parse("02001000-0010-0000-0000-003200000000")),
                };
            }
        }

        protected override string[] FailTestData
        {
            get { return new[] { "a", string.Empty, "  ", null }; }
        }
    }

    [TestClass]
    public class GuidConverterWithFormatTest : GuidConverterTest
    {
        protected override ITypeConverter<Guid> Converter
        {
            get { return new GuidConverter("D"); }
        }
    }
}

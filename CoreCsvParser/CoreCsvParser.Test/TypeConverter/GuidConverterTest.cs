// Copyright (c) Philipp Wagner and Joel Mueller. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using CoreCsvParser.TypeConverter;

namespace CoreCsvParser.Test.TypeConverter
{
    [TestFixture]
    public class GuidConverterTest : BaseConverterTest<Guid>
    {
        protected override ITypeConverter<Guid> Converter
        {
            get { return new GuidConverter(); }
        }

        protected override (string, Guid)[] SuccessTestData
        {
            get
            {
                return new[] {
                    ("02001000-0010-0000-0000-003200000000", Guid.Parse("02001000-0010-0000-0000-003200000000")),
                };
            }
        }

        protected override string[] FailTestData
        {
            get { return new[] { "a", string.Empty, "  ", null }; }
        }
    }

    [TestFixture]
    public class GuidConverterWithFormatTest : GuidConverterTest
    {
        protected override ITypeConverter<Guid> Converter
        {
            get { return new GuidConverter("D"); }
        }
    }
}

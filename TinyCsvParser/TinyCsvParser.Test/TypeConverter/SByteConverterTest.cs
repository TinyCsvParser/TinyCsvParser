// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.Test.TypeConverter
{
    [TestFixture]
    public class SByteConverterTest : BaseConverterTest<SByte>
    {
        protected override ITypeConverter<SByte> Converter
        {
            get { return new SByteConverter(); }
        }

        protected override (string, SByte)[] SuccessTestData
        {
            get
            {
                return new (string, SByte)[] {
                    (SByte.MinValue.ToString(), SByte.MinValue),
                    (SByte.MaxValue.ToString(), SByte.MaxValue),
                    ("0", 0),
                    ("-128", -128),
                    ("127", 127)
                };
            }
        }

        protected override string[] FailTestData
        {
            get { return new[] { "a", string.Empty, "  ", null, "-129", "128" }; }
        }
    }
}

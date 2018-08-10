// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.Test.TypeConverter
{
    [TestFixture]
    public class BoolConverterTest : BaseConverterTest<bool>
    {
        protected override ITypeConverter<bool> Converter
        {
            get { return new BoolConverter(); }
        }

        protected override (string, bool)[] SuccessTestData
        {
            get
            {
                return new[] {
                    ("true", true),
                    ("false", false),
                    ("True", true),
                    ("False", false),
                };
            }
        }

        protected override string[] FailTestData
        {
            get { return new[] { "a", string.Empty, "  ", null }; }
        }
    }

    [TestFixture]
    public class BoolConverterNonDefaultTest : BaseConverterTest<bool>
    {
        protected override ITypeConverter<bool> Converter
        {
            get { return new BoolConverter("ThisIsTrue", "ThisIsFalse", StringComparison.Ordinal); }
        }

        protected override (string, bool)[] SuccessTestData
        {
            get
            {
                return new[] {
                    ("ThisIsTrue", true),
                    ("ThisIsFalse", false),
                };
            }
        }

        protected override string[] FailTestData
        {
            get { return new[] { "a", string.Empty, "  ", null, "thisistrue", "thisisfalse" }; }
        }
    }
}

// Copyright (c) Philipp Wagner and Joel Mueller. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using NUnit.Framework;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.Test.TypeConverter
{
    [TestFixture]
    public class NullableBoolConverterTest : BaseConverterTest<bool?>
    {
        protected override ITypeConverter<bool?> Converter
        {
            get { return new NullableBoolConverter(); }
        }

        protected override (string, bool?)[] SuccessTestData
        {
            get
            {
                return new[] {
                    ("true", true),
                    ("false", false),
                    (null, default(bool?)),
                    (string.Empty, default(bool?)),
                };
            }
        }

        protected override string[] FailTestData
        {
            get { return new[] { "a" }; }
        }
    }

    [TestFixture]
    public class NullableBoolConverterWithFormatConstructorTest : NullableBoolConverterTest
    {
        protected override ITypeConverter<bool?> Converter
        {
            get { return new NullableBoolConverter("true", "false", StringComparison.OrdinalIgnoreCase); }
        }
        
    }
}

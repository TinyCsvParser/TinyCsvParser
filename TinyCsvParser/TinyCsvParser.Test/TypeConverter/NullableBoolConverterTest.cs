// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.Test.TypeConverter
{
    [TestClass]
    public class NullableBoolConverterTest : BaseConverterTest<bool?>
    {
        protected override ITypeConverter<bool?> Converter
        {
            get { return new NullableBoolConverter(); }
        }

        protected override Tuple<string, bool?>[] SuccessTestData
        {
            get
            {
                return new[] {
                    MakeTuple("true", true),
                    MakeTuple("false", false),
                    MakeTuple(null, default(bool?)),
                    MakeTuple(string.Empty, default(bool?)),
                };
            }
        }

        protected override string[] FailTestData
        {
            get { return new[] { "a" }; }
        }
    }

    [TestClass]
    public class NullableBoolConverterWithFormatConstructorTest : NullableBoolConverterTest
    {
        protected override ITypeConverter<bool?> Converter
        {
            get { return new NullableBoolConverter("true", "false", StringComparison.OrdinalIgnoreCase); }
        }
        
    }
}

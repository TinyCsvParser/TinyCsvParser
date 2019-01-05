// Copyright (c) Philipp Wagner and Joel Mueller. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using NUnit.Framework;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.Test.TypeConverter
{
    [TestFixture]
    public class StringConverterTest : BaseConverterTest<string>
    {
        protected override ITypeConverter<String> Converter
        {
            get { return new StringConverter(); }
        }

        protected override (string, String)[] SuccessTestData
        {
            get
            {
                return new[] {
                    (string.Empty, string.Empty),
                    (" ", " "),
                    ("Abc", "Abc"),
                    (null, string.Empty) // spans can't be null, and null is evil
                };
            }
        }

        protected override string[] FailTestData
        {
            get { return new String[] {  }; } // Should never fail, because values are passed through.
        }
    }
}

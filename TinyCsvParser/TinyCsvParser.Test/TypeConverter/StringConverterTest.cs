// Copyright (c) Philipp Wagner and Joel Mueller. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using CoreCsvParser.TypeConverter;

namespace CoreCsvParser.Test.TypeConverter
{
    [TestFixture]
    public class StringConverterTest : BaseConverterTest<String>
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

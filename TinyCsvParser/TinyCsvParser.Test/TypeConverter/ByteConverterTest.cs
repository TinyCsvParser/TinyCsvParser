// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.Test.TypeConverter
{
    [TestFixture]
    public class ByteConverterTest : BaseConverterTest<Byte>
    {
        protected override ITypeConverter<Byte> Converter
        {
            get { return new ByteConverter(); }
        }

        protected override (string, Byte)[] SuccessTestData
        {
            get
            {
                return new (string, Byte)[] {
                    (Byte.MinValue.ToString(), Byte.MinValue),
                    (Byte.MaxValue.ToString(), Byte.MaxValue),
                    ("0", 0),
                    ("255", 255)
                };
            }
        }

        protected override string[] FailTestData
        {
            get { return new[] { "a", string.Empty, "  ", null, "-1", "256" }; }
        }
    }
}

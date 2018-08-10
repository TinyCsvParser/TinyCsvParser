// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.Test.TypeConverter
{
    [TestFixture]
    public class DateTimeConverterTest : BaseConverterTest<DateTime>
    {
        protected override ITypeConverter<DateTime> Converter
        {
            get { return new DateTimeConverter(); }
        }

        protected override (string, DateTime)[] SuccessTestData
        {
            get
            {
                return new[] {
                    ("2014/01/01", new DateTime(2014, 1, 1)),
                    ("9999/12/31", new DateTime(9999, 12, 31)),
                };
            }
        }

        protected override string[] FailTestData
        {
            get { return new[] { "a", string.Empty, "  ", null, "10000/01/01", "1753/01/32", "0/0/0" }; }
        }
    }
}

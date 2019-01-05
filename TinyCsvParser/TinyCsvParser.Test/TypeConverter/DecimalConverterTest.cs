// Copyright (c) Philipp Wagner and Joel Mueller. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using CoreCsvParser.TypeConverter;

namespace CoreCsvParser.Test.TypeConverter
{
    [TestFixture]
    public class DecimalConverterTest : BaseConverterTest<Decimal>
    {
        protected override ITypeConverter<Decimal> Converter
        {
            get { return new DecimalConverter(); }
        }

        protected override (string, Decimal)[] SuccessTestData
        {
            get
            {
                return new[] {
                    (Decimal.MinValue.ToString(), Decimal.MinValue),
                    (Decimal.MaxValue.ToString(), Decimal.MaxValue),
                    ("0", 0),
                    ("-1000", -1000),
                    ("1000", 1000)
                };
            }
        }

        protected override string[] FailTestData
        {
            get { return new[] { "a", string.Empty, "  ", null }; }
        }
    }
}

// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.Test.TypeConverter
{
    [TestFixture]
    public class SingleConverterTest : BaseConverterTest<Single>
    {
        protected override ITypeConverter<Single> Converter
        {
            get { return new SingleConverter(); }
        }

        protected override Tuple<string, Single>[] SuccessTestData
        {
            get
            {
                return new[] {
                    MakeTuple(Single.MinValue.ToString("R"), Single.MinValue),
                    MakeTuple(Single.MaxValue.ToString("R"), Single.MaxValue),
                    MakeTuple("0", 0),
                    MakeTuple("-1000", -1000),
                    MakeTuple("1000", 1000),
                    MakeTuple("5e2", 500),
                };
            }
        }

        public override void AssertAreEqual(float expected, float actual)
        {
            Assert.AreEqual(expected, actual, float.Epsilon);
        }

        protected override string[] FailTestData
        {
            get { return new[] { "a", string.Empty, "  ", null, Double.MinValue.ToString(), Double.MaxValue.ToString() }; }
        }
    }
}

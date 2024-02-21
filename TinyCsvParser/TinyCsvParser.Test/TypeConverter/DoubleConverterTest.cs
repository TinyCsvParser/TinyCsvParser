// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.Test.TypeConverter
{
    [TestFixture]
    public class DoubleConverterTest : BaseConverterTest<Double>
    {
        protected override ITypeConverter<Double> Converter
        {
            get { return new DoubleConverter(); }
        }

        protected override Tuple<string, Double>[] SuccessTestData
        {
            get
            {
                return new[] {
                    MakeTuple(double.MinValue.ToString("R"), double.MinValue),
                    MakeTuple(double.MaxValue.ToString("R"), double.MaxValue),
                    MakeTuple("0", 0),
                    MakeTuple("-1000", -1000),
                    MakeTuple("1000", 1000),
                    MakeTuple("5e2", 500),
                };
            }
        }

        public override void AssertAreEqual(Double expected, Double actual)
        {
            Assert.AreEqual(expected, actual, 1e-5);
        }

        protected override string[] FailTestData
        {
            get { return new[] { "a", string.Empty, "  ", null }; }
        }
    }
}

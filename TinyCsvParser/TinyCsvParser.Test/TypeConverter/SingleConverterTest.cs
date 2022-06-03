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
                    MakeTuple(float.MinValue.ToString("R"), float.NegativeInfinity),
                    MakeTuple(float.MaxValue.ToString("R"), float.PositiveInfinity),
                    MakeTuple("0", 0),
                    MakeTuple("-1000", -1000),
                    MakeTuple("1000", 1000),
                    MakeTuple("5e2", 500),
                };
            }
        }

        public override void AssertAreEqual(float expected, float actual)
        {
            Assert.That(actual, Is.EqualTo(expected).Within(float.Epsilon));
        }

        protected override string[] FailTestData
        {
            get { return new[] { "a", string.Empty, "  ", null }; }
        }
    }
}

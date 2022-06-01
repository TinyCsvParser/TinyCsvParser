// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.Test.TypeConverter
{
    public enum TestNullableEnum
    {
        A = 1
    }

    [TestFixture]
    public class NullableEnumConverterCaseSensitiveTest : BaseConverterTest<TestNullableEnum?>
    {

        protected override ITypeConverter<TestNullableEnum?> Converter
        {
            get { return new NullableEnumConverter<TestNullableEnum>(false); }
        }

        protected override Tuple<string, TestNullableEnum?>[] SuccessTestData
        {
            get
            {
                return new[] {
                    MakeTuple("A", TestNullableEnum.A),
                    MakeTuple(null, null),
                    MakeTuple(string.Empty, null),
                    MakeTuple(" ", null),
                };
            }
        }

        protected override string[] FailTestData
        {
            get { return new[] { "B", "a" }; }
        }
    }

    public class NullableEnumConverterCaseInsensitiveTest : BaseConverterTest<TestNullableEnum?>
    {

        protected override ITypeConverter<TestNullableEnum?> Converter
        {
            get { return new NullableEnumConverter<TestNullableEnum>(true); }
        }

        protected override Tuple<string, TestNullableEnum?>[] SuccessTestData
        {
            get
            {
                return new[] {
                    MakeTuple("A", TestNullableEnum.A),
                    MakeTuple("a", TestNullableEnum.A),
                    MakeTuple(null, null),
                    MakeTuple(string.Empty, null),
                    MakeTuple(" ", null),
                };
            }
        }

        protected override string[] FailTestData
        {
            get { return new[] { "B" }; }
        }
    }

    [TestFixture]
    public class NullableEnumConverterGeneralTest
    {
        private struct NoEnum 
        {
        }

        [Test]
        public void CouldNotInstantiateNonEnumTest()
        {
            Assert.Throws<ArgumentException>(() => new EnumConverter<NoEnum>());
        }
    }
}

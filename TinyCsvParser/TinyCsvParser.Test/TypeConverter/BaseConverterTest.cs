// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.Test.TypeConverter
{
    [TestFixture]
    public abstract class BaseConverterTest<TTargetType>
    {
        protected abstract ITypeConverter<TTargetType> Converter { get; }

        protected abstract Tuple<string, TTargetType>[] SuccessTestData { get; }

        protected abstract string[] FailTestData { get; }

        [Test]
        public void Success()
        {
            foreach (var item in SuccessTestData)
            {
                TTargetType result;

                var canParse = Converter.TryConvert(item.Item1, out result);

                Assert.IsTrue(canParse);

                AssertAreEqual(item.Item2, result);
            }
        }

        public virtual void AssertAreEqual(TTargetType expected, TTargetType actual)
        {
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Fail()
        {
            foreach (var item in FailTestData)
            {
                TTargetType result;

                var canParse = Converter.TryConvert(item, out result);

                Assert.False(canParse);
            }
        }

        public Tuple<string, TTargetType> MakeTuple(string item1, TTargetType item2)
        {
            return new Tuple<string, TTargetType>(item1, item2);
        }
    }
}

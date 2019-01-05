// Copyright (c) Philipp Wagner and Joel Mueller. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using CoreCsvParser.TypeConverter;

namespace CoreCsvParser.Test.TypeConverter
{
    [TestFixture]
    public abstract class BaseConverterTest<TTargetType>
    {
        protected abstract ITypeConverter<TTargetType> Converter { get; }

        protected abstract (string, TTargetType)[] SuccessTestData { get; }

        protected abstract string[] FailTestData { get; }

        [Test]
        public void Success()
        {
            foreach (var item in SuccessTestData)
            {
                var canParse = Converter.TryConvert(item.Item1, out TTargetType result);

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
                var canParse = Converter.TryConvert(item, out TTargetType result);

                Assert.False(canParse);
            }
        }
    }
}

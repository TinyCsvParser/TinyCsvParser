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
                Assert.AreEqual(item.Item2, result);
            }
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

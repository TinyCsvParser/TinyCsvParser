using NUnit.Framework;
using System;
using TinyCsvParser.Extensions;

namespace TinyCsvParser.Test.Extensions
{
    [TestFixture]
    public class SpanSplitTests
    {
        [Test]
        public void Basic_Split_Array_Test()
        {
            var input = "1,2,3,4,5";
            var expected = input.Split(',');
            var results = input.AsSpan().Split(',').ToArray();

            Assert.AreEqual(expected.Length, results.Length);

            for (var i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i], results[i]);
            }
        }

        [Test]
        public void Basic_Split_Test()
        {
            var input = "1,2,3,4,5";
            var expected = input.Split(',');
            var results = input.AsSpan().Split(',');
            var i = 0;

            foreach (var part in results)
            {
                Assert.IsTrue(part.Equals(expected[i++], StringComparison.Ordinal));
            }
            Assert.AreEqual(expected.Length, i);
        }

        [Test]
        public void Basic_SplitAny_Test()
        {
            var input = "1,2;3,4,;5,6|7|8";
            var expected = input.Split(',', ';', '|');
            var results = input.AsSpan().Split(',', ';', '|');
            var i = 0;

            foreach (var part in results)
            {
                Assert.IsTrue(part.Equals(expected[i++], StringComparison.Ordinal));
            }
            Assert.AreEqual(expected.Length, i);
        }

        [Test]
        public void Split_Remove_Empty_Test()
        {
            var input = ",1,2,,3,,,4,5,";
            var expected = input.Split(',', StringSplitOptions.RemoveEmptyEntries);
            var results = input.AsSpan().Split(',', StringSplitOptions.RemoveEmptyEntries);
            var i = 0;

            foreach (var part in results)
            {
                Console.WriteLine($">{part.ToString()}<");
                Assert.IsTrue(part.Equals(expected[i++], StringComparison.Ordinal));
            }
            Assert.AreEqual(expected.Length, i);
        }

        [Test]
        public void SplitAny_Remove_Empty_Test()
        {
            var input = ";;1,,2;,;|,3;;;4,;5,6|,|7|8||";
            var expected = input.Split(new[] { ',', ';', '|' }, StringSplitOptions.RemoveEmptyEntries);
            var results = input.AsSpan().Split(new[] { ',', ';', '|' }, StringSplitOptions.RemoveEmptyEntries);
            var i = 0;

            foreach (var part in results)
            {
                Console.WriteLine($">{part.ToString()}<");
                Assert.IsTrue(part.Equals(expected[i++], StringComparison.Ordinal));
            }
            Assert.AreEqual(expected.Length, i);
        }
    }
}

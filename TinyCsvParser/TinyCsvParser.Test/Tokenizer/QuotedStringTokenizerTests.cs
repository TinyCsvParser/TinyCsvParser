// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using TinyCsvParser.Tokenizer;

namespace TinyCsvParser.Test.Tokenizer
{
    [TestFixture]
    public class QuotedStringTokenizerTests
    {
        [Test]
        public void QuotedString_CommaDelimiter_Test()
        {
            var tokenizer = new QuotedStringTokenizer(',');
            
            var input = "1,\"2,3\",4";
            using (var tokens = tokenizer.Tokenize(input))
            {
                var result = tokens.Memory.Span;
                Assert.AreEqual(3, result.Length);

                Assert.AreEqual("1", result[0].Memory.ToString());
                Assert.AreEqual("2,3", result[1].Memory.ToString());
                Assert.AreEqual("4", result[2].Memory.ToString());
            }
        }

        [Test]
        public void QuotedString_SemicolonDelimiter_Test()
        {
            var tokenizer = new QuotedStringTokenizer(';');

            var input = "1;\"2;3\";4";
            using (var tokens = tokenizer.Tokenize(input))
            {
                var result = tokens.Memory.Span;
                Assert.AreEqual(3, result.Length);

                Assert.AreEqual("1", result[0].Memory.ToString());
                Assert.AreEqual("2;3", result[1].Memory.ToString());
                Assert.AreEqual("4", result[2].Memory.ToString());
            }
        }

        [Test]
        public void QuotedString_PipeSymbol_Test()
        {
            var tokenizer = new QuotedStringTokenizer('|');

            var input = "1|\"2|3\"|4";

            using (var tokens = tokenizer.Tokenize(input))
            {
                var result = tokens.Memory.Span;

                Assert.AreEqual(3, result.Length);

                Assert.AreEqual("1", result[0].Memory.ToString());
                Assert.AreEqual("2|3", result[1].Memory.ToString());
                Assert.AreEqual("4", result[2].Memory.ToString());
            }
        }

        [Test]
        public void QuotedString_ToString_Test()
        {
            var tokenizer = new QuotedStringTokenizer(';');

            Assert.DoesNotThrow(() => tokenizer.ToString());
        }
     }
}

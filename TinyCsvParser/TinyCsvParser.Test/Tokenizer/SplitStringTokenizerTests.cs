// Copyright (c) Philipp Wagner and Joel Mueller. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using TinyCsvParser.Tokenizer;

namespace TinyCsvParser.Test.Tokenizer
{
    [TestFixture]
    public class SplitStringTokenizerTests
    {
        [Test]
        public void SplitLine_WithTrim_Test()
        {
            var tokenizer = new StringSplitTokenizer(',', true);
            
            var input = " 1,2,3 ";

            var result = tokenizer.Tokenize(input).ToArray();
            Assert.AreEqual("1", result[0]);
            Assert.AreEqual("2", result[1]);
            Assert.AreEqual("3", result[2]);
            Assert.AreEqual(3, result.Length);
        }

        [Test]
        public void SplitLine_WithOutTrim_Test()
        {
            var tokenizer = new StringSplitTokenizer(',', false);
            
            var input = " 1,2,3 ";

            var result = tokenizer.Tokenize(input).ToArray();
            Assert.AreEqual(" 1", result[0]);
            Assert.AreEqual("2", result[1]);
            Assert.AreEqual("3 ", result[2]);
            Assert.AreEqual(3, result.Length);
        }

        [Test]
        public void SplitLine_WithMultCharSeparator_Test()
        {
            var tokenizer = new StringSplitTokenizer("||");

            var input = "1||2|3||4";

            var result = tokenizer.Tokenize(input).ToArray();
            Assert.AreEqual("1", result[0]);
            Assert.AreEqual("2|3", result[1]);
            Assert.AreEqual("4", result[2]);
            Assert.AreEqual(3, result.Length);
        }
     }
}

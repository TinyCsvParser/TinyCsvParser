// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using TinyCsvParser.Tokenizer;

namespace TinyCsvParser.Test.Tokenizer
{
    [TestClass]
    public class QuotedStringTokenizerTests
    {
        [TestMethod]
        public void QuotedString_CommaDelimiter_Test()
        {
            var tokenizer = new QuotedStringTokenizer(',');
            
            var input = "1,\"2,3\",4";
            var result = tokenizer.Tokenize(input);


            Assert.AreEqual(3, result.Length);

            Assert.AreEqual("1", result[0]);
            Assert.AreEqual("2,3", result[1]);
            Assert.AreEqual("4", result[2]);
        }

        [TestMethod]
        public void QuotedString_SemicolonDelimiter_Test()
        {
            var tokenizer = new QuotedStringTokenizer(';');

            var input = "1;\"2;3\";4";
            var result = tokenizer.Tokenize(input);

            Assert.AreEqual(3, result.Length);

            Assert.AreEqual("1", result[0]);
            Assert.AreEqual("2;3", result[1]);
            Assert.AreEqual("4", result[2]);
        }

        [TestMethod]
        public void QuotedString_PipeSymbol_Test()
        {
            var tokenizer = new QuotedStringTokenizer('|');

            var input = "1|\"2|3\"|4";

            var result = tokenizer.Tokenize(input);


            Assert.AreEqual(3, result.Length);

            Assert.AreEqual("1", result[0]);
            Assert.AreEqual("2|3", result[1]);
            Assert.AreEqual("4", result[2]);
        }

        [TestMethod]
        public void QuotedString_ToString_Test()
        {
            var tokenizer = new QuotedStringTokenizer(';');

            try
            {
                tokenizer.ToString();
            } catch
            {
                Assert.IsTrue(false);
            }
        }
     }
}

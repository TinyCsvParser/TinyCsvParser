// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using TinyCsvParser.Tokenizer;

namespace TinyCsvParser.Test.Tokenizer
{
    [TestClass]
    public class SplitStringTokenizerTests
    {
        [TestMethod]
        public void SplitLine_WithTrim_Test()
        {
            var tokenizer = new StringSplitTokenizer(new char[] { ',' }, true);
            
            var input = " 1,2,3 ";
            var result = tokenizer.Tokenize(input);


            Assert.AreEqual("1", result[0]);
            Assert.AreEqual("2", result[1]);
            Assert.AreEqual("3", result[2]);
        }

        [TestMethod]
        public void SplitLine_WithOutTrim_Test()
        {
            var tokenizer = new StringSplitTokenizer(new char[] { ',' }, false);
            
            var input = " 1,2,3 ";
            var result = tokenizer.Tokenize(input);


            Assert.AreEqual(" 1", result[0]);
            Assert.AreEqual("2", result[1]);
            Assert.AreEqual("3 ", result[2]);
        }
     }
}

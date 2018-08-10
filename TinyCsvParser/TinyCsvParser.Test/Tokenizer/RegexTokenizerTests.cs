// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System.Text.RegularExpressions;
using TinyCsvParser.Tokenizer.RegularExpressions;

namespace TinyCsvParser.Test.Tokenizer
{
    [TestFixture]
    public class RegexTokenizerTests
    {
        [Test]
        public void Regex_Split()
        {
            var tokenizer = new RegularExpressionTokenizer(new Regex("[,|]"));

            var input = "abc,def|ghi,|j";

            var result = tokenizer.Tokenize(input).ToArray();

            Assert.AreEqual(5, result.Length);
            Assert.AreEqual("abc", result[0]);
            Assert.AreEqual("def", result[1]);
            Assert.AreEqual("ghi", result[2]);
            Assert.AreEqual("", result[3]);
            Assert.AreEqual("j", result[4]);
        }
    }
}

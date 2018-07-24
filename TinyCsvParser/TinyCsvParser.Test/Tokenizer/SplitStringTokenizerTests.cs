// Copyright (c) Philipp Wagner. All rights reserved.
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
            var tokenizer = new StringSplitTokenizer(new char[] { ',' }, true);
            
            var input = " 1,2,3 ";
            using (var tokens = tokenizer.Tokenize(input))
            {
                var result = tokens.Memory.Span;
                Assert.AreEqual("1", result[0].Memory.ToString());
                Assert.AreEqual("2", result[1].Memory.ToString());
                Assert.AreEqual("3", result[2].Memory.ToString());
            }

            int i = 1;
            foreach (var token in ((ITokenizer2)tokenizer).Tokenize(input))
            {
                Assert.AreEqual(i.ToString(), token.ToString());
                i++;
            }
        }

        [Test]
        public void SplitLine_WithOutTrim_Test()
        {
            var tokenizer = new StringSplitTokenizer(new char[] { ',' }, false);
            
            var input = " 1,2,3 ";
            using (var tokens = tokenizer.Tokenize(input))
            {
                var result = tokens.Memory.Span;

                Assert.AreEqual(" 1", result[0].Memory.ToString());
                Assert.AreEqual("2", result[1].Memory.ToString());
                Assert.AreEqual("3 ", result[2].Memory.ToString());
            }

            int i = 1;
            foreach (var token in ((ITokenizer2)tokenizer).Tokenize(input))
            {
                switch (i)
                {
                    case 1:
                        Assert.AreEqual(" 1", token.ToString());
                        break;
                    case 2:
                        Assert.AreEqual("2", token.ToString());
                        break;
                    case 3:
                        Assert.AreEqual("3 ", token.ToString());
                        break;
                    default:
                        Assert.Fail($"Unexpected token '{token.ToString()}' at position {i}.");
                        break;
                }
                i++;
            }
        }
     }
}

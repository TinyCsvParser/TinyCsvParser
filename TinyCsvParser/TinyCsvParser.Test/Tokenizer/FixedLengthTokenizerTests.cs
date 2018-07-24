// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System.Text;
using TinyCsvParser.Tokenizer;

namespace TinyCsvParser.Test.Tokenizer
{
    [TestFixture]
    public class FixedLengthTokenizerTests
    {
        [Test]
        public void Tokenize_Line_Test()
        {
            var columns = new[] {
                new FixedLengthTokenizer.ColumnDefinition(0, 10),
                new FixedLengthTokenizer.ColumnDefinition(10, 20),
            };

            var tokenizer = new FixedLengthTokenizer(columns);
            
            var input = new StringBuilder()
                .AppendLine("Philipp   Wagner    ")
                .ToString();

            using (var tokens = tokenizer.Tokenize(input))
            {
                var result = tokens.Memory.Span;
                Assert.AreEqual("Philipp   ", result[0].Memory.ToString());
                Assert.AreEqual("Wagner    ", result[1].Memory.ToString());
                Assert.AreEqual(2, result.Length);
            }

            int i = 0;
            foreach (var token in ((ITokenizer2)tokenizer).Tokenize(input))
            {
                switch (i)
                {
                    case 0:
                        Assert.AreEqual("Philipp   ", token.ToString());
                        break;
                    case 1:
                        Assert.AreEqual("Wagner    ", token.ToString());
                        break;
                    default:
                        Assert.Fail($"Unexpected token '{token.ToString()}' at index {i}.");
                        break;
                }
                i++;
            }
        }
     }
}

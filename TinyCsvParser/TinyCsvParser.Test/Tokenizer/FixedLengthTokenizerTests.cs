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

            var result = tokenizer.Tokenize(input);

            Assert.AreEqual("Philipp   ", result[0].ToString());
            Assert.AreEqual("Wagner    ", result[1].ToString());
        }
     }
}

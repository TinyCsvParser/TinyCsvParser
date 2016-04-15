// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using System.Text;
using TinyCsvParser.Tokenizer;
using TinyCsvParser.Tokenizer.Decorators;
using ColumnDefinition = TinyCsvParser.Tokenizer.FixedLengthTokenizer.ColumnDefinition;

namespace TinyCsvParser.Test.Tokenizer
{
    [TestFixture]
    public class PostProcessingTokenizerDecoratorTests
    {
        [Test]
        public void Tokenize_With_Trim_Test()
        {
            ColumnDefinition[] columns = new[] {
                new FixedLengthTokenizer.ColumnDefinition(0, 10),
                new FixedLengthTokenizer.ColumnDefinition(10, 20),
            };

            Func<string, string>  processor = (s) => s.Trim();

            ITokenizer decoratedTokenizer = new FixedLengthTokenizer(columns);

            ITokenizer tokenizer = new PostProcessingTokenizerDecorator(decoratedTokenizer, processor);
            
            string input = new StringBuilder()
                .AppendLine(" Philipp   Wagner   ")
                .ToString();

            string[] result = tokenizer.Tokenize(input);

            Assert.AreEqual("Philipp", result[0]);
            Assert.AreEqual("Wagner", result[1]);
        }
     }
}

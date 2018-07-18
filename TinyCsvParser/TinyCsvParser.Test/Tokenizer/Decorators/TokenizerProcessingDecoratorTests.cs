// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using System.Text;
using TinyCsvParser.Tokenizer;
using TinyCsvParser.Tokenizer.Decorators;

// Used for defining fixed-width columns:
using ColumnDefinition = TinyCsvParser.Tokenizer.FixedLengthTokenizer.ColumnDefinition;

// Used for Processing CSV Values before and after Tokenization:
using Preprocessor = TinyCsvParser.Tokenizer.Decorators.TokenizerProcessingDecorator.Preprocessor;
using Postprocessor = TinyCsvParser.Tokenizer.Decorators.TokenizerProcessingDecorator.Postprocessor;

namespace TinyCsvParser.Test.Tokenizer
{
    [TestFixture]
    public class TokenizerProcessingDecoratorTests
    {
        [Test]
        public void Tokenize_With_Preprocess_ToUppercase_Postprocess_Trim_Test()
        {
            // Defines the Columns of a File:
            ColumnDefinition[] columns = new[] {
                new FixedLengthTokenizer.ColumnDefinition(0, 10),
                new FixedLengthTokenizer.ColumnDefinition(10, 20),
            };

            // The Postprocessing Function on the Column value:
            var preprocessor = new Preprocessor(s =>
            {
                var output = new Memory<char>(new char[s.Length]);
                s.ToUpperInvariant(output.Span);
                return output.Span;
            });
            Postprocessor postprocessor = new Postprocessor(s => s.Trim());

            // The Original Tokenizer, which tokenizes the line:
            ITokenizer decoratedTokenizer = new FixedLengthTokenizer(columns);

            ITokenizer tokenizer = new TokenizerProcessingDecorator(decoratedTokenizer, preprocessor, postprocessor);

            string input = new StringBuilder()
                .AppendLine(" Philipp   Wagner   ")
                .ToString();

            using (var tokens = tokenizer.Tokenize(input))
            {
                var result = tokens.Memory.Span;
                Assert.AreEqual("PHILIPP", result[0].Memory.ToString());
                Assert.AreEqual("WAGNER", result[1].Memory.ToString());
            }
        }
    }
}
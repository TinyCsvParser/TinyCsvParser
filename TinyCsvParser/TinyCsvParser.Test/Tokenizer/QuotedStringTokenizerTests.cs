// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System.Linq;
using TinyCsvParser.Tokenizer;
using TinyCsvParser.Tokenizer.RFC4180;

namespace TinyCsvParser.Test.Tokenizer
{
    [TestFixture]
    public class QuotedStringTokenizerTests
    {
        [Test]
        public void QuotedString_CommaDelimiter_Test()
        {
            var tokenizer = new RFC4180Tokenizer(new Options('"', '\\', ',', false));
            
            var input = new[] { "1,\"2,3\",4" };

            var result = tokenizer
                .Tokenize(input)
                .ToList();


            Assert.AreEqual(3, result[0].Length);

            Assert.AreEqual("1", result[0][0]);
            Assert.AreEqual("2,3", result[0][1]);
            Assert.AreEqual("4", result[0][2]);
        }

        [Test]
        public void QuotedString_SemicolonDelimiter_Test()
        {
            var tokenizer = new RFC4180Tokenizer(new Options('"', '\\', ';', false));

            var input = new[] { "1;\"2;3\";4" };

            var result = tokenizer
                .Tokenize(input)
                .ToList();

            Assert.AreEqual(3, result[0].Length);

            Assert.AreEqual("1", result[0][0]);
            Assert.AreEqual("2;3", result[0][1]);
            Assert.AreEqual("4", result[0][2]);
        }

        [Test]
        public void QuotedString_PipeSymbol_Test()
        {
            var tokenizer = new RFC4180Tokenizer(new Options('"', '\\', '|', false));

            var input = new[] { "1|\"2|3\"|4" };

            var result = tokenizer
                .Tokenize(input)
                .ToList();


            Assert.AreEqual(3, result[0].Length);

            Assert.AreEqual("1", result[0][0]);
            Assert.AreEqual("2|3", result[0][1]);
            Assert.AreEqual("4", result[0][2]);
        }
     }
}

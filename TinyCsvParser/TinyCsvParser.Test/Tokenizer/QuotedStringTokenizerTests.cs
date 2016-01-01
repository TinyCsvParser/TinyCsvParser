using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyCsvParser.Tokenizer;
using TinyCsvParser.Tokenizer.RegularExpressions;

namespace TinyCsvParser.Test.Tokenizer
{
    [TestFixture]
    public class QuotedStringTokenizerTests
    {
        [Test]
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

        [Test]
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

        [Test]
        public void QuotedString_ToString_Test()
        {
            var tokenizer = new QuotedStringTokenizer(';');

            Assert.DoesNotThrow(() => tokenizer.ToString());
        }
     }
}

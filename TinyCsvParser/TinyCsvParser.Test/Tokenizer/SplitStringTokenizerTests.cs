using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            var result = tokenizer.Tokenize(input);


            Assert.AreEqual("1", result[0]);
            Assert.AreEqual("2", result[1]);
            Assert.AreEqual("3", result[2]);
        }

        [Test]
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

// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using TinyCsvParser.Tokenizer.Rfc4180;

namespace TinyCsvParser.Test.Tokenizer
{
    [TestFixture]
    public class Rfc4180TokenizerTest
    {
        public void Rfc4180_QuotedString_Test()
        {
            var tokenizer = new Rfc4180Tokenizer();

            // Initialize a String with Double Quoted Data:
            var line = "\"Michael, Chester\",24,\"Also goes by \"\"Mike\"\", among friends that is\"";

            // Split the Line into its Tokens:
            var tokens = tokenizer.Tokenize(line);

            // And make sure the Quotes are retained:
            Assert.AreEqual("Michael, Chester", tokens[0]);
            Assert.AreEqual("24", tokens[1]);
            Assert.AreEqual("Also goes by \"\"Mike\"\", among friends that is", tokens[2]);
        }

    }
}

// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using System.Linq;
using System.Text;
using TinyCsvParser.Mapping;
using TinyCsvParser.Tokenizer.RFC4180;

namespace TinyCsvParser.Test.Tokenizer
{
    [TestFixture]
    public class Rfc4180TokenizerTest
    {
        private class SampleEntity
        {
            public string Name { get; set; }

            public int? Age { get; set; }

            public string Description { get; set; }
        }

        private class SampleEntityMapping : CsvMapping<SampleEntity>
        {
            public SampleEntityMapping()
            {
                MapProperty(0, x => x.Name);
                MapProperty(1, x => x.Age);
                MapProperty(2, x => x.Description);
            }
        }

        [Test]
        public void RFC4180_CsvParser_Integration_Test()
        {
            // Use a " as Quote Character, a \\ as Escape Character and a , as Delimiter.
            var options = new Options('"', '\\', ',', false);

            // Initialize the Rfc4180 Tokenizer:
            var tokenizer = new RFC4180Tokenizer(options);

            // Now Build the Parser:
            CsvParserOptions csvParserOptions = new CsvParserOptions(true, tokenizer);
            SampleEntityMapping csvMapper = new SampleEntityMapping();
            CsvParser<SampleEntity> csvParser = new CsvParser<SampleEntity>(csvParserOptions, csvMapper);


            var stringBuilder = new StringBuilder()
                .AppendLine("Name, Age, Description")
                .AppendLine("\"Michael, Chester\",24,\"Also goes by \\\"Mike\\\", among friends that is\"")
                .AppendLine("\"Robert, Willliamson\", ,\"All-around nice guy who always says hi\"");
            
            // Define the NewLine Character to split at:
            CsvReaderOptions csvReaderOptions = new CsvReaderOptions(new[] { Environment.NewLine });

            var result = csvParser
                .ReadFromString(csvReaderOptions, stringBuilder.ToString())
                .ToList();

            Assert.AreEqual(2, result.Count);

            Assert.AreEqual(true, result.All(x => x.IsValid));

            Assert.AreEqual("Michael, Chester", result[0].Result.Name);
            Assert.AreEqual(24, result[0].Result.Age);
            Assert.AreEqual("Also goes by \"Mike\", among friends that is", result[0].Result.Description);

            Assert.AreEqual("Robert, Willliamson", result[1].Result.Name);
            Assert.AreEqual(false, result[1].Result.Age.HasValue);
            Assert.AreEqual("All-around nice guy who always says hi", result[1].Result.Description);
        }

        [Test]
        public void Rfc4180_QuotedString_Double_Quoted_Data_Test()
        {
            // Use a " as Quote Character, a \\ as Escape Character and a , as Delimiter.
            var options = new Options('"', '\\', ',', false);

            // Initialize the Rfc4180 Tokenizer:
            var tokenizer = new RFC4180Tokenizer(options);

            // Initialize a String with Double Quoted Data:
            var line = "\"Michael, Chester\",24,\"Also goes by \\\"Mike\\\", among friends that is\"";

            // Split the Line into its Tokens:
            var tokens = tokenizer.Tokenize(new[] { line }).ToList();

            // And make sure the Quotes are retained:
            Assert.IsNotNull(tokens);

            Assert.AreEqual(3, tokens[0].Length);

            Assert.AreEqual("Michael, Chester", tokens[0][0]);
            Assert.AreEqual("24", tokens[0][1]);
            Assert.AreEqual("Also goes by \"Mike\", among friends that is", tokens[0][2]);
        }

        [Test]
        public void Rfc4180_Issue3_Empty_Column_Test()
        {
            // Use a " as Quote Character, a \\ as Escape Character and a , as Delimiter.
            var options = new Options('"', '\\', ',', false);

            // Initialize the Rfc4180 Tokenizer:
            var tokenizer = new RFC4180Tokenizer(options);

            // Initialize a String with Double Quoted Data:
            var line = "\"Robert, Willliamson\", ,\"All-around nice guy who always says hi\"";
            // Split the Line into its Tokens:
            var tokens = tokenizer.Tokenize(new[] { line }).ToList();

            // And make sure the Quotes are retained:
            Assert.IsNotNull(tokens);

            Assert.AreEqual(3, tokens[0].Length);

            Assert.AreEqual("Robert, Willliamson", tokens[0][0]);
            Assert.AreEqual(" ", tokens[0][1]);
            Assert.AreEqual("All-around nice guy who always says hi", tokens[0][2]);
        }

        [Test]
        public void Rfc4180_Issue3_Empty_First_Column_Test()
        {
            // Use a " as Quote Character, a \\ as Escape Character and a , as Delimiter.
            var options = new Options('"', '\\', ',', false);

            // Initialize the Rfc4180 Tokenizer:
            var tokenizer = new RFC4180Tokenizer(options);

            // Initialize a String with Double Quoted Data:
            var line = " , 24 ,\"Great Guy\"";
            // Split the Line into its Tokens:
            var tokens = tokenizer.Tokenize(new[] { line }).ToList();

            // And make sure the Quotes are retained:
            Assert.IsNotNull(tokens);

            Assert.AreEqual(3, tokens[0].Length);

            Assert.AreEqual(" ", tokens[0][0]);
            Assert.AreEqual(" 24 ", tokens[0][1]);
            Assert.AreEqual("Great Guy", tokens[0][2]);
        }

        [Test]
        public void Rfc4180_Issue3_Empty_Last_Columns_Test()
        {
            // Use a " as Quote Character, a \\ as Escape Character and a , as Delimiter.
            var options = new Options('"', '\\', ',', false);

            // Initialize the Rfc4180 Tokenizer:
            var tokenizer = new RFC4180Tokenizer(options);

            // Initialize a String with Double Quoted Data:
            var line = "\"Robert, Willliamson\",27,";
            // Split the Line into its Tokens:
            var tokens = tokenizer.Tokenize(new[] { line }).ToList();

            // And make sure the Quotes are retained:
            Assert.IsNotNull(tokens);

            Assert.AreEqual(3, tokens[0].Length);

            Assert.AreEqual("Robert, Willliamson", tokens[0][0]);
            Assert.AreEqual("27", tokens[0][1]);
            Assert.AreEqual("", tokens[0][2]);
        }

        [Test]
        public void All_Empty_Last_Columns_Test()
        {
            // Use a " as Quote Character, a \\ as Escape Character and a , as Delimiter.
            var options = new Options('"', '\\', ',', false);

            // Initialize the Rfc4180 Tokenizer:
            var tokenizer = new RFC4180Tokenizer(options);

            // Initialize a String with Double Quoted Data:
            var line = ",,";
            // Split the Line into its Tokens:
            var tokens = tokenizer.Tokenize(new[] { line }).ToList();

            // And make sure the Quotes are retained:
            Assert.IsNotNull(tokens);

            Assert.AreEqual(3, tokens[0].Length);

            Assert.AreEqual("", tokens[0][0]);
            Assert.AreEqual("", tokens[0][1]);
            Assert.AreEqual("", tokens[0][2]);
        }

        [Test]
        public void All_Empty_Last_Column_Not_Empty_Test()
        {
            // Use a " as Quote Character, a \\ as Escape Character and a , as Delimiter.
            var options = new Options('"', '\\', ',', false);

            // Initialize the Rfc4180 Tokenizer:
            var tokenizer = new RFC4180Tokenizer(options);

            // Initialize a String with Double Quoted Data:
            var line = ",,a";
            // Split the Line into its Tokens:
            var tokens = tokenizer.Tokenize(new[] { line }).ToList();

            // And make sure the Quotes are retained:
            Assert.IsNotNull(tokens);

            Assert.AreEqual(3, tokens[0].Length);

            Assert.AreEqual("", tokens[0][0]);
            Assert.AreEqual("", tokens[0][1]);
            Assert.AreEqual("a", tokens[0][2]);
        }
    }
}

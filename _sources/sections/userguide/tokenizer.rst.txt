.. _userguide_tokenizer:

Tokenizer
=========

Motivation
~~~~~~~~~~

There is no possible standard how CSV files are described. There is no schema, so every file you get 
may look completely different. This rules out one single strategy to tokenize a given line in your CSV 
data. 

Imagine a situation, where a column delimiter is also present in the column data like this:

::

    FirstNameLastName;BirthDate
    "Philipp,Wagner",1986/05/12
    ""Max,Mustermann",2014/01/01


A simple :csharp:`string.Split` with a comma as column delimiter will lead to wrong data, so the line 
needs to be split differently. And this is exactely where a :code:`Tokenizer` fits in.

So a :code:`Tokenizer` is used to split a given line of your CSV data into the column data.

Available Tokenizers
~~~~~~~~~~~~~~~~~~~~

StringSplitTokenizer
""""""""""""""""""""

The :csharp:`StringSplitTokenizer` splits a line at a given column delimiter.

::

    Philipp,Wagner,1986/05/12
    
Is tokenized into the following values:

* :code:`Philipp`
* :code:`Wagner`
* :code:`1986/05/12`

RFC4180Tokenizer
""""""""""""""""

The `RFC4180`_ proposes a specification for the CSV format, which is commonly accepted. You can use 
the :csharp:`RFC4180Tokenizer` to parse a CSV file in a `RFC4180`_-compliant format.

Example
^^^^^^^

Imagine a RFC4180-compliant CSV file with Person Names should be parsed. Each Person has a :code:`Name`, 
:code:`Age` and :code:`Description`. The :code:`Name` and :code:`Description` may contain the column 
delimiter and also double quotes.  

::

    Name, Age, Description
	"Michael, Chester", 24, "Also goes by ""Mike"", among friends that is"
	"Robert, Willliamson", , "All-around nice guy who always says hi"

The following example shows how to use the :code:`RFC4180Tokenizer` for the given example data.
	
.. code-block:: csharp

	// Copyright (c) Philipp Wagner. All rights reserved.
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
				var options = new Options('"', '\\', ',');

				// Initialize the Rfc4180 Tokenizer:
				var tokenizer = new RFC4180Tokenizer(options);

				// Now Build the Parser:
				CsvParserOptions csvParserOptions = new CsvParserOptions(true, tokenizer);
				SampleEntityMapping csvMapper = new SampleEntityMapping();
				CsvParser<SampleEntity> csvParser = new CsvParser<SampleEntity>(csvParserOptions, csvMapper);


				var stringBuilder = new StringBuilder()
					.AppendLine("Name, Age, Description")
					.AppendLine("\"Michael, Chester\",24,\"Also goes by \"\"Mike\"\", among friends that is\"")
					.AppendLine("\"Robert, Willliamson\", , \"All-around nice guy who always says hi\"");
				
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
		}
	}
	
RegularExpressionTokenizer
""""""""""""""""""""""""""

The :csharp:`RegularExpressionTokenizer` is an **abstract base class**, that uses a regular expression 
to match a given line. So if you need to match a line with a regular expression, you have to implement 
this base class.

The :csharp:`QuotedStringTokenizer` is a good example to start with.

QuotedStringTokenizer
"""""""""""""""""""""
 
The :csharp:`QuotedStringTokenizer` is an implementation of a :csharp:`RegularExpressionTokenizer`. It uses 
a (rather complicated) regular expression to leave data in a double quotes (:code:`""`) untouched, so a line 
like:

::

    "Philipp,Wagner",1986/05/12
    
Is tokenized into the following values:

* :code:`Philipp,Wagner`
* :code:`1986/05/12`

Example 
^^^^^^^

Imagine a CSV file contains a list of persons with the following data:

::

    FirstNameLastName;BirthDate
    "Philipp,Wagner",1986/05/12
    ""Max,Mustermann",2014/01/01

The first name and the last name are using a comma, which is the same character as the column delimiter. 
So the file can't be tokenized by only splitting at the column delimiter with the default 
:code:`StringSplitTokenizer`. 

This is where the :code:`QuotedStringTokenizer` is needed! 

The :code:`Tokenizer` is set in the :code:`CsvParserOptions`.

.. code-block:: csharp

    using NUnit.Framework;
    using System;
    using System.Linq;
    using System.Text;
    using TinyCsvParser.Mapping;
    using TinyCsvParser.Tokenizer.RegularExpressions;
    
    namespace TinyCsvParser.Test.Tokenizer
    {
        [TestFixture]
        public class TokenizerExampleTest
        {
            private class Person
            {
                public string FirstNameWithLastName { get; set; }
                public DateTime BirthDate { get; set; }
            }
    
            private class CsvPersonMapping : CsvMapping<Person>
            {
                public CsvPersonMapping()
                {
                    MapProperty(0, x => x.FirstNameWithLastName);
                    MapProperty(1, x => x.BirthDate);
                }
            }
    
            [Test]
            public void QuotedStringTokenizerExampleTest()
            {
                CsvParserOptions csvParserOptions = new CsvParserOptions(true, new QuotedStringTokenizer(','));
                CsvReaderOptions csvReaderOptions = new CsvReaderOptions(new[] { Environment.NewLine });
                CsvPersonMapping csvMapper = new CsvPersonMapping();
                CsvParser<Person> csvParser = new CsvParser<Person>(csvParserOptions, csvMapper);
    
                var stringBuilder = new StringBuilder()
                    .AppendLine("FirstNameLastName;BirthDate")
                    .AppendLine("\"Philipp,Wagner\",1986/05/12")
                    .AppendLine("\"Max,Mustermann\",2014/01/01");
    
                var result = csvParser
                    .ReadFromString(csvReaderOptions, stringBuilder.ToString())
                    .ToList();
    
                // Make sure we got 2 results:
                Assert.AreEqual(2, result.Count);
    
                // And all of them have been parsed correctly:
                Assert.IsTrue(result.All(x => x.IsValid));
    
                // Now check the values:
                Assert.AreEqual("Philipp,Wagner", result[0].Result.FirstNameWithLastName);
    
                Assert.AreEqual(1986, result[0].Result.BirthDate.Year);
                Assert.AreEqual(5, result[0].Result.BirthDate.Month);
                Assert.AreEqual(12, result[0].Result.BirthDate.Day);
    
                Assert.AreEqual("Max,Mustermann", result[1].Result.FirstNameWithLastName);
    
                Assert.AreEqual(2014, result[1].Result.BirthDate.Year);
                Assert.AreEqual(1, result[1].Result.BirthDate.Month);
                Assert.AreEqual(1, result[1].Result.BirthDate.Day);
            }
        }
    }


FixedLengthTokenizer
""""""""""""""""""""

Sometimes you need to parse a CSV file, that is defined by fixed width columns. The :code:`FixedLengthTokenizer` addresses this problem and makes 
it possible to define columns by their start and end position in a given file. The :code:`FixedLengthTokenizer` takes a list of 
:code:`FixedLengthTokenizer.ColumnDefinition`, which define the columns of the file.

Example 
^^^^^^^

In the following example the textual input is split into two columns. The first column is between index 0 and 10, and the second column is between the 
index 10 and 20. You can see, that these values build the list of :code:`ColumnDefinition`, which are passed into the :code:`FixedLengthTokenizer`.

.. code-block:: csharp

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

				Assert.AreEqual("Philipp   ", result[0]);
				Assert.AreEqual("Wagner    ", result[1]);
			}
		 }
	}

.. _RFC4180: http://tools.ietf.org/html/rfc4180
.. _TinyCsvParser: https://github.com/bytefish/TinyCsvParser
.. _NUnit: http://www.nunit.org
.. MIT License: https://opensource.org/licenses/MIT
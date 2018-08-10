.. _userguide_postprocessing:

Preprocessing and Postprocessing
================================

There may be times, when you simply cannot parse the CSV data, because the data contains problematic 
characters, unwanted characters or even malformed data. This is when you need to pre- or postprocess 
the data before and after tokenization.

Tokenization described the process of splitting your data into column. You may have read about the 
:code:`StringSplitTokenizer`, the :code:`QuotedStringTokenizer` or the :code:`FixedLengthTokenizer`. 
The different tokenizers are simply different strategies for turning the data into their parts.

Still sometimes, there is no alternative to preprocess or postprocess the data. This is where the 
:code:`TokenizerProcessingDecorator` is going to help you.

Motivation
~~~~~~~~~~

A Decorator, sometimes also called a Wrapper, is a simple pattern from the seminal Gang of Four Book. It allows you to add 
behavior to a class, by wrapping the original implementation of the class and implementing the same interface. This allows 
us to perform actions before and after invoking the wrapped object.

TokenizerProcessingDecorator
~~~~~~~~~~~~~~~~~~~~~~~~~~~~

The decorator used for preprocessing and postprocessing the data is the :code:`TokenizerProcessingDecorator`.

The processors responsible for processing the data are:

* :code:`TokenizerProcessingDecorator.Preprocessor`
* :code:`TokenizerProcessingDecorator.Postprocessor`

The available constructors are:

* :code:`TokenizerProcessingDecorator(ITokenizer tokenizer, Preprocessor preprocessor)`
* :code:`TokenizerProcessingDecorator(ITokenizer tokenizer, Postprocessor postprocessor)`
* :code:`TokenizerProcessingDecorator(ITokenizer tokenizer, Preprocessor preprocessor, Postprocessor postprocessor)`

Example
~~~~~~~

Imagine a CSV file with fixed columns is parsed:

::

	" Philipp   Wagner   "

The first column starts at index 0 and ends at index 10, the second column starts at index 10 and ends at index 20. 

A :code:`FixedLengthTokenizer` can be used to parse the data. The parsing leads to the following values:

::

	"Philipp   "
	"Wagner    "

	
You can see, that we are left with unwanted whitespace characters in the tokenized data. This is probably not a huge problem, because it could be removed 
when processing the result object, but I do not think such raw data preprocessing and postprocessing steps should pollute your processing pipeline.

Imagine we want the Tokenization result to be upper case and the whitespaces trimmed. Here is how to do it with a :code:`TokenizerProcessingDecorator`.

.. code-block:: csharp

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
				Preprocessor preprocessor = new Preprocessor(s => s.ToUpperInvariant());
				Postprocessor postprocessor = new Postprocessor(s => s.Trim());
	
				// The Original Tokenizer, which tokenizes the line:
				ITokenizer decoratedTokenizer = new FixedLengthTokenizer(columns);
	
				ITokenizer tokenizer = new TokenizerProcessingDecorator(decoratedTokenizer, preprocessor, postprocessor);
	
				string input = new StringBuilder()
					.AppendLine(" Philipp   Wagner   ")
					.ToString();
	
				string[] result = tokenizer.Tokenize(input);
	
				Assert.AreEqual("PHILIPP", result[0]);
				Assert.AreEqual("WAGNER", result[1]);
			}
		}
	}


.. _TinyCsvParser: https://github.com/bytefish/TinyCsvParser
.. _NUnit: http://www.nunit.org
.. MIT License: https://opensource.org/licenses/MIT
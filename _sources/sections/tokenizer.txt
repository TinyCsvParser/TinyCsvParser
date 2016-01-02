.. _tokenizer:

Tokenizer
=========

Motivation
~~~~~~~~~~

There is no possible standard how CSV files are described. There is no schema, so every file you get 
may look completely different. This rules out a single strategy to tokenize a given line in your CSV 
data. 

Imagine a situation, where a column delimiter is also present in the column data like this:

::

    FirstNameLastName;BirthDate
    "Philipp,Wagner",1986/05/12
    ""Max,Mustermann",2014/01/01


A simple :csharp:`string.Split` with a comma as column delimiter will lead to wrong data, so the line needs 
to be split differently. And this is exactely where a :code:`Tokenizer` fits in.

Available Tokenizers
~~~~~~~~~~~~~~~~~~~~

The library comes with the following Tokenizers.

StringSplitTokenizer
""""""""""""""""""""

The :csharp:`StringSplitTokenizer` splits a line at a given column delimiter.

RegularExpressionTokenizer
""""""""""""""""""""""""""

The :csharp:`RegularExpressionTokenizer` is an abstract base class, that uses a Regular Expression 
to match a given line. If you need to match a line with a Regular Expression, you have to implement 
this base class.

The :csharp:`QuotedStringTokenizer` is a good example to implement a :csharp:`RegularExpressionTokenizer`.

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
~~~~~~~

Now imagine a CSV file contains a list of persons with the following data:

```
FirstNameLastName;BirthDate
"Philipp,Wagner",1986/05/12
""Max,Mustermann",2014/01/01
```

The first name and the last name are using the same character as the column delimiter. So the file can't 
be tokenized by only splitting at the column delimiter. This is where the ``QuotedStringTokenizer`` is 
needed! 

The Tokenizer is set in the ``CsvParserOptions``.

```csharp
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
```

.. _TinyCsvParser: https://github.com/bytefish/TinyCsvParser
.. _NUnit: http://www.nunit.org
.. MIT License: https://opensource.org/licenses/MIT
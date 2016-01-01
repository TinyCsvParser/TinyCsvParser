.. _quickstart:

Quickstart
==========


Imagine we have list of persons in a CSV file :code:`persons.csv` with their first name, last name and birthdate.

::

    Philipp;Wagner;1986/05/12
    Max;Musterman;2014/01/02


The corresponding domain model in our system might look like this.

.. code-block:: csharp

    private class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
    }

When using `TinyCsvParser`_ you have to define the mapping between the columns in the CSV data 
and the property in you domain model.

.. code-block:: csharp

    private class CsvPersonMapping : CsvMapping<Person>
    {
        public CsvPersonMapping()
            : base()
        {
            MapProperty(0, x => x.FirstName);
            MapProperty(1, x => x.LastName);
            MapProperty(2, x => x.BirthDate);
        }
    }

And then we can use the mapping to parse the CSV data with a ``CsvParser``.

.. code-block:: csharp

    namespace TinyCsvParser.Test
    {
        [TestFixture]
        public class TinyCsvParserTest
        {
            [Test]
            public void TinyCsvTest()
            {
                CsvParserOptions csvParserOptions = new CsvParserOptions(true, new[] { ';' });
                CsvPersonMapping csvMapper = new CsvPersonMapping();
                CsvParser<Person> csvParser = new CsvParser<Person>(csvParserOptions, csvMapper);
    
                var result = csvParser
                    .ReadFromFile(@"persons.csv", Encoding.ASCII)
                    .ToList();
    
                Assert.AreEqual(2, result.Count);
    
                Assert.IsTrue(result.All(x => x.IsValid));
                
                Assert.AreEqual("Philipp", result[0].Result.FirstName);
                Assert.AreEqual("Wagner", result[0].Result.LastName);
    
                Assert.AreEqual(1986, result[0].Result.BirthDate.Year);
                Assert.AreEqual(5, result[0].Result.BirthDate.Month);
                Assert.AreEqual(12, result[0].Result.BirthDate.Day);
    
                Assert.AreEqual("Max", result[1].Result.FirstName);
                Assert.AreEqual("Mustermann", result[1].Result.LastName);
    
                Assert.AreEqual(2014, result[1].Result.BirthDate.Year);
                Assert.AreEqual(1, result[1].Result.BirthDate.Month);
                Assert.AreEqual(1, result[1].Result.BirthDate.Day);
            }
        }
    }

Reading From a String
"""""""""""""""""""""

Reading from a string is possible with the :csharp:`CsvParser.ReadFromString` method. 

.. code-block:: csharp

    namespace TinyCsvParser.Test
    {
        [TestFixture]
        public class TinyCsvParserTest
        {
            [Test]
            public void TinyCsvTest()
            {
                CsvParserOptions csvParserOptions = new CsvParserOptions(true, new[] { ';' });
                CsvReaderOptions csvReaderOptions = new CsvReaderOptions(new[] { Environment.NewLine });
                CsvPersonMapping csvMapper = new CsvPersonMapping();
                CsvParser<Person> csvParser = new CsvParser<Person>(csvParserOptions, csvMapper);
    
                var stringBuilder = new StringBuilder()
                    .AppendLine("FirstName;LastName;BirthDate")
                    .AppendLine("Philipp;Wagner;1986/05/12")
                    .AppendLine("Max;Mustermann;2014/01/01");
    
                var result = csvParser
                    .ReadFromString(csvReaderOptions, stringBuilder.ToString())
                    .ToList();
    
                Assert.AreEqual(2, result.Count);
    
                Assert.IsTrue(result.All(x => x.IsValid));
    
                // Asserts ...
            }
        }
    }
    
.. _TinyCsvParser: https://github.com/bytefish/TinyCsvParser
.. _NUnit: http://www.nunit.org
.. MIT License: https://opensource.org/licenses/MIT
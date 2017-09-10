.. _quickstart:

Quickstart
==========

This section is only a quick tour of the most common use of TinyCsvParser. For more detailed 
information, please consult the full :ref:`User Guide <userguide>`.

The :ref:`Tutorials<tutorials>` are also a good place to find more advanced use-cases.

Quick Installation
~~~~~~~~~~~~~~~~~~

You can use the `NuGet <https://www.nuget.org>`_ package to install `TinyCsvParser`_. Run the following 
command in the `Package Manager Console <http://docs.nuget.org/consume/package-manager-console>`_.

::
    
    PM> Install-Package TinyCsvParser


Getting Started
~~~~~~~~~~~~~~~

Imagine we have list of persons in a CSV file :code:`persons.csv` with their first name, last name 
and birthdate. The columns are separated by :code:`;` as column delimiter, which each line will be 
split at.

::

    Philipp;Wagner;1986/05/12
    Max;Musterman;2014/01/02

The corresponding domain model in our C# code might look like this.

.. code-block:: csharp

    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
    }

When using `TinyCsvParser`_ you have to define the mapping between the columns in the CSV data and 
the property in you domain model, which is done by implementing the class :code:`CsvMapping<TEntity>`, 
where :code:`TEntity` is the class :code:`Person`.

.. code-block:: csharp

    public class CsvPersonMapping : CsvMapping<Person>
    {
        public CsvPersonMapping()
            : base()
        {
            MapProperty(0, x => x.FirstName);
            MapProperty(1, x => x.LastName);
            MapProperty(2, x => x.BirthDate);
        }
    }

The method :code:`MapProperty` is used to map between the column number in the CSV file and the property in the 
domain model. 

Then we can use the mapping to parse the CSV data with a ``CsvParser``. In the `CsvParserOptions` we are 
defining to not skip the header and use a :code:`;` as column delimiter. I have assumed, that the file is encoded 
as :code:`ASCII`.

.. code-block:: csharp

    namespace TinyCsvParser.Test
    {
        [TestFixture]
        public class TinyCsvParserTest
        {
            [Test]
            public void TinyCsvTest()
            {
                CsvParserOptions csvParserOptions = new CsvParserOptions(false, ';');
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

Reading from a string is possible with the :code:`CsvParser.ReadFromString` method.

.. code-block:: csharp

    namespace TinyCsvParser.Test
    {
        [TestFixture]
        public class TinyCsvParserTest
        {
            [Test]
            public void TinyCsvTest()
            {
                CsvParserOptions csvParserOptions = new CsvParserOptions(true, ';');
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
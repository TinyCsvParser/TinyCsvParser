.. _userguide_concepts:

Concepts
========

CsvMapping
~~~~~~~~~~

The :code:`CsvMapping` defines the mapping between the CSV column index and the properties of a .NET object. It is an abstract base class, 
that needs to be implemented and it exposes the :code:`MapProperty` method to define the mapping.

You have seen an example for a :code:`CsvMapping` in the :ref:`Quickstart<quickstart>` document.

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


CsvParserOptions
~~~~~~~~~~~~~~~~

In order to parse a CSV file, you need to define the column delimiter to use and define to skip the header 
or not. These options are passed into a :code:`CsvParser` by using the :code:`CsvParserOptions`. The most 
basic constructor for a :code:`CsvParserOption` is:

.. code-block:: csharp

    CsvParserOptions(bool skipHeader, char fieldsSeparator)
 
There are more advanced options, which can be passed with:

.. code-block:: csharp

    CsvParserOptions(bool skipHeader, string commentCharacter, ITokenizer tokenizer, int degreeOfParallelism, bool keepOrder)

The Parameters are:
  
* skipHeader
    * Signals to skip the header row (true) or not (false).
* tokenizer
    * The Tokenizer to use for splitting a line in the CSV data into the column values. The default is a :code:`StringSplitTokenizer` (see :ref:`User Guide on Tokenizer <userguide_tokenizer>` for additional information).
* degreeOfParallelism
    * The number of threads used to do the mapping and further processing. The default is :code:`Environment.ProcessorCount` (`See MSDN for additional information <https://msdn.microsoft.com/en-us/library/system.environment.processorcount(v=vs.110).aspx>`_).
* keepOrder
    * When the input is processed in parallel, the results can be unordered. The :code:`keepOrder` flag signals wether to keep the original order (:code:`true`) or return the unordered results (:code:`false`). The default is :code:`true`.

CsvReaderOptions
~~~~~~~~~~~~~~~~

When reading CSV data from a string with :code:`CsvParser.ReadFromString`, you have to define the 
NewLine character used for splitting the input into lines. This class is not neccessary when reading 
from a file with :code:`CsvParser.ReadFromFile`.

The :code:`CsvReaderOptions` constructor signature is:

.. code-block:: csharp

    public CsvReaderOptions(string[] newLine)
    
The parameter is:

* newLine
    * Defines the character used for splitting the input data into lines.
    
CsvMappingResult
~~~~~~~~~~~~~~~~

The :code:`CsvMappingResult` is the result of the parsing. The class contains either the populated object or 
an error. Why doesn't a :code:`CsvParser` return just the mapped entities? Because the input data is processed 
in parallel and the :code:`CsvParser` can't stop parsing, because a single line has an error.

That's why the parsed entity is wrapped in a :code:`CsvMappingResult`, which holds either the entity or an error, 
that may have occured during parsing a CSV line.

You can check, if a :code:`CsvMappingResult` is valid by checking the property :code:`CsvMappingResult<TEntity>.IsValid`. 
If the :code:`CsvMappingResult` is valid, then it contains the populated entity in the property. If the parsing was 
not possible due to an error, then the property :code:`CsvMappingResult<TEntity>.Error` is filled.

You have seen this in the :ref:`Quickstart<quickstart>` example already.
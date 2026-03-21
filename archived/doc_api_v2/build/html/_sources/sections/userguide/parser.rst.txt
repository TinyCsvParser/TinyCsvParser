.. _userguide_parser:

Parser
======

A :code:`CsvParser` is at the core of the library. It is used to parse the given CSV data into strongly-typed objects.

Contructing a Parser
~~~~~~~~~~~~~~~~~~~~

A :code:`CsvParser` needs the :code:`CsvParserOptions` and a :code:`CsvMapping` to be constructed. 

Mapping
"""""""

The parser has to know how to map between the textual CSV data and the strongly typed .NET object. This mapping is defined with 
a :code:`CsvMapping`, which defines the mapping between the CSV column index and the property of the .NET object. It is an 
abstract base class, that needs to be implemented by the user of the library. 

The :code:`CsvMapping` exposes the method :code:`MapProperty` to define the actual property mapping.

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

Options
"""""""

The :code:`CsvParser` doesn't know by default, if the header row of the CSV data should be skipped or how to tokenize (see :ref:`Tokenizer<userguide_tokenizer>`) a line. The options 
are set in the :code:`CsvParserOptions` and passed into to the :code:`CsvParser`. Since input data can processed in parallel, so there are also options for the degree 
of parallelism. 

In the simplest case it is sufficient to pass the flag for the header skip and the column delimiter.

You have seen an example for :code:`CsvParserOptions` in the :ref:`Quickstart<quickstart>` document.

.. code-block:: csharp

    CsvParserOptions csvParserOptions = new CsvParserOptions(false, ';');

Parsing CSV Data
~~~~~~~~~~~~~~~~

The :code:`CsvParser` exposes the methods :code:`ReadFromFile` and :code:`ReadFromString` to read the CSV data from a given file or :code:`string`.

You have seen an example for :code:`CsvParserOptions` in the :ref:`Quickstart<quickstart>` document.

.. code-block:: csharp

    var result = csvParser
        .ReadFromFile("person.csv", Encoding.UTF8)
        .ToList();

Working with the Results
~~~~~~~~~~~~~~~~~~~~~~~~

The return value of the :code:`CsvParser.ReadFromFile` and :code:`CsvParser.ReadFromString` methods is a :code:`ParallelQuery<CsvMappingResult<TEntity>>`. 

A `ParallelQuery`? A :code:`ParallelQuery` is a special :code:`IEnumerable` from the Parallel LINQ namespace, that behaves almost like a normal 
:code:`IEnumerable` (with a few exceptions). In order to evaluate the results, you can iterate through the :code:`ParallelQuery`, which is the preferred 
way of working with the results. If you are uncomfortable with enumerables, you can also turn the data into a simple list by calling the method 
:code:`ToList()` on it.

.. note:: 

    The library uses Parallel LINQ (PLINQ) to support a high degree of parallelism. Building a parallel processing pipeline 
    with PLINQ may not be intuitive, so reading the most important PLINQ concepts is suggested. There is a great documentation 
    on working with Parallel LINQ at MSDN: `Parallel LINQ (PLINQ) <https://msdn.microsoft.com/en-us/library/dd460688(v=vs.110).aspx>`_.

The :code:`CsvMappingResult` holds the parse results. You can access the result through the property :code:`CsvMappingResult<TEntity>.Result`, but the property 
is only populated, when the parsing was successful. You can check if the CSV data was parsed successfully by evaluating the property :code:`CsvMappingResult<TEntity>.IsValid`.

.. attention::

    The :code:`CsvParser` doesn't throw any exceptions during parsing, because the input data is processed in parallel and the 
    :code:`CsvParser` can't stop parsing, just because a single line has an error. So the :code:`CsvMappingResult` can also 
    contain an error, if parsing a line was not successful.

If a CSV line could not be parsed, the property :code:`CsvMappingResult<TEntity>.Error` is populated and contains the problematic column and error message.
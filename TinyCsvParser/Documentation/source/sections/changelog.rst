.. _changelog:

Changelog
=========

2.5.1
~~~~~

* Introduced Bugfixes to Ranges to allow compilation for .NET Core 3.0.

2.5.0
~~~~~

* Added Range-based mappings to map values into Arrays.

2.4.0
~~~~~

* Added an ``ICsvMapping`` interface to allow replacing the ``CsvMapping`` implementation.
* Removed ``new()`` constraints for mappings.
* Added a ``CsvStringArrayMapping`` to map data into a String array.
* Uses ``PackageLicenseExpression`` for NuGet Licensing.

2.3.0
~~~~~

* Extension method ``ReadLinesFromStream`` has been added to read data from Streams directly.

2.2.1
~~~~~

* Introduced .netstandard 2.0 support.

2.2.0
~~~~~

* Added an Option "UnmappedRow" to the Parser, so Errors have access to the original data.

2.1.1
~~~~~

* Add a Option to trim values to the ``FixedLengthTokenizer``

2.1.0
~~~~~

* Fix ``RFC4180Tokenizer`` when using Tabs as Delimiter
* Added an Option to trim values to the ``StringSplitTokenizer``

2.0.0
~~~~~

Breaking Changes
----------------

* Removed the :code:`WithCustomConverter` method from the ``CsvPropertyMapping``
* Changed the constructors of :code:`CsvParserOptions`
  
  *  The parameter :code:`fieldsSeparator` (column separator) is now a :code:`char` (was :code:`char[]`)


Features
--------

* Added Row Numbers

  * Accessible through :code:`CsvMappingResult.RowIndex`
  
* :code:`RFC4180Tokenizer` now set as default Tokenizer

.. MIT License: https://opensource.org/licenses/MIT
.. _changelog:

Changelog
=========

2.5.1
~~~~~

* Introduced Bugfixes to Ranges to allow compilation for .NET Core 3.0.

2.5.0
~~~~~

* Added a range-based :code:`MapProperty` overload, that allows mapping to arrays.

2.4.0
~~~~~

* Added an :code:`ICsvMapping` interface to allow replacing the :code:`CsvMapping` implementation.
* Removed :code:`new()` constraints for mappings.
* Added a :code:`CsvStringArrayMapping` to map data into a String array.
* Uses :code:`PackageLicenseExpression` for NuGet Licensing.

2.3.0
~~~~~

* Extension method :code:`ReadLinesFromStream` has been added to read data from Streams directly.

2.2.1
~~~~~

* Introduced .netstandard 2.0 support.

2.2.0
~~~~~

* Added an Option :code:`UnmappedRow` to the Parser, so Errors have access to the original data.

2.1.1
~~~~~

* Add a Option to trim values to the :code:`FixedLengthTokenizer`

2.1.0
~~~~~

* Fix :code:`RFC4180Tokenizer` when using Tabs as Delimiter
* Added an Option to trim values to the :code:`StringSplitTokenizer`

2.0.0
~~~~~

Breaking Changes
----------------

* Removed the :code:`WithCustomConverter` method from the :code:`CsvPropertyMapping`
* Changed the constructors of :code:`CsvParserOptions`
  
  *  The parameter :code:`fieldsSeparator` (column separator) is now a :code:`char` (was :code:`char[]`)


Features
--------

* Added Row Numbers

  * Accessible through :code:`CsvMappingResult.RowIndex`
  
* :code:`RFC4180Tokenizer` now set as default Tokenizer

.. MIT License: https://opensource.org/licenses/MIT
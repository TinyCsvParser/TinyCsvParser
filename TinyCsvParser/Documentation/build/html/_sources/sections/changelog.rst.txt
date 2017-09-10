.. _changelog:

Changelog
=========

2.0
~~~

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
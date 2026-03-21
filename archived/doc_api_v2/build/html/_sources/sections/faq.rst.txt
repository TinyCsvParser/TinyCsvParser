.. _faq:

FAQ
===

This is a list of Frequently Asked Questions (FAQ) about TinyCsvParser.

* How do I access the Line Number?
    * The Line Number is accessible through the property :code:`CsvMappingResult.RowIndex`.
    
* Why is my CSV data split into wrong columns?
    * By default the CSV data is split at the column delimiter given in the :code:`CsvParserOptions`. If the column delimiter is also present in the column data, 
      then a simple split will lead to wrong column data. 
      
      In this situation you need to use a custom :ref:`Tokenizer <userguide_tokenizer>`, please see the section on :ref:`Tokenizers<userguide_tokenizer>` for a detailed introduction.
      
* How can I parse CSV data coming in a custom format?
    * All built-in converters support format strings and other advanced formatting options. Please see the detailed guide on :ref:`Type Converter <userguide_type_converter>` 
      and have a look at the tutorial on :ref:`Parsing Custom Formats <tutorials_custom_formats>`. If your needs are highly specific, check out :ref:`Custom Mapping <tutorials_custom_mapping>`.
      
* Can I contribute to the project?
    * **Yes!** You can help out with code, documentation, bug reports, bug fixes, ... Please see the section on :ref:`Contributing`.

.. _TinyCsvParser: https://github.com/bytefish/TinyCsvParser

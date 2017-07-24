.. _overview_introduction:

Introduction to TinyCsvParser
=============================

What is TinyCsvParser?
~~~~~~~~~~~~~~~~~~~~~~

`TinyCsvParser`_ is an open source library to parse CSV data into strongly typed .NET objects. It 
is probably the fastest .NET CSV Parser around (see :ref:`Benchmark<benchmark>`) and is highly 
configurable to provide maximum flexibility. 

In summary, TinyCsvParser offers the following key features:

* Easy to use and a very clean API
* **Fast!** (:ref:`Benchmark<benchmark>`)
* Open Source
* Highly Flexible

  * Parsing almost every format is possible
  * Converting custom data types is easy
  
* Batteries-included

  * All .NET CLR types are automatically converted (see :ref:`Type Converter <userguide_type_converter>`)
  * Parsing ``enums`` is a very easy task (see :ref:`Tutorials <tutorials_parsing_enums>`)
  
* Extensively Unit Tested
* Extensively Documented

Why build TinyCsvParser?
~~~~~~~~~~~~~~~~~~~~~~~~

I have often been tearing my hair out at work, when trying to grok custom hand-rolled CSV parsing code. 

CSV parsing can lead to code monsters in real life, that will eat your precious lifetime and pose 
a threat to your application. After all, you are importing data into your system. Your data must 
be valid or you (or your team) are going to suffer. Correcting imported data manually, is an expensive 
and frustrating task.

CSV files are abused for a lot of things in reality. They can be complicated beasts and sometimes even 
contain malformed data. Often enough you don't have any control of the format. Parsing the input data 
is often enough not just a :code:`string.Split`, think of File Encodings, Date formats, Number formats...

A consistent approach for parsing CSV data in your application is important or you will get into severe 
problems maintaining an application. Enough hair was torn out by developers reading custom hand-rolled 
CSV parsing code!

.. _TinyCsvParser: https://github.com/bytefish/TinyCsvParser
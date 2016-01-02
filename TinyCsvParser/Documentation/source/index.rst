.. include:: includes/roles.rst

TinyCsvParser
=============

`TinyCsvParser`_ is a library to parse CSV data in an easy and fun way, while offering high performance 
and a very clean API. It is probably the fastest .NET CSV Parser around and it is highly extendable to 
provide maximum flexibility.

It makes your life easier and brings you some joy, when working with CSV files.

**And it's released unter terms of the** `MIT license`_ **!**

Key Features
~~~~~~~~~~~~

* Easy to use and a very clean API
* **Fast!** (:ref:`Benchmark<benchmark>`)
* Highly Flexible

  * Parsing almost every format is possible
  * Converting custom data types is easy
  
* Batteries-included

  * All .NET data types are automatically converted
  * Parsing ``enums`` is a very easy task
  
* Extensively Unit Tested
* Extensively Documented

Install
~~~~~~~

You can also use the `NuGet <https://www.nuget.org>`_ package. To install `TinyCsvParser`_, run the following 
command in the `Package Manager Console <http://docs.nuget.org/consume/package-manager-console>`_.

::
    
    PM> Install-Package TinyCsvParser

Documentation index
~~~~~~~~~~~~~~~~~~~

.. toctree::
    :maxdepth: 1
	
    sections/motivation
    sections/quickstart
    sections/tokenizer
    sections/examples
    sections/benchmark
    
Indices and tables
""""""""""""""""""

* :ref:`genindex`
* :ref:`modindex`
* :ref:`search`

.. _MIT license: https://opensource.org/licenses/MIT
.. _TinyCsvParser: https://github.com/bytefish/TinyCsvParser
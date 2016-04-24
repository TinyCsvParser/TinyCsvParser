.. _contributing:

Contributing
============

Help Out
~~~~~~~~

TinyCsvParser is an open source project and there are many ways to contribute to it.  

Some things you could help out with are:

* Documentation (both code and features)
* Bug reports
* Bug fixes
* Feature requests
* Feature implementations
* Test coverage
* Code quality
* Sample applications

Open Source
~~~~~~~~~~~

The TinyCsvParser project is hosted on GitHub at:

* `https://github.com/bytefish/TinyCsvParser <https://github.com/bytefish/TinyCsvParser>`_

Want to get your hands on the sources? Then clone the repository using the following git command:

::

    git clone https://github.com/bytefish/TinyCsvParser

Building the Documentation
~~~~~~~~~~~~~~~~~~~~~~~~~~

Once you have cloned the repository to your local machine, the following instructions will walk you through installing the tools necessary to build the documentation.

1. `Download python <https://www.python.org/downloads/>`_ version 2.7.10 or higher.

2. If you are installing on Windows, ensure both the Python install directory and the Python scripts directory have been added to your :code:`PATH` environment variable. 
   For example, if you install Python into the :code:`c:\python34 directory`, you would add :code:`c:\python34;c:\python34\scripts` to your :code:`PATH` environment variable.
   
3. Install Sphinx by opening a command prompt and running the following Python command. (Note that this operation might take a few minutes to complete.)

   ::
        
       pip install sphinx

4. By default, when you install Sphinx, it will install the ReadTheDocs custom theme automatically. If you need to update the installed version of this theme, you should run:

   ::

       pip install -U sphinx_rtd_theme
       
5. Navigate to one of the projects Documentation located at :code:`TinyCsvParser/Documentation/`.

6. Run :code:`make` (:code:`make.bat` on Windows, Makefile on Mac/Linux) from command line:

   ::

       make html

7. Once make completes, the generated docs will be in the :code:`.../Documentation/build/html` directory. Simply open the :code:`index.html` file in your browser to see the built documentation.

.. _NUnit: http://www.nunit.org
.. MIT License: https://opensource.org/licenses/MIT
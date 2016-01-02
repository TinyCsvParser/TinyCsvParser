.. _motivation:

Motivation
==========

`TinyCsvParser`_ is a library for parsing CSV data. It will help you to provide an easy and consistent 
way for reading CSV data in your application. It is highly extendable to provide maximum flexibility, 
while maintaining a very high performance (see `Benchmark<benchmark>`). I hope it makes your life easier 
and brings you some joy, when working with CSV files.

Why a CSV Parser? 

I have often been tearing my hair out at work, when trying to grok CSV parsing code. 

CSV parsing can lead to code monsters in real life, that will eat your precious lifetime and pose 
a threat to your application. After all, you are importing data into your system. Your data must 
be valid or you (or your team) are going to suffer correcting the data manually, which is expensive 
and frustrating.

And believe me. An interface for reading CSV data is going to become important in an application, 
especially when a team is short on time. It is just some CSV data, right? How complicated can it 
be? Oh, well. 

If you have ever felt the same pain as I did, you probably know, that CSV files are abused for a lot 
of things in reality. They can be complicated beasts and sometimes even contain malformed data. Often 
enough you don't have any control of the format. No blaming, even your client may have no control over 
the CSV data format. 

Oh wait, what about File Encodings? Oh, what about date formats? Oh, what about culture specific number 
formats? Oh Oh Oh...!

A consistent approach for parsing CSV data in your application is important or you will get 
into severe problems maintaining an application. Let's agree, that enough hair was torn out by 
developers reading custom hand-rolled CSV parsing code!

.. _TinyCsvParser: https://github.com/bytefish/TinyCsvParser
.. _NUnit: http://www.nunit.org
.. MIT License: https://opensource.org/licenses/MIT
.. _userguide_type_converter:

Type Converter
==============

Introduction
~~~~~~~~~~~~

A :code:`TypeConverter` is used to convert the text value in a column of your CSV data into a .NET type.

All available :code:`TypeConverter` in the library are initialized with sane default formats to parse data. These 
formats default to the same formats .NET uses to parse string data. So MSDN is a great source of information to get 
informations about formatting strings.

If you need to parse custom formats, which do not match the default format, you have to specify a custom format for 
the converter when defining the :code:`CsvMapping`, which is shown in the example on this page.

Available Type Converters
~~~~~~~~~~~~~~~~~~~~~~~~~

+---------------+-----------------------------+
| .NET CLR Type | Type Converter              |
+===============+=============================+
|Boolean        | BoolConverter               |
+---------------+-----------------------------+
|Boolean?       | NullableBooleanConverter    |
+---------------+-----------------------------+
|Byte           | ByteConverter               |
+---------------+-----------------------------+
|Byte?          | NullableByteConverter       |
+---------------+-----------------------------+
|DateTime       | DateTimeConverter           |
+---------------+-----------------------------+
|DateTime?      | NullableDateTimeConverter   |
+---------------+-----------------------------+
|Decimal        | DecimalConverter            |
+---------------+-----------------------------+
|Decimal?       | NullableDecimalConverter    |
+---------------+-----------------------------+
|Double         | DoubleConverter             |
+---------------+-----------------------------+
|Double?        | NullableDoubleConverter     |
+---------------+-----------------------------+
|Enum           | EnumConverter<TEnum>        |
+---------------+-----------------------------+
|Guid           | GuidConverter               |
+---------------+-----------------------------+
|Guid?          | NullableGuidConverter       |
+---------------+-----------------------------+
|Int16          | Int16Converter              |
+---------------+-----------------------------+
|Int16?         | NullableInt16Converter      |
+---------------+-----------------------------+
|Int32          | Int32Converter              |
+---------------+-----------------------------+
|Int32?         | NullableInt32Converter      |
+---------------+-----------------------------+
|Int64          | Int64Converter              |
+---------------+-----------------------------+
|Int64?         | NullableInt64Converter      |
+---------------+-----------------------------+
|SByte          | SByteConverter              |
+---------------+-----------------------------+
|SByte?         | NullableSByteConverter      |
+---------------+-----------------------------+
|Single         | SingleConverter             |
+---------------+-----------------------------+
|Single?        | NullableSingleConverter     |
+---------------+-----------------------------+
|String         | StringConverter             |
+---------------+-----------------------------+
|TimeSpan       | TimeSpanConverter           |
+---------------+-----------------------------+
|TimeSpan?      | NullableTimeSpanConverter   |
+---------------+-----------------------------+
|UInt16         | UInt16Converter             |
+---------------+-----------------------------+
|UInt16?        | NullableUInt16Converter     |
+---------------+-----------------------------+
|UInt32         | UInt32Converter             |
+---------------+-----------------------------+
|UInt32?        | NullableUInt32Converter     |
+---------------+-----------------------------+
|UInt64         | UInt64Converter             |
+---------------+-----------------------------+
|UInt64?        | NullableUInt64Converter     |
+---------------+-----------------------------+

Parsing Custom Formats
~~~~~~~~~~~~~~~~~~~~~~

In order to parse a value with a custom format, you have to instantiate a Type Converter with the format. All of the available Type Converters 
take custom format strings. You should look up the MSDN documentation on the formatting strings for the specific .NET type. Some converters can 
also use an `IFormatProvider <https://msdn.microsoft.com/en-us/library/system.iformatprovider(v=vs.110).aspx>`_, again MSDN is a great resource 
for information.

The custom converter needs to be defined when defining the :code:`CsvMapping`, by passing it into the :code:`CsvMapping.MapProperty(...)`. You will understand everything with an example.

DateTimeConverter Example
"""""""""""""""""""""""""

Imagine a CSV file contains data with a weird format for dates, like this :code:`2004###01###25`. These values cannot be parsed with the default 
date format, but in TinyCsvParser a DateTimeConverter with the custom date time format can be defined.

To use the custom converter, you have to pass it to the :code:`MapProperty` method on the property mapping.

.. code-block:: csharp

    private class CsvPersonMappingWithCustomConverter : CsvMapping<Person>
    {
        public CsvPersonMappingWithCustomConverter()
        {
            MapProperty(0, x => x.FirstName);
            MapProperty(1, x => x.LastName);
            MapProperty(2, x => x.BirthDate, new DateTimeConverter("yyyy###MM###dd"));
        }
    }

.. _TinyCsvParser: https://github.com/bytefish/TinyCsvParser
.. _NUnit: http://www.nunit.org
.. MIT License: https://opensource.org/licenses/MIT
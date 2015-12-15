## TypeConverter ##

A ``TypeConverter`` is used to convert from a ``string`` value into a .NET type. The TypeConverters are 
located in the namespace ``TinyCsvParser.TypeConverter`` and include converters for all basic .NET types. 

This section describes how to parse CSV data with custom formats, parse enums and write your own ``TypeConverter``.

### Customizing a Converter ###

Sometimes the CSV data comes with values in a special format, which the default converter registrations 
cannot parse. All of the available TypeConverters have constructors to parse .

Imagine a client sends you data with a weird format for dates and writes dates like this ``2004###01###25``. 
These values cannot be parsed with the default date format, but in TinyCsvParser we can easily define a 
DateTimeConverter with the custom date time format.

To use the custom converter, you have to use the ``WithCustomConverter`` method on the property mapping.

```csharp
private class CsvPersonMappingWithCustomConverter : CsvMapping<Person>
{
    public CsvPersonMappingWithCustomConverter()
    {
        MapProperty(0, x => x.FirstName);
        MapProperty(1, x => x.LastName);
        MapProperty(2, x => x.BirthDate)
            .WithCustomConverter(new DateTimeConverter("yyyy###MM###dd"));
    }
}
```

### Parsing Enums ###



### Available Converters ###

A ``TypeConverter`` for the following data types are built-in:

* ``Bool``, ``Bool?``
* ``Byte``, ``Byte?``
* ``DateTime``, ``DateTime?`` 
* ``Decimal``, ``Decimal?`` 
* ``Double``, ``Double?``
* ``Enum``, ``Enum?``
* ``Guid``, ``Guid?``
* ``Int16``, ``Int16?``
* ``Int32``, ``Int32?``
* ``Int64``, ``Int64?``
* ``SByte``, ``SByte?``
* ``Single``, ``Single?``
* ``String``
* ``TimeSpan``, ``TimeSpan?``
* ``UInt16``, ``UInt16?``
* ``UInt32``, ``UInt32?``
* ``UInt64``, ``UInt64?``

## Converting an Enum ##


## TypeConverter ##

A ``TypeConverter`` is used to convert from a ``string`` value into a .NET type. When you are defining 
a ``CsvMapping`` and map the property with ``MapProperty(...)``, the ``TypeConverter`` for the .NET type 
is resolved and subsequently used for parsing the data.

The library already contains TypeConverters for all basic .NET types. 

The existing converters can be customized, so also custom formats of input data can be parsed.

### Available Converters ###

The following ``TypeConverter`` are built-in:

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

### Customizing a Converter (Using a Format) ###

Sometimes the CSV data comes with values in a special format, that the default instances of the converters 
cannot parse. But all existing converters support customizing the format used for parsing.

Imagine a client sends data with a weird format for dates and writes dates like this ``2004###01###25``. 
These values cannot be parsed with the default date format, but in TinyCsvParser a DateTimeConverter with 
the custom date time format can be defined.

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

#### BooleanConverter ####

Imagine you want to map between the CSV value and a boolean. The library makes the assumption, that 

Imagine your CSV data uses the text ``ThisIsTrue`` for the boolean value ``true``, and ``ThisIsFalse`` for 
the boolean value ``false``. Then you have to instantiate and use the ``BoolConverter`` like this:

```csharp
new BoolConverter("ThisIsTrue", "ThisIsFalse", StringComparison.InvariantCulture);
```

And use it for the property mapping.

### Parsing Enums ###

Sometimes it is neccessary to parse the CSV data into an enum, which can be done with the ``EnumConverter``.

Imagine we have a CSV file containing a vehicle:

```
VehicleType;Name
Car;Suzuki Swift
Bike;A Bike
```

First define the ``enum`` in code:

```csharp
private enum VehicleTypeEnum
{
	Car,
	Bike
}
```

Then define the class the results should be mapped to:

```csharp
private class Vehicle
{
	public VehicleTypeEnum VehicleType { get; set; }

	public string Name { get; set; }
}
```

And now the mapping between both has to be defined. For parsing the ``VehicleType`` a custom converter 
has to be used, which simply is a ``EnumConverter<VehicleTypeEnum``, the constructor argument signals to 
ignore the case (upper-case/lower-case) for parsing the enum.

```
private class CsvVehicleMapping : CsvMapping<Vehicle>
{
	public CsvVehicleMapping()
	{
		MapProperty(0, x => x.VehicleType, new EnumConverter<VehicleTypeEnum>(true));
		MapProperty(1, x => x.Name);
	}
}
```

And then the CSV data can be parsed as usual:

```csharp
[Test]
public void CustomEnumConverterTest()
{
	CsvParserOptions csvParserOptions = new CsvParserOptions(true, new[] { ';' });
	CsvReaderOptions csvReaderOptions = new CsvReaderOptions(new[] { Environment.NewLine });
	CsvVehicleMapping csvMapper = new CsvVehicleMapping();
	CsvParser<Vehicle> csvParser = new CsvParser<Vehicle>(csvParserOptions, csvMapper);

	var stringBuilder = new StringBuilder()
		.AppendLine("VehicleType;Name")
		.AppendLine("Car;Suzuki Swift")
		.AppendLine("Bike;A Bike");

	var result = csvParser
		.ReadFromString(csvReaderOptions, stringBuilder.ToString())
		.ToList();

	Assert.AreEqual(VehicleTypeEnum.Car, result[0].Result.VehicleType);
	Assert.AreEqual("Suzuki Swift", result[0].Result.Name);

	Assert.AreEqual(VehicleTypeEnum.Bike, result[1].Result.VehicleType);
	Assert.AreEqual("A Bike", result[1].Result.Name);
}
```

## TypeConverterProvider ##

So how does the ``CsvMapping`` resolve the correct converter for a property?

``CsvMapping`` is a base class, that uses an ``ITypeConverterProvider`` to resolve the TypeConverter for a 
properties type. The default provider is the ``TypeConverterProvider``, which contains all of the available 
converters. It is also possible to pass an ``ITypeConverter`` into the ``CsvMapping``, but as a normal user 
you shouldn't have to do this at all. 

If it's really necessary to pass a custom ``ITypeConverterProvider`` into the ``CsvMapping``, then I suggest 
to to instantiate the default ``TypeConverterProvider`` and use its ``Add<TTargetType>`` method to add a new 
converter or its ``Override<TTargetType>`` method to override an existing converter.
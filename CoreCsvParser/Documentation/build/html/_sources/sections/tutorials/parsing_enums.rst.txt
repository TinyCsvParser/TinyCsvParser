.. _tutorials_parsing_enums:

Parsing Enums
=============

Sometimes it is neccessary to parse the CSV data into an enum, which can be done with an :code:`EnumConverter`.

Example
~~~~~~~

Imagine we have a CSV file containing a vehicle, with a Name and a VehicleType. The VehicleType can only be a :code:`Car` or a :code:`Bike`.

::

    VehicleType;Name
    Car;Suzuki Swift
    Bike;A Bike

It useful to represent the VehicleType as an enumeration in our C# code. So first define the :code:`enum` in code:

.. code-block:: csharp

    private enum VehicleTypeEnum
    {
        Car,
        Bike
    }

Then define the class the results should be mapped to:

.. code-block:: csharp

    private class Vehicle
    {
        public VehicleTypeEnum VehicleType { get; set; }
    
        public string Name { get; set; }
    }

And now the mapping between the CSV File and the domain model has to be defined. For parsing the :code:`VehicleType`
a custom converter has to be used, which simply is a :code:`EnumConverter<VehicleTypeEnum`, the constructor argument 
signals to ignore the case (upper-case/lower-case) for parsing the enum.

.. code-block:: csharp

    private class CsvVehicleMapping : CsvMapping<Vehicle>
    {
        public CsvVehicleMapping()
        {
            MapProperty(0, x => x.VehicleType, new EnumConverter<VehicleTypeEnum>(true));
            MapProperty(1, x => x.Name);
        }
    }

And then the CSV data can be parsed as usual:

.. code-block:: csharp

    [Test]
    public void CustomEnumConverterTest()
    {
        CsvParserOptions csvParserOptions = new CsvParserOptions(true, ';');
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

How **easy** was that?

.. _TinyCsvParser: https://github.com/bytefish/TinyCsvParser
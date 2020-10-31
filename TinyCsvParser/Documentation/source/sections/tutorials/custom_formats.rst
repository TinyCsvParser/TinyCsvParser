.. _tutorials_custom_formats:

Parsing Custom Formats
======================

`TinyCsvParser`_ makes assumptions about the format of the data, which defaults to the .NET default 
formats. This is often sufficient for simple CSV files, but sometimes CSV data comes with values in 
special formats. When the default converter is unable to parse the format, you need to customize the 
converter.

It sounds more complex, than it actually turns out to be. All existing converters support customizing 
the the format used for parsing the value. The formatting string is the same as used for parsing string 
values in .NET.

Reading a Date with a custom Format
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Imagine a client sends data with a weird format for dates and writes dates like this :code:`2004###01###25`. 
These values cannot be parsed with the default date format, but in `TinyCsvParser`_ a :code:`DateTimeConverter` 
with the custom date time format can be used for the mapping.

To use the custom converter, you are simply pass the Converter to the :code:`MapProperty` method to define a custom 
converter for the property mapping.

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

Reading a Boolean with a custom Format
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Imagine you want to map between the CSV value and a boolean. The library makes the assumption, that the 
string value for true is :code:`"true"` and for false is :code:`"false"`. But now imagine your CSV data 
uses the text :code:`"ThisIsTrue"` for the boolean value :code:`true`, and :code:`"ThisIsFalse"` for 
the boolean value :code:`false`. 

Then you have to instantiate and use the :code:`BoolConverter` like this:

.. code-block:: csharp

	new BoolConverter("ThisIsTrue", "ThisIsFalse", StringComparison.InvariantCulture);
	
This converter can be used in a Property Mapping like this:

.. code-block:: csharp

    public class EntityWithBoolean
    {
        public bool PropertyBoolean { get; set; }
    }
    
    public class BooleanMappingWithCustomConverter : CsvMapping<EntityWithBoolean>
    {
        public BooleanMappingWithCustomConverter()
        {
            MapProperty(0, x => x.PropertyBoolean, new BoolConverter("ThisIsTrue", "ThisIsFalse", StringComparison.InvariantCulture));
        }
    }

.. _TinyCsvParser: https://github.com/bytefish/TinyCsvParser
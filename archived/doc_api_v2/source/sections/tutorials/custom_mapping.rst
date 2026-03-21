.. _tutorials_custom_mapping:

Custom Mapping
==============

In some cases you may need to map CSV columns onto properties of your output entity in a way that is not 1:1, or your
output entity may contain other non-scalar types which you need to populate using multiple columns from the CSV row.
This is where :code:`MapUsing` comes into play. MapUsing accepts a delegate which will be called for each non-empty,
non-comment row in your CSV. The call to the supplied delegate happens *after* all of the :code:`MapProperty` mappings
have executed for that row, so your entity maybe partially populated with data by the time your delegate executes.

Example
~~~~~~~

First, like any other mapping, you need to establish an implementation of :code:`CsvMapping<MyEntity>`.

.. code-block:: csharp

    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class MyEntity
    {
        public string OrderId { get; set; }
        public Person Customer { get; set; }
    }

    // CSV format: 1234,Doe,John
    public class MyMap : CsvMapping<MyEntity>
    {
        public MyMap()
        {
            MapProperty(0, x => x.OrderId);
            // TODO: Map person using MapUsing()
        }
    }


Adding MapUsing
~~~~~~~~~~~~~~~

Now we have our first column mapped to the OrderId, but how can we map the second & third columns to an instance of our :code:`Person` class?
:code:`MapUsing` to the rescue!

.. code-block:: csharp

    // CSV format: 1234,Doe,John
    public class MyMap : CsvMapping<MyEntity>
    {
        public MyMap()
        {
            MapProperty(0, x => x.OrderId);
            MapUsing((entity, values) =>
            {
                // TODO: Invalidate the row if first name is missing.

                var customer = new Person();

                // WARNING: IndexOutOfRangeException could happen here!!
                customer.LastName = values.Tokens[1];
                customer.FirstName = values.Tokens[2];
                
                entity.Customer = customer;

                return true;
            });
        }
    }

Getting Defensive
~~~~~~~~~~~~~~~~~

Great! Now our :code:`MyEntity` class will get correctly populated with the order ID and a :code:`Person` instance with the correct
first & last name set. But what happens if we encounter a row that is missing the first name, along the lines of "1234,Acme Inc"?
This is bad news, especially if multiple rows could be missing the third column ... each row will raise an exception, which would be
very detrimental to parsing performance. That's why we require your :code:`MapUsing` delegate to return a boolean, indicating
whether the data you mapped resulted in a valid row.

Note: you should avoid doing things that could raise exceptions within your
delegate, **even** if you use :code:`try...catch`. The very fact that the exception is raised will slow your CSV parsing down
tremendously, even if it is caught and discarded.


.. code-block:: csharp

    // CSV format: 1234,Doe,John
    public class MyMap : CsvMapping<MyEntity>
    {
        public MyMap()
        {
            MapProperty(0, x => x.OrderId);
            MapUsing((entity, values) =>
            {
                // Checking that we have enough data and that the data is within range
                // should happen before we try to access & map it below.
                if(values.Tokens.Length < 3)
                {
                    return false;
                }

                var customer = new Person();

                customer.LastName = values.Tokens[1];
                customer.FirstName = values.Tokens[2];
                
                entity.Customer = customer;

                return true;
            });
        }
    }


.. _TinyCsvParser: https://github.com/bytefish/TinyCsvParser
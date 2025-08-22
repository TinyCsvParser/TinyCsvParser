using NUnit.Framework;
using System;
using System.Linq;
using System.Text;
using TinyCsvParser.Mapping;

namespace TinyCsvParser.Test.CsvParser
{
    [TestFixture]
    public class MapConstructorUsingTest
    {
        private class MainClass
        {
            public MainClass(SubClass subClass)
			{
                SubClass = subClass;
			}

            public MainClass(int property2)
			{
                Property2 = property2;
			}

            public string Property1 { get; set; }

            public SubClass SubClass { get; }

            public int Property2 { get; }
        }

        private class SubClass
        {
            public string Property3 { get; set; }

            public string Property4 { get; set; }
        }

        private class CsvMainClassMapping : CsvMapping<MainClass>
        {
            public CsvMainClassMapping()
            {
                MapProperty(0, x => x.Property1);
                MapUsing((values) =>
                {
                    // Example of invalidating the row based on its contents
                    if (values.Tokens.Any(t => t == "Z"))
                    {
                        return null;
                    }

                    var subClass = new SubClass();

                    subClass.Property3 = values.Tokens[1];
                    subClass.Property4 = values.Tokens[2];

                    var entity = new MainClass(subClass);

                    return entity;
                });
            }
        }

        private class CsvMainClassMappingDefaultToActivator : CsvMapping<MainClass>
        {
            public CsvMainClassMappingDefaultToActivator()
            {
                MapProperty(0, x => x.Property1);
                MapUsing((values) =>
                {
                    // Example of invalidating the row based on its contents
                    if (values.Tokens.Any(t => t == "Z"))
                    {
                        return null;
                    }

                    var subClass = new SubClass();

                    subClass.Property3 = values.Tokens[1];
                    subClass.Property4 = values.Tokens[2];

                    var entity = new MainClass(subClass);

                    return entity;
                });

                MapConstructorParameter<int>(3, 0);
            }
        }

        [Test]
        public void MapUsingTest()
        {
            CsvParserOptions csvParserOptions = new CsvParserOptions(false, ';' );
            CsvReaderOptions csvReaderOptions = new CsvReaderOptions(new[] { Environment.NewLine });
            CsvMainClassMapping csvMapper = new CsvMainClassMapping();
            CsvParser<MainClass> csvParser = new CsvParser<MainClass>(csvParserOptions, csvMapper);

            var stringBuilder = new StringBuilder()
                .AppendLine("X;Y;Z;4")
                .AppendLine("A;B;C;5");

            var result = csvParser
                .ReadFromString(csvReaderOptions, stringBuilder.ToString())
                .ToList();

            Assert.AreEqual(2, result.Count);

            Assert.IsFalse(result[0].IsValid);
            Assert.IsTrue(result[1].IsValid);
            
            Assert.AreEqual("A", result[1].Result.Property1);
            
            Assert.IsNotNull(result[1].Result.SubClass);

            Assert.AreEqual("B", result[1].Result.SubClass.Property3);
            Assert.AreEqual("C", result[1].Result.SubClass.Property4);
        }

        [Test]
        public void MapUsingDefaultToActivatorTest()
        {
            CsvParserOptions csvParserOptions = new CsvParserOptions(false, ';');
            CsvReaderOptions csvReaderOptions = new CsvReaderOptions(new[] { Environment.NewLine });
            CsvMainClassMappingDefaultToActivator csvMapper = new CsvMainClassMappingDefaultToActivator();
            CsvParser<MainClass> csvParser = new CsvParser<MainClass>(csvParserOptions, csvMapper);

            var stringBuilder = new StringBuilder()
                .AppendLine("X;Y;Z;4")
                .AppendLine("A;B;C;5");

            var result = csvParser
                .ReadFromString(csvReaderOptions, stringBuilder.ToString())
                .ToList();

            Assert.AreEqual(2, result.Count);

            Assert.IsTrue(result[0].IsValid);
            Assert.AreEqual("X", result[0].Result.Property1);
            Assert.IsNull(result[0].Result.SubClass);
            Assert.AreEqual(4, result[0].Result.Property2);

            Assert.IsTrue(result[1].IsValid);
            Assert.AreEqual("A", result[1].Result.Property1);
            Assert.IsNotNull(result[1].Result.SubClass);
            Assert.AreEqual("B", result[1].Result.SubClass.Property3);
            Assert.AreEqual("C", result[1].Result.SubClass.Property4);
        }
    }
}

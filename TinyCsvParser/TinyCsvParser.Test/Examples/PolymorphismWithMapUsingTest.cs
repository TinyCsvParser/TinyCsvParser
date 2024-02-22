// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using System.Linq;
using System.Text;
using TinyCsvParser.Mapping;
using TinyCsvParser.Model;

namespace TinyCsvParser.Test.Examples
{
    [TestFixture]
    public class PolymorphismWithMapUsingTest
    {
        private class MyCsvRowEntity
        {
            public string ShapeName { get; set; }

            public Shape Shape { get; set; }
        }

        private abstract class Shape { }

        private class Square : Shape
        {
            public int Width { get; set; }
        }

        private class Triangle : Shape
        {
            public int Side1 { get; set; }
            public int Side2 { get; set; }
            public int Side3 { get; set; }
        }

        private class MainMapping : CsvMapping<MyCsvRowEntity>
        {
            private readonly SquareMapping squareMap = new SquareMapping();
            private readonly TriangleMapping triangleMap = new TriangleMapping();

            public MainMapping()
            {
                MapProperty(0, x => x.ShapeName);
                MapUsing(ShapeMapper);  // You could also do this with an inline Lambda here...
            }

            private bool ShapeMapper(MyCsvRowEntity inProgressEntity, TokenizedRow rowData)
            {
                // NOTE: Be defensive and validate every piece of data you consume,
                // don't do things that will raise exceptions. (This same rule would
                // apply with any TypeConverter you write)... This gets called for
                // every row in your csv; if exceptions get raised within the loop
                // EVEN if you catch them), your performance will suffer dramatically!


                // We could also check this with tokens[0], but since we already mapped
                // it via MapProperty, we'll use the value on the entity that we're in
                // the process of building.
                if (inProgressEntity.ShapeName == "square")
                {
                    var subMap = squareMap.Map(rowData, 1);
                    if (subMap.IsValid)
                    {
                        inProgressEntity.Shape = subMap.Result;
                    }

                    return subMap.IsValid;
                }

                if (inProgressEntity.ShapeName == "triangle")
                {
                    var subMap = triangleMap.Map(rowData, 1);
                    if (subMap.IsValid)
                    {
                        inProgressEntity.Shape = subMap.Result;
                    }

                    return subMap.IsValid;
                }

                // NOTE: There are two possible strategies here. One, you can return true
                // which will allow any *OTHER* Row Mappings to run and see if they can make
                // sense of the data from this row. Maybe you have a whole separate mapper
                // for Circles & Ellipses, etc. Alternatively, you can return false, which
                // will cause this row to be marked as Invalid, SKIP any other Row Mappings,
                // and allow parsing to continue to subsequent rows.

                return false;
            }
        }


        // NOTE: These "sub" maps aren't referenced directly by the CsvParser, but instead
        // by the top-level map, which conditionally uses them based on the contents of a
        // given row.

        private class SquareMapping : CsvMapping<Square>
        {
            public SquareMapping()
            {
                MapProperty(1, s => s.Width);
            }
        }

        private class TriangleMapping : CsvMapping<Triangle>
        {
            public TriangleMapping()
            {
                MapProperty(1, s => s.Side1);
                MapProperty(2, s => s.Side2);
                MapProperty(3, s => s.Side3);
            }
        }



        [Test]
        public void MapUsingPolymorphicTest()
        {
            CsvParserOptions csvParserOptions = new CsvParserOptions(false, ';');
            CsvReaderOptions csvReaderOptions = new CsvReaderOptions(new[] { Environment.NewLine });
            MainMapping csvMapper = new MainMapping();

            CsvParser<MyCsvRowEntity> csvParser = new CsvParser<MyCsvRowEntity>(csvParserOptions, csvMapper);

            var stringBuilder = new StringBuilder()
                .AppendLine("square;10")
                .AppendLine("triangle;3;4;5")
                .AppendLine("circle;7")         // not handled, default return of false from the MapUsing delegate will fail this.
                .AppendLine("triangle;13;2");   // not enough sides, this will fail.


            var result = csvParser
                .ReadFromString(csvReaderOptions, stringBuilder.ToString())
                .Items
                .ToList();

            Assert.AreEqual(4, result.Count);

            Assert.IsTrue(result[0].IsValid);
            Assert.IsTrue(result[1].IsValid);
            Assert.IsFalse(result[2].IsValid);
            Assert.IsFalse(result[3].IsValid);

            Assert.AreEqual("square", result[0].Result.ShapeName);
            Assert.AreEqual(typeof(Square), result[0].Result.Shape.GetType());
            Assert.AreEqual(10, ((Square)result[0].Result.Shape).Width);

            Assert.AreEqual("triangle", result[1].Result.ShapeName);
            Assert.AreEqual(typeof(Triangle), result[1].Result.Shape.GetType());
            Assert.AreEqual(3, ((Triangle)result[1].Result.Shape).Side1);
            Assert.AreEqual(4, ((Triangle)result[1].Result.Shape).Side2);
            Assert.AreEqual(5, ((Triangle)result[1].Result.Shape).Side3);

        }


    }
}

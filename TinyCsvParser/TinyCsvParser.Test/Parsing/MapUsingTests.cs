// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System.Globalization;
using System.Linq;
using TinyCsvParser.Models;

namespace TinyCsvParser.Test.Parsing;

[TestFixture]
public class MapUsingTests
{
    public readonly struct GeographicCoordinate
    {
        public double Latitude { get; }
        public double Longitude { get; }

        public GeographicCoordinate(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }
    }

    public class DepthCell
    {
        public GeographicCoordinate Coordinate { get; set; }
        public double Z10 { get; set; }
        public double Z25 { get; set; }
    }

    public sealed class DepthCellMapper : CsvMapping<DepthCell>
    {
        public DepthCellMapper()
        {
            MapUsing((DepthCell entity, ref CsvRow row) =>
            {
                if (row.Count < 4) return false;

                if (!double.TryParse(row.GetSpan(0), NumberStyles.Float, CultureInfo.InvariantCulture, out double lat) ||
                    !double.TryParse(row.GetSpan(1), NumberStyles.Float, CultureInfo.InvariantCulture, out double lon) ||
                    !double.TryParse(row.GetSpan(2), NumberStyles.Float, CultureInfo.InvariantCulture, out double z10) ||
                    !double.TryParse(row.GetSpan(3), NumberStyles.Float, CultureInfo.InvariantCulture, out double z25))
                {
                    return false;
                }

                entity.Coordinate = new GeographicCoordinate(lat, lon);
                entity.Z10 = z10;
                entity.Z25 = z25;

                return true;
            });
        }
    }

    [TestFixture]
    public class DepthCellMapperTests
    {
        private CsvParser<DepthCell> _parser;

        [SetUp]
        public void Setup()
        {
            var options = new CsvOptions(
                Delimiter: ';',
                QuoteChar: '"',
                EscapeChar: '\\',
                SkipHeader: false
            );

            var mapper = new DepthCellMapper();
            _parser = new CsvParser<DepthCell>(options, mapper);
        }

        [Test]
        public void Map_ValidRow_ReturnsSuccessAndCorrectValues()
        {
            // Arrange
            string csvRow = "52.2734;7.6221;10.5;25.2";

            // Act
            var results = _parser.ReadFromString(csvRow).ToList();

            // Assert
            Assert.That(results, Has.Count.EqualTo(1));
            Assert.That(results[0].IsSuccess, Is.True);

            var cell = results[0].Result;
            Assert.Multiple(() =>
            {
                Assert.That(cell.Coordinate.Latitude, Is.EqualTo(52.2734).Within(0.0001));
                Assert.That(cell.Coordinate.Longitude, Is.EqualTo(7.6221).Within(0.0001));
                Assert.That(cell.Z10, Is.EqualTo(10.5));
                Assert.That(cell.Z25, Is.EqualTo(25.2));
            });
        }

        [Test]
        public void Map_InvalidData_ReturnsMappingError()
        {
            // Arrange
            string csvRow = "not_a_number;7.6221;10.5;25.2";

            // Act
            var results = _parser.ReadFromString(csvRow).ToList();

            // Assert
            Assert.That(results, Has.Count.EqualTo(1));
            Assert.That(results[0].IsSuccess, Is.False);
            Assert.That(results[0].Error.Value, Contains.Substring("Custom MapUsing validation/mapping failed."));
        }

        [Test]
        public void Map_MissingColumns_ReturnsMappingError()
        {
            // Arrange - only 2 columns provided instead of 4
            string csvRow = "52.2734;7.6221";

            // Act
            var results = _parser.ReadFromString(csvRow).ToList();

            // Assert
            Assert.That(results[0].IsSuccess, Is.False);
        }

        [Test]
        public void Map_MultipleRows_ParsesAllCorrectly()
        {
            // Arrange
            string csvContent = "52.0;7.0;10.0;20.0\n53.0;8.0;11.0;21.0";

            // Act
            var results = _parser.ReadFromString(csvContent).ToList();

            // Assert
            Assert.That(results, Has.Count.EqualTo(2));
            Assert.That(results.All(r => r.IsSuccess), Is.True);
            Assert.That(results[0].Result.Coordinate.Latitude, Is.EqualTo(52.0));
            Assert.That(results[1].Result.Coordinate.Latitude, Is.EqualTo(53.0));
        }
    }
}
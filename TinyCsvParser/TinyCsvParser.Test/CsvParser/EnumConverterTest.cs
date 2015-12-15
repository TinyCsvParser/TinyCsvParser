using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using System.Text;
using TinyCsvParser.Mapping;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.Test.CsvParser
{
    [TestFixture]
    public class EnumConverterTest
    {
        private enum VehicleTypeEnum
        {
            Car,
            Bike
        }

        private class Vehicle
        {
            public VehicleTypeEnum VehicleType { get; set; }
        }

        private class CsvVehicleMapping : CsvMapping<Vehicle>
        {
            public CsvVehicleMapping()
            {
                MapProperty(0, x => x.VehicleType, new EnumConverter<VehicleTypeEnum>(true));
            }
        }

        [Test]
        public void CustomEnumConverterTest()
        {
            CsvParserOptions csvParserOptions = new CsvParserOptions(false, new[] { ';' });
            CsvReaderOptions csvReaderOptions = new CsvReaderOptions(new[] { Environment.NewLine });
            CsvVehicleMapping csvMapper = new CsvVehicleMapping();
            CsvParser<Vehicle> csvParser = new CsvParser<Vehicle>(csvParserOptions, csvMapper);

            var stringBuilder = new StringBuilder()
                .AppendLine("Car")
                .AppendLine("Bike");

            var result = csvParser
                .ReadFromString(csvReaderOptions, stringBuilder.ToString())
                .ToList();

            Assert.AreEqual(VehicleTypeEnum.Car, result[0].Result.VehicleType);
            Assert.AreEqual(VehicleTypeEnum.Bike, result[1].Result.VehicleType);
        }
    }
}

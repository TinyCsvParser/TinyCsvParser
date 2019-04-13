// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyCsvParser.Mapping;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.Test.CsvParser
{
    [TestClass]
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

            public string Name { get; set; }
        }

        private class CsvVehicleMapping : CsvMapping<Vehicle>
        {
            public CsvVehicleMapping()
            {
                MapProperty(0, x => x.VehicleType, new EnumConverter<VehicleTypeEnum>(true));
                MapProperty(1, x => x.Name);
            }
        }

        [TestMethod]
        public async Task CustomEnumConverterTest()
        {
            CsvParserOptions csvParserOptions = new CsvParserOptions(true, ';');
            CsvReaderOptions csvReaderOptions = new CsvReaderOptions(new[] { Environment.NewLine });
            CsvVehicleMapping csvMapper = new CsvVehicleMapping();
            CsvParser<Vehicle> csvParser = new CsvParser<Vehicle>(csvParserOptions, csvMapper);

            var stringBuilder = new StringBuilder()
                .AppendLine("VehicleType;Name")
                .AppendLine("Car;Suzuki Swift")
                .AppendLine("Bike;A Bike");

            var result = await csvParser
                .ReadFromStringAsync(csvReaderOptions, stringBuilder.ToString())
                .ToListAsync();

            Assert.AreEqual(VehicleTypeEnum.Car, result[0].Result.VehicleType);
            Assert.AreEqual("Suzuki Swift", result[0].Result.Name);

            Assert.AreEqual(VehicleTypeEnum.Bike, result[1].Result.VehicleType);
            Assert.AreEqual("A Bike", result[1].Result.Name);
        }
    }
}

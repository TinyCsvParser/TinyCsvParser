// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NUnit.Framework;
using System;
using System.Linq;
using System.Text;
using TinyCsvParser.Core;

namespace TinyCsvParser.Test.CsvParser;

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

        public string Name { get; set; }
    }

    private class CsvVehicleMapping : CsvMapping<Vehicle>
    {
        public CsvVehicleMapping()
        {
            MapProperty(0, x => x.VehicleType, new EnumConverter<VehicleTypeEnum>());
            MapProperty(1, x => x.Name);
        }
    }

    [Test]
    public void CustomEnumConverterTest()
    {
        var csvParserOptions = CsvOptions.Default with { SkipHeader = true };
        var csvMapper = new CsvVehicleMapping();
        var csvParser = new CsvParser<Vehicle>(csvParserOptions, csvMapper);

        var stringBuilder = new StringBuilder()
            .AppendLine("VehicleType;Name")
            .AppendLine("Car;Suzuki Swift")
            .AppendLine("Bike;A Bike");

        var result = csvParser
            .ReadFromString(stringBuilder.ToString())
            .ToList();

        Assert.AreEqual(VehicleTypeEnum.Car, result[0].Result.VehicleType);
        Assert.AreEqual("Suzuki Swift", result[0].Result.Name);

        Assert.AreEqual(VehicleTypeEnum.Bike, result[1].Result.VehicleType);
        Assert.AreEqual("A Bike", result[1].Result.Name);
    }
}
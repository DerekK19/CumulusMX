﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CumulusMX.Data;
using CumulusMX.Data.Statistics.Unit;
using CumulusMX.Extensions.Station;
using CumulusMXTest.Common;
using Newtonsoft.Json;
using UnitsNet;
using UnitsNet.Serialization.JsonNet;
using UnitsNet.Units;
using Xunit;

namespace CumulusMXTest.Data
{
    public class WeatherDataStatisticsTest : TestBase
    {
        [Fact]
        public void SimpleCreationTest()
        {
            var wds = new WeatherDataStatistics();
            wds.DefineStatistic("OutdoorTemperature", typeof(Temperature));
            wds.DefineStatistic("OutdoorHumidity", typeof(Ratio));
            wds.DefineStatistic("WindSpeed", typeof(Speed));
            wds.DefineStatistic("WindBearing", typeof(Angle));
            wds.DefineStatistic("SolarRadiation", typeof(Irradiance));
            var wdm = new WeatherDataModel()
            {
                Timestamp = DateTime.Parse("2019-04-01 18:45"),
                OutdoorTemperature = Temperature.FromDegreesCelsius(20),
                OutdoorHumidity = Ratio.FromPercent(80),
                WindSpeed = Speed.FromKilometersPerHour(20),
                WindBearing = Angle.FromDegrees(45),
                SolarRadiation = Irradiance.FromKilowattsPerSquareCentimeter(10)
            };
            wds.Add(wdm);
            
            Assert.Equal(20, ((IStatistic<Temperature>) wds["OutdoorTemperature"]).DayMaximum.DegreesCelsius);
            Assert.Equal(0.8, ((IStatistic<Ratio>)wds["OutdoorHumidity"]).RecordMaximum.DecimalFractions);
            Assert.True((bool)((IStatistic<Ratio>)wds["OutdoorHumidity"]).RecordNow);
            Assert.Equal(20000, ((IStatistic<Speed>)wds["WindSpeed"]).DayAverage.MetersPerHour);
        }

        [Fact]
        public void SerialiseDeserialiseTest()
        {
            var wds = new WeatherDataStatistics();
            wds.DefineStatistic("OutdoorTemperature", typeof(Temperature));
            wds.DefineStatistic("OutdoorHumidity", typeof(Ratio));
            wds.DefineStatistic("WindSpeed", typeof(Speed));
            wds.DefineStatistic("WindBearing", typeof(Angle));
            wds.DefineStatistic("SolarRadiation", typeof(Irradiance));
            var wdm = new WeatherDataModel()
            {
                Timestamp = DateTime.Parse("2019-04-01 18:45"),
                OutdoorTemperature = Temperature.FromDegreesCelsius(20),
                OutdoorHumidity = Ratio.FromPercent(80),
                WindSpeed = Speed.FromKilometersPerHour(20),
                WindBearing = Angle.FromDegrees(45),
                SolarRadiation = Irradiance.FromKilowattsPerSquareCentimeter(10)
            };
            wds.Add(wdm);
            wdm = new WeatherDataModel()
            {
                Timestamp = DateTime.Parse("2019-04-01 18:46"),
                OutdoorTemperature = Temperature.FromDegreesCelsius(21),
                OutdoorHumidity = Ratio.FromPercent(82),
                WindSpeed = Speed.FromKilometersPerHour(18),
                WindBearing = Angle.FromDegrees(48),
                SolarRadiation = Irradiance.FromKilowattsPerSquareCentimeter(4)
            };
            wds.Add(wdm);
            var serialiser = new JsonSerializer();
            serialiser.Converters.Add(new UnitsNetJsonConverter());
            serialiser.TypeNameHandling = TypeNameHandling.Auto;
            var stringWriter = new StringWriter();
            serialiser.Serialize(stringWriter, wds);
            var theString = stringWriter.ToString();
            var textReader = new StringReader(theString);
            var reader = new JsonTextReader(textReader);
            var newWds = serialiser.Deserialize<WeatherDataStatistics>(reader);
            Assert.Equal(21, (((IStatistic<Temperature>) newWds["OutdoorTemperature"]).DayMaximum).DegreesCelsius);
            Assert.Equal(0.82, (((IStatistic <Ratio>) newWds["OutdoorHumidity"]).RecordMaximum).DecimalFractions);
            Assert.True((bool)((IStatistic<Ratio>)newWds["OutdoorHumidity"]).RecordNow);
            Assert.Equal(19000, (((IStatistic<Speed>) newWds["WindSpeed"]).DayAverage).MetersPerHour,3);
        }
    }
}
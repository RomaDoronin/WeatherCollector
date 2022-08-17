using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherCollector.WeatherDataSource
{
    internal class VentuskyWeather : IWeatherDataSource
    {
        private int dayInWeek = 7;
        private int timestampNumber = 8;
        private int dataAmount;

        public VentuskyWeather()
        {
            dataAmount = dayInWeek * timestampNumber;
        }

        public List<string> StationList => new List<string>()
        {
            "shakhunya",
            "nizhny-novgorod",
            "vyksa"
        };
        public bool IsDivideDayNight => false;

        public string GetUrl(string station)
        {
            return "https://www.ventusky.com/ru/" + station + "#forecast";
        }

        public void FindTemperature(string source, WeekWeather currentWeekWeather)
        {
            var commonKeyForParametr = "d_1\":";
            var beginKeys = new List<string>() { "\"td\":\"" };
            var endKey = ' ';

            var temperatureParametrs = WeatherProvider.FindParametrs(source, commonKeyForParametr, beginKeys, endKey, dataAmount);
            for (var dayCount = 0; dayCount < dayInWeek; dayCount++)
            {
                var weekMinTemperature = int.MaxValue;
                var weekMaxTemperature = int.MinValue;
                for (var timestampCount = 0; timestampCount < timestampNumber; timestampCount++)
                {
                    var temperature = int.Parse(temperatureParametrs[dayCount * timestampNumber + timestampCount]);
                    if (temperature < weekMinTemperature)
                    {
                        weekMinTemperature = temperature;
                    }
                    if (temperature > weekMaxTemperature)
                    {
                        weekMaxTemperature = temperature;
                    }
                }
                currentWeekWeather.SetTemperature(weekMaxTemperature.ToString(), dayCount + 1, WeekWeather.TimeOfDay.Day);
                currentWeekWeather.SetTemperature(weekMinTemperature.ToString(), dayCount + 1, WeekWeather.TimeOfDay.Night);
            }
        }

        public void FindPrecipitation(string source, WeekWeather currentWeekWeather)
        {
            var commonKeyForParametr = "d_1\":";
            var beginKeys = new List<string>() { "\"srd\":\"" };
            var endKey = ' ';

            var precipitationParametrs = WeatherProvider.FindParametrs(source, commonKeyForParametr, beginKeys, endKey, dataAmount);
            for (var dayCount = 0; dayCount < dayInWeek; dayCount++)
            {
                double dayPrecipitation = 0;
                for (var timestampCount = 0; timestampCount < timestampNumber; timestampCount++)
                {
                    var stringPrecipitation = precipitationParametrs[dayCount * timestampNumber + timestampCount];
                    dayPrecipitation += Convert.ToDouble(stringPrecipitation.Replace(".", ","));

                }
                currentWeekWeather.SetPrecipitation(dayPrecipitation.ToString(), dayCount + 1, WeekWeather.TimeOfDay.Night);
            }
        }

        private readonly Dictionary<string, string> DirectionDict = new()
        {
            { "\\u0421", "С" },
            { "\\u0421\\u0412", "СЗ" },
            { "\\u0412", "З" },
            { "\\u042e\\u0412", "ЮЗ" },
            { "\\u042e", "Ю" },
            { "\\u042e\\u0417", "ЮВ" },
            { "\\u0417", "В" },
            { "\\u0421\\u0417", "СВ" }
        };

        public void FindWindDirection(string source, WeekWeather currentWeekWeather)
        {
            var commonKeyForParametr = "d_1\":";
            var beginKeys = new List<string>() { "\"vdId\":\"" };
            var endKey = '"';

            var directionParametrs = WeatherProvider.FindParametrs(source, commonKeyForParametr, beginKeys, endKey, dataAmount);
            for (var dayCount = 0; dayCount < dayInWeek; dayCount++)
            {
                var fifteenHoursTimestam = 5;
                var direction = directionParametrs[dayCount * timestampNumber + fifteenHoursTimestam];
                currentWeekWeather.SetWindDirection(DirectionDict[direction], dayCount + 1, WeekWeather.TimeOfDay.Night);
            }
        }

        public void FindWindSpeed(string source, WeekWeather currentWeekWeather)
        {
            var commonKeyForParametr = "d_1\":";
            var beginKeys = new List<string>() { "\"vsd\":\"" };
            var endKey = ' ';

            var speedParametrs = WeatherProvider.FindParametrs(source, commonKeyForParametr, beginKeys, endKey, dataAmount);
            for (var dayCount = 0; dayCount < dayInWeek; dayCount++)
            {
                int speeedSum = 0;
                for (var timestampCount = 0; timestampCount < timestampNumber; timestampCount++)
                {
                    var stringSpeed = speedParametrs[dayCount * timestampNumber + timestampCount];
                    speeedSum += int.Parse(stringSpeed);
                }
                var integer = speeedSum / timestampNumber;
                var remainder = speeedSum % timestampNumber;
                var speed = 0;
                if (remainder * 2 > integer)
                {
                    speed = integer + 1;
                }
                else
                {
                    speed = integer;
                }
                currentWeekWeather.SetWindSpeed(speed.ToString(), dayCount + 1, WeekWeather.TimeOfDay.Night);
            }
        }
    }
}

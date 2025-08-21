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

        public List<string> StationList => new List<string>()
        {
            "shakhunya",
            "nizhny-novgorod",
            "vyksa"
        };
        public bool IsDivideDayNight => false;

        public string GetUrl(string station)
        {
            return "https://www.ventusky.com/ru/" + station;
        }

        public void FindTemperature(string source, WeekWeather currentWeekWeather)
        {
            var commonKeyForParametr = "temp&amp;data";
            var beginKeys = new List<string>() { "=", ":" };
            var endKey = ':';
            var dataAmount = 2;

            var temperatureParametrs = WeatherProvider.FindParametrs(source, commonKeyForParametr, beginKeys, endKey, dataAmount);
            if (temperatureParametrs.Count < dataAmount)
            {
                return;
            }
            var dayTemperature = temperatureParametrs[1].Split(';');
            var nightTemperature = temperatureParametrs[0].Split(';');
            for (var dayCount = 0; dayCount < dayInWeek; dayCount++)
            {
                currentWeekWeather.SetTemperature(dayTemperature[dayCount].ToString(), dayCount, WeekWeather.TimeOfDay.Day);
                currentWeekWeather.SetTemperature(nightTemperature[dayCount].ToString(), dayCount, WeekWeather.TimeOfDay.Night);
            }
        }

        public void FindPrecipitation(string source, ref WeekWeather currentWeekWeather)
        {
            var commonKeyForParametr = "temp&amp;data";
            var beginKeys = new List<string>() { "=", ":" };
            var endKey = ':';
            var dataAmount = 3;

            var precipitationParametrs = WeatherProvider.FindParametrs(source, commonKeyForParametr, beginKeys, endKey, dataAmount);
            if (precipitationParametrs.Count < dataAmount)
            {
                return;
            }
            var stringPrecipitation = precipitationParametrs[2].Split(';');

            for (var dayCount = 0; dayCount < dayInWeek; dayCount++)
            {
                var precipitation = Convert.ToDouble(stringPrecipitation[dayCount].Replace(".", ","));
                currentWeekWeather.SetPrecipitation(Math.Round(precipitation, 1).ToString(), dayCount, WeekWeather.TimeOfDay.Night);
            }
        }

        private readonly Dictionary<string, string> DirectionDict = new()
        {
            { "u0421", "С" },
            { "u0421\\u0412", "СЗ" },
            { "u0412", "З" },
            { "u042e\\u0412", "ЮЗ" },
            { "u042e", "Ю" },
            { "u042e\\u0417", "ЮВ" },
            { "u0417", "В" },
            { "u0421\\u0417", "СВ" }
        };

        public void FindWindDirection(string source, WeekWeather currentWeekWeather)
        {
            var commonKeyForParametr = "\"vdId\":[";
            var beginKeys = new List<string>() { "\"\\" };
            var endKey = '"';
            var dataAmount = dayInWeek;

            var directionParametrs = WeatherProvider.FindParametrs(source, commonKeyForParametr, beginKeys, endKey, dataAmount);
            for (var dayCount = 0; dayCount < dayInWeek; dayCount++)
            {
                currentWeekWeather.SetWindDirection(DirectionDict[directionParametrs[dayCount]], dayCount, WeekWeather.TimeOfDay.Night);
            }
        }

        public void FindWindSpeed(string source, WeekWeather currentWeekWeather)
        {
            var commonKeyForParametr = "temp&amp;data";
            var beginKeys = new List<string>() { "=", ":" };
            var endKey = ':';
            var dataAmount = 4;

            var windSpeedParametrs = WeatherProvider.FindParametrs(source, commonKeyForParametr, beginKeys, endKey, dataAmount);
            if (windSpeedParametrs.Count < dataAmount)
            {
                return;
            }
            var stringWindSpeed = windSpeedParametrs[3].Split(';');

            for (var dayCount = 0; dayCount < dayInWeek; dayCount++)
            {
                var speed = Convert.ToDouble(stringWindSpeed[dayCount]) / 3.6;
                currentWeekWeather.SetWindSpeed(Math.Round(speed).ToString(), dayCount + 1, WeekWeather.TimeOfDay.Night);
            }
        }
    }
}

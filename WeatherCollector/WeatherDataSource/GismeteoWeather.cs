using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherCollector.WeatherDataSource
{
    public class GismeteoWeather : IWeatherDataSource
    {
        public List<string> StationList => new List<string>()
        {
            "shakhunya-4322",
            "nizhny-novgorod-4355",
            "vyksa-4375"
        };
        public bool IsDivideDayNight => false;

        public string GetUrl(string station)
        {
            return "https://www.gismeteo.ru/weather-" + station + "/10-days/";
        }

        public void FindTemperature(string source, WeekWeather currentWeekWeather)
        {
            var commonKeyForParametr = "widget-row-chart widget-row-chart-temperature-air";
            var beginKeys = new List<string>() { "temperature-value value=\"" };
            var endKey = '\"';
            var extraCelsius = 1;
            var dataAmount = (WeatherProvider.numberForecastDaysMax + 1) * 2 + extraCelsius;
            var temperatureParametrs = WeatherProvider.FindParametrs(source, commonKeyForParametr, beginKeys, endKey, dataAmount);
            temperatureParametrs.RemoveAt(0);
            for (int count = 1; count < temperatureParametrs.Count; count++)
            {
                var dayIndex = (count + 1) / 2;
                var temperature = temperatureParametrs[count].Replace("&minus;", "-");
                switch (count % 2)
                {
                    case 0:
                        currentWeekWeather.SetTemperature(temperature, dayIndex, WeekWeather.TimeOfDay.Night);
                        break;
                    case 1:
                        currentWeekWeather.SetTemperature(temperature, dayIndex, WeekWeather.TimeOfDay.Day);
                        break;
                }
            }
        }

        public void FindPrecipitation(string source, ref WeekWeather currentWeekWeather)
        {
            var commonKeyForParametr = "Осадки в жидком эквиваленте";
            var beginKeys = new List<string>() { "item-unit unit-blue\">", "item-unit\">" };
            var endKey = '<';
            var dataAmount = WeatherProvider.numberForecastDaysMax + 1;
            var precipitationParametrs = WeatherProvider.FindParametrs(source, commonKeyForParametr, beginKeys, endKey, dataAmount);
            for (int count = 0; count < precipitationParametrs.Count; count++)
            {
                currentWeekWeather.SetPrecipitation(precipitationParametrs[count], count, WeekWeather.TimeOfDay.Night);
            }
        }

        public void FindWindDirection(string source, WeekWeather currentWeekWeather)
        {
            var commonKeyForParametr = "widget-row-wind";
            var beginKeys = new List<string>() { "/></svg></div>" };
            var endKey = '<';
            var dataAmount = WeatherProvider.numberForecastDaysMax + 1;
            var windDirectionParametrs = WeatherProvider.FindParametrs(source, commonKeyForParametr, beginKeys, endKey, dataAmount);
            for (int count = 0; count < windDirectionParametrs.Count; count++)
            {
                currentWeekWeather.SetWindDirection(windDirectionParametrs[count], count, WeekWeather.TimeOfDay.Night);
            }
        }

        public void FindWindSpeed(string source, WeekWeather currentWeekWeather)
        {
            var commonKeyForParametr = "widget-row-wind";
            var beginKeys = new List<string>() { "speed-value value=\"" };
            var endKey = '\"';
            var dataAmount = (WeatherProvider.numberForecastDaysMax + 1) * 2;
            var windSpeedParametrs = WeatherProvider.FindParametrs(source, commonKeyForParametr, beginKeys, endKey, dataAmount);

            var minWindSpeed = -1;
            for (int count = 0; count < windSpeedParametrs.Count; count++)
            {
                if (minWindSpeed == -1)
                {
                    minWindSpeed = int.Parse(windSpeedParametrs[count]);
                }
                else
                {
                    var maxWindSpeed = int.Parse(windSpeedParametrs[count]);
                    var averageWindSpeed = (minWindSpeed + maxWindSpeed) / 2;
                    currentWeekWeather.SetWindSpeed(averageWindSpeed.ToString(), count / 2, WeekWeather.TimeOfDay.Night);
                    minWindSpeed = -1;
                }
            }
        }
    }
}

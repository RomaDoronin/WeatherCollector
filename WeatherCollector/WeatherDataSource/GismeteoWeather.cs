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
            var commonKeyForParametr = "widget-row-chart widget-row-chart-temperature";
            var beginKeys = new List<string>() { "unit_temperature_c\">" };
            var endKey = '<';
            var dataAmount = (WeatherProvider.numberForecastDaysMax + 1) * 2;
            var temperatureParametrs = WeatherProvider.FindParametrs(source, commonKeyForParametr, beginKeys, endKey, dataAmount);
            for (int count = 0; count < temperatureParametrs.Count; count++)
            {
                switch (count % 2)
                {
                    case 0:
                        currentWeekWeather.SetTemperature(temperatureParametrs[count], count / 2, true);
                        break;
                    case 1:
                        currentWeekWeather.SetTemperature(temperatureParametrs[count], count / 2, false);
                        break;
                }
            }
        }

        public void FindPrecipitation(string source, WeekWeather currentWeekWeather)
        {
            var commonKeyForParametr = "Осадки, мм";
            var beginKeys = new List<string>() { "item-unit unit-blue\">", "item-unit\">" };
            var endKey = '<';
            var dataAmount = WeatherProvider.numberForecastDaysMax + 1;
            var precipitationParametrs = WeatherProvider.FindParametrs(source, commonKeyForParametr, beginKeys, endKey, dataAmount);
            for (int count = 0; count < precipitationParametrs.Count; count++)
            {
                currentWeekWeather.SetPrecipitation(precipitationParametrs[count], count, false);
            }
        }

        public void FindWindDirection(string source, WeekWeather currentWeekWeather)
        {
            var commonKeyForParametr = "widget-row-wind-direction";
            var beginKeys = new List<string>() { "\"direction\">" };
            var endKey = '<';
            var dataAmount = WeatherProvider.numberForecastDaysMax + 1;
            var precipitationParametrs = WeatherProvider.FindParametrs(source, commonKeyForParametr, beginKeys, endKey, dataAmount);
            for (int count = 0; count < precipitationParametrs.Count; count++)
            {
                currentWeekWeather.SetWindDirection(precipitationParametrs[count], count, false);
            }
        }

        public void FindWindSpeed(string source, WeekWeather currentWeekWeather)
        {
            var commonKeyForParametr = "widget-row-wind-speed\"";
            var beginKeys = new List<string>() { "unit_wind_m_s\">\n", "unit_wind_m_s warning\">\n" };
            var endKey = '\n';
            var dataAmount = WeatherProvider.numberForecastDaysMax + 1;
            var precipitationParametrs = WeatherProvider.FindParametrs(source, commonKeyForParametr, beginKeys, endKey, dataAmount);
            for (int count = 0; count < precipitationParametrs.Count; count++)
            {
                var speed = precipitationParametrs[count].Replace(" ", "");
                currentWeekWeather.SetWindSpeed(speed, count, false);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherCollector.WeatherDataSource
{
    internal class GidroMCWeather : IWeatherDataSource
    {
        public List<string> StationList => new List<string>()
        {
            "sakunja",
            "vetluga",
            "krasnye-baki",
            "voskresenskoe",
            "volzskaja-gmo",
            "niznij-novgoro",
            "lyskovo",
            "arzamas",
            "pavlovo",
            "sergac",
            "vyksa",
            "lukojanov"
        };
        public bool IsDivideDayNight => true;

        public string GetUrl(string station)
        {
            return "https://meteoinfo.ru/forecasts5000/russia/nizhegorodskaya-area/" + station;
        }

        public void FindTemperature(string source, WeekWeather currentWeekWeather)
        {
            string key = "&deg";
            int dayCount = 0;
            bool isDay = true;
            var size = source.Length - key.Length;

            for (int i = 0; i < size; i++)
            {
                if (WeatherProvider.CollectSubstring(key, source, i))
                {
                    int ampersandPosition = i;
                    var temperature = "";
                    int count = 1;
                    char ch = source[ampersandPosition - count];

                    while (ch != '>')
                    {
                        temperature = ch + temperature;
                        count++;
                        ch = source[ampersandPosition - count];
                    }

                    if (isDay)
                    {
                        currentWeekWeather.SetTemperature(temperature, dayCount, isDay);
                        dayCount++;
                        isDay = false;
                    }
                    else
                    {
                        if (dayCount == WeatherProvider.numberForecastDaysMax + 1)
                        {
                            break;
                        }
                        currentWeekWeather.SetTemperature(temperature, dayCount, isDay);
                        isDay = true;
                    }
                }
            }
        }

        public void FindPrecipitation(string source, WeekWeather currentWeekWeather)
        {
            var key = "Осадки, мм (вероятность)";
            int dayCount = 0;
            var findDayPrecipitation = FindState.NotFind;
            var findNightPrecipitation = FindState.NotFind;

            for (int i = 0; i < source.Length - key.Length; i++)
            {
                if (findDayPrecipitation == FindState.Finding || findNightPrecipitation == FindState.Finding)
                {
                    var nobrKey = "<nobr>";
                    if (WeatherProvider.CollectSubstring(nobrKey, source, i))
                    {
                        int startIndex = i + nobrKey.Length;
                        string precipitation = "";
                        char ch = source[startIndex];
                        int count = 0;
                        while (ch != '<' && ch != ')')
                        {
                            if (ch == '.')
                            {
                                precipitation += ',';
                            }
                            else
                            {
                                precipitation += ch;
                            }
                            count++;
                            ch = source[startIndex + count];
                        }

                        if (precipitation.EndsWith('%'))
                        {
                            continue;
                        }
                        else
                        {
                            if (findDayPrecipitation == FindState.Finding)
                            {
                                currentWeekWeather.SetPrecipitation(precipitation, dayCount, true);
                            }
                            else if (findNightPrecipitation == FindState.Finding)
                            {
                                currentWeekWeather.SetPrecipitation(precipitation, dayCount, false);
                            }
                            if (dayCount == WeatherProvider.numberForecastDaysMax)
                            {
                                if (findDayPrecipitation == FindState.Finding)
                                {
                                    findDayPrecipitation = FindState.Found;
                                }
                                else if (findNightPrecipitation == FindState.Finding)
                                {
                                    findNightPrecipitation = FindState.Found;
                                }
                                dayCount = 1;
                                continue;
                            }
                            dayCount++;
                        }
                    }
                }
                else if (WeatherProvider.CollectSubstring(key, source, i))
                {
                    if (findDayPrecipitation == FindState.NotFind)
                    {
                        findDayPrecipitation = FindState.Finding;
                    }
                    if (findDayPrecipitation == FindState.Found && findNightPrecipitation == FindState.NotFind)
                    {
                        findNightPrecipitation = FindState.Finding;
                    }
                }
            }
        }

        public void FindWindDirection(string source, WeekWeather currentWeekWeather)
        {
            var key = "Ветер, м/с</div>";
            int dayCount = 0;
            var findDayWind = FindState.NotFind;
            var findNightWind = FindState.NotFind;

            for (int i = 0; i < source.Length - key.Length; i++)
            {
                if (findDayWind == FindState.Finding || findNightWind == FindState.Finding)
                {
                    var titleKey = "<span title=\"";
                    if (WeatherProvider.CollectSubstring(titleKey, source, i))
                    {
                        int startIndex = i + titleKey.Length;
                        string stringWind = "";
                        char ch = source[startIndex];
                        int count = 0;
                        while (ch != '\"')
                        {
                            stringWind += ch;
                            count++;
                            ch = source[startIndex + count];
                        }

                        if (findDayWind == FindState.Finding)
                        {
                            currentWeekWeather.SetWindDirection(stringWind, dayCount, true);
                        }
                        else if (findNightWind == FindState.Finding)
                        {
                            currentWeekWeather.SetWindDirection(stringWind, dayCount, false);
                        }
                        if (dayCount == WeatherProvider.numberForecastDaysMax)
                        {
                            if (findDayWind == FindState.Finding)
                            {
                                findDayWind = FindState.Found;
                            }
                            else if (findNightWind == FindState.Finding)
                            {
                                findNightWind = FindState.Found;
                            }
                            dayCount = 1;
                            continue;
                        }
                        dayCount++;
                    }
                }
                else if (WeatherProvider.CollectSubstring(key, source, i))
                {
                    if (findDayWind == FindState.NotFind)
                    {
                        findDayWind = FindState.Finding;
                    }
                    if (findDayWind == FindState.Found && findNightWind == FindState.NotFind)
                    {
                        findNightWind = FindState.Finding;
                    }
                }
            }
        }

        public void FindWindSpeed(string source, WeekWeather currentWeekWeather)
        {
            var key = "Ветер, м/с</div>";
            int dayCount = 0;
            var findDayWind = FindState.NotFind;
            var findNightWind = FindState.NotFind;

            for (int i = 0; i < source.Length - key.Length; i++)
            {
                if (findDayWind == FindState.Finding || findNightWind == FindState.Finding)
                {
                    var nobrKey = "<nobr>";
                    if (WeatherProvider.CollectSubstring(nobrKey, source, i))
                    {
                        int startIndex = i + nobrKey.Length;
                        string stringWind = "";
                        char ch = source[startIndex];
                        int count = 0;
                        while (ch != '<')
                        {
                            stringWind += ch;
                            count++;
                            ch = source[startIndex + count];
                        }

                        if (findDayWind == FindState.Finding)
                        {
                            currentWeekWeather.SetWindSpeed(stringWind, dayCount, true);
                        }
                        else if (findNightWind == FindState.Finding)
                        {
                            currentWeekWeather.SetWindSpeed(stringWind, dayCount, false);
                        }
                        if (dayCount == WeatherProvider.numberForecastDaysMax)
                        {
                            if (findDayWind == FindState.Finding)
                            {
                                findDayWind = FindState.Found;
                            }
                            else if (findNightWind == FindState.Finding)
                            {
                                findNightWind = FindState.Found;
                            }
                            dayCount = 1;
                            continue;
                        }
                        dayCount++;
                    }
                }
                else if (WeatherProvider.CollectSubstring(key, source, i))
                {
                    if (findDayWind == FindState.NotFind)
                    {
                        findDayWind = FindState.Finding;
                    }
                    if (findDayWind == FindState.Found && findNightWind == FindState.NotFind)
                    {
                        findNightWind = FindState.Finding;
                    }
                }
            }
        }
    }
}

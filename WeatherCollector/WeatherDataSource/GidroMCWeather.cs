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
        public bool IsDivideDayNight => true; // https://meteoinfo.ru/forecasts5000/russia/nizhegorodskaya-area/sakunja

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
                        currentWeekWeather.SetTemperature(temperature, dayCount, WeekWeather.TimeOfDay.Day);
                        dayCount++;
                        isDay = false;
                    }
                    else
                    {
                        if (dayCount == WeatherProvider.numberForecastDaysMax + 1)
                        {
                            break;
                        }
                        currentWeekWeather.SetTemperature(temperature, dayCount, WeekWeather.TimeOfDay.Night);
                        isDay = true;
                    }
                }
            }
        }

        public void FindPrecipitation(string source, ref WeekWeather currentWeekWeather)
        {
            var key = "Осадки, вероятность<";
            int dayCount = 0;
            var findDayPrecipitation = FindState.NotFind;
            var findNightPrecipitation = FindState.NotFind;
            currentWeekWeather.PrecipitationHeader = "Вероятность осадков";

            var findPersent = FindState.NotFind;

            for (int i = 0; i < source.Length - key.Length; i++)
            {
                if (findDayPrecipitation == FindState.Finding || findNightPrecipitation == FindState.Finding)
                {
                    string precipitation = "";
                    var persentKey = "";
                    if (findDayPrecipitation == FindState.Finding)
                    {
                        persentKey = "fc_small_gorizont_ww\">";
                    } else if (findNightPrecipitation == FindState.Finding)
                    {
                        persentKey = "fc_small_gorizont_ww sdvig_div\">";
                    }

                    if (findPersent == FindState.Finding)
                    {
                        var nobrKey = "<nobr>";
                        if (WeatherProvider.CollectSubstring(nobrKey, source, i))
                        {
                            int startIndex = i + nobrKey.Length;
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
                            findPersent = FindState.NotFind;
                        }
                    }
                    else if (WeatherProvider.CollectSubstring(persentKey, source, i))
                    {
                        if (source[i + persentKey.Length] == '0')
                        {
                            precipitation = "0%";
                        }
                        else
                        {
                            findPersent = FindState.Finding;
                        }
                    }

                    if (precipitation.Length > 0)
                    {
                        if (findDayPrecipitation == FindState.Finding)
                        {
                            currentWeekWeather.SetPrecipitation(precipitation, dayCount, WeekWeather.TimeOfDay.Day);
                        }
                        else if (findNightPrecipitation == FindState.Finding)
                        {
                            currentWeekWeather.SetPrecipitation(precipitation, dayCount, WeekWeather.TimeOfDay.Night);
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
                            currentWeekWeather.SetWindDirection(stringWind, dayCount, WeekWeather.TimeOfDay.Day);
                        }
                        else if (findNightWind == FindState.Finding)
                        {
                            currentWeekWeather.SetWindDirection(stringWind, dayCount, WeekWeather.TimeOfDay.Night);
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
                            currentWeekWeather.SetWindSpeed(stringWind, dayCount, WeekWeather.TimeOfDay.Day);
                        }
                        else if (findNightWind == FindState.Finding)
                        {
                            currentWeekWeather.SetWindSpeed(stringWind, dayCount, WeekWeather.TimeOfDay.Night);
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

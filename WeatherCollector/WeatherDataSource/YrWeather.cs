using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace WeatherCollector.WeatherDataSource
{
    internal class YrWeather : IWeatherDataSource
    {
        private int dataAmount = 8;

        public List<string> StationList => new List<string>()
        {
            "Shakhun'ya",
            "Nizhny%20Novgorod",
            "Vyksa"
        };
        public bool IsDivideDayNight => false;

        private string GetIdByStation(string station)
        {
            if (station == StationList[0])
            {
                return "496012";
            }
            else if (station == StationList[1])
            {
                return "520555";
            }
            else if (station == StationList[2])
            {
                return "470444";
            }

            return "0";
        }

        public string GetUrl(string station)
        {
            return "https://www.yr.no/en/forecast/daily-table/2-" + GetIdByStation(station) + "/Russia/Nizhny%20Novgorod%20Oblast/" + station;
        }

        public void FindTemperature(string source, WeekWeather currentWeekWeather)
        {
            var commonKeyForParametr = "daily-weather-list__intervals";
            var dayBeginKeys = new List<string>() { "max temperature--warm\" role=\"text\">" };
            var nightBeginKeys = new List<string>() { "min temperature--warm\" role=\"text\">" };
            var endKey = '<';

            var dayTemperatureParametrs = WeatherProvider.FindParametrs(source, commonKeyForParametr, dayBeginKeys, endKey, dataAmount);
            var nightTemperatureParametrs = WeatherProvider.FindParametrs(source, commonKeyForParametr, nightBeginKeys, endKey, dataAmount);
            for (var dayCount = 0; dayCount < dataAmount; dayCount++)
            {
                currentWeekWeather.SetTemperature(dayTemperatureParametrs[dayCount], dayCount, WeekWeather.TimeOfDay.Day);
                currentWeekWeather.SetTemperature(nightTemperatureParametrs[dayCount], dayCount, WeekWeather.TimeOfDay.Night);
            }
        }

        public void FindPrecipitation(string source, WeekWeather currentWeekWeather)
        {
            var commonKeyForParametr = "daily-weather-list__intervals";
            var beginKeys = new List<string>() { "precipitation__value\">" };
            var endKey = '<';

            var precipitationParametrs = WeatherProvider.FindParametrs(source, commonKeyForParametr, beginKeys, endKey, dataAmount);
            for (var dayCount = 0; dayCount < dataAmount; dayCount++)
            {
                currentWeekWeather.SetPrecipitation(precipitationParametrs[dayCount], dayCount, WeekWeather.TimeOfDay.Night);
            }
        }

        public void FindWindDirection(string source, WeekWeather currentWeekWeather) { }

        public void FindWindSpeed(string source, WeekWeather currentWeekWeather)
        {
            var commonKeyForParametr = "daily-weather-list__intervals";
            var beginKeys = new List<string>() { "wind__value\">" };
            var endKey = '<';

            var speedParametrs = WeatherProvider.FindParametrs(source, commonKeyForParametr, beginKeys, endKey, dataAmount);
            for (var dayCount = 0; dayCount < dataAmount; dayCount++)
            {
                currentWeekWeather.SetWindSpeed(speedParametrs[dayCount], dayCount, WeekWeather.TimeOfDay.Night);
            }
        }
    }

    internal class YrWeatherWindDirection
    {
        private YrWeather yrWeather;
        private Dictionary<String, WeekWeather> weatherDict;
        private readonly IProgressBarInteraction progressBarInteraction;

        private WeekWeather currentWeekWeather;
        private int dayAmount = 8;
        private int dayNumber = 0;

        public YrWeatherWindDirection(YrWeather yrWeather, Dictionary<String, WeekWeather> weatherDict, IProgressBarInteraction progressBarInteraction)
        {
            this.yrWeather = yrWeather;
            this.weatherDict = weatherDict;
            this.progressBarInteraction = progressBarInteraction;
        }

        public void GetDataFromStations()
        {
            foreach (var station in yrWeather.StationList)
            {
                currentWeekWeather = weatherDict[station];
                for (var dayCount = 1; dayCount < dayAmount; dayCount++)
                {
                    dayNumber = dayCount;
                    SendRequestForWeather(station, dayCount.ToString());
                    progressBarInteraction.IncrementProgressCount();
                }
                weatherDict[station] = currentWeekWeather;
            }
        }

        private void SendRequestForWeather(string station, string day)
        {
            var url = "https://www.yr.no/en/forecast/hourly-table/2-496012/Russia/Nizhny%20Novgorod%20Oblast/" + station + "?i=" + day;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            Console.WriteLine("Content length is" + response.ContentLength);
            Console.WriteLine("Content type is" + response.ContentType);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Console.WriteLine("Всё норм.");

                // Get the stream associated with the response.
                Stream receiveStream = response.GetResponseStream();

                // Pipes the stream to a higher level stream reader with the required encoding format.
                StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);

                var htmlString = readStream.ReadToEnd();
                ParseHtmlString(htmlString, station);
                readStream.Close();
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                Console.WriteLine("Такой страницы нет.");
            }
            response.Close();
        }

        private void ParseHtmlString(string source, string station)
        {
            FindWindDirection(source);
        }

        private readonly Dictionary<string, string> DirectionDict = new()
        {
            { "north", "С" },
            { "north west", "СЗ" },
            { "west", "З" },
            { "south west", "ЮЗ" },
            { "south", "Ю" },
            { "south east", "ЮВ" },
            { "east", "В" },
            { "north east", "СВ" }
        };

        private void FindWindDirection(string source)
        {
            var commonKeyForParametr = "wind wind--display-arrow location-weather-table__wind-value";
            var beginKeys = new List<string>() { "from " };
            var triggerKey = new List<string>() { "<time dateTime=\"09:00\">09</time>–<time dateTime=\"15:00\">" };
            var endKey = '<';

            var triggerValue = WeatherProvider.FindParametrs(source, commonKeyForParametr, triggerKey, endKey, 1);

            var dataAmount = 0;
            var index = 0;
            if (triggerValue.Count == 0)
            {
                dataAmount = 24;
                index = 15;
            }
            else
            {
                dataAmount = 4;
                index = 2;
            }

            var directionParametrs = WeatherProvider.FindParametrs(source, commonKeyForParametr, beginKeys, endKey, dataAmount);
            var direction = directionParametrs[index];
            currentWeekWeather.SetWindDirection(DirectionDict[direction], dayNumber, WeekWeather.TimeOfDay.Night);
        }
    }
}

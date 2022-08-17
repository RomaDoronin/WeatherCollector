using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace WeatherCollector
{
    public interface IWeatherDataSource
    {
        List<String> StationList { get; }
        bool IsDivideDayNight { get; }

        string GetUrl(string station);
        void FindTemperature(string source, WeekWeather currentWeekWeather);
        void FindPrecipitation(string source, WeekWeather currentWeekWeather);
        void FindWindDirection(string source, WeekWeather currentWeekWeather);
        void FindWindSpeed(string source, WeekWeather currentWeekWeather);
    }

    public interface IProgressBarInteraction
    {
        void IncrementProgressCount();
    }

    public class WeatherProvider
    {
        public Dictionary<String, WeekWeather> weatherDict;

        private readonly IWeatherDataSource dataSource;
        private readonly IProgressBarInteraction progressBarInteraction;

        public WeatherProvider(IWeatherDataSource dataSource, IProgressBarInteraction progressBarInteraction)
        {
            this.dataSource = dataSource;
            this.progressBarInteraction = progressBarInteraction;
            weatherDict = new Dictionary<String, WeekWeather>();
        }

        public void GetDataFromStations()
        {
            foreach (var station in dataSource.StationList)
            {
                SendRequestForWeather(station);
                progressBarInteraction.IncrementProgressCount();
            }
        }

        private void SendRequestForWeather(string station)
        {
            String url = dataSource.GetUrl(station);

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
            var currentWeekWeather = new WeekWeather();

            dataSource.FindTemperature(source, currentWeekWeather);
            dataSource.FindPrecipitation(source, currentWeekWeather);
            dataSource.FindWindDirection(source, currentWeekWeather);
            dataSource.FindWindSpeed(source, currentWeekWeather);

            weatherDict[station] = currentWeekWeather;
        }

        // Static

        static public int numberForecastDaysMax = 6;

        static public List<string> FindParametrs(string source, string commonKeyForParametr, List<string> beginKeys, char endKey, int dataAmount)
        {
            var result = new List<string>();
            var dataCount = 1;
            var findState = FindState.NotFind;

            for (int i = 0; i < source.Length - commonKeyForParametr.Length; i++)
            {
                if (findState == FindState.Finding)
                {
                    foreach (var beginKey in beginKeys)
                    {
                        if (CollectSubstring(beginKey, source, i))
                        {
                            int startIndex = i + beginKey.Length;
                            string stringParametr = "";
                            char ch = source[startIndex];
                            int count = 0;
                            while (ch != endKey)
                            {
                                stringParametr += ch;
                                count++;
                                ch = source[startIndex + count];
                            }

                            result.Add(stringParametr);

                            if (dataCount == dataAmount)
                            {
                                return result;
                            }
                            dataCount++;
                        }
                    }
                }
                else if (CollectSubstring(commonKeyForParametr, source, i))
                {
                    if (findState == FindState.NotFind)
                    {
                        findState = FindState.Finding;
                    }
                }
            }

            return result;
        }

        static public bool CollectSubstring(string substring, string source, int index)
        {
            for (int i = 0; i < substring.Length; i++)
            {
                if (source[index + i] != substring[i])
                {
                    return false;
                }
            }
            return true;
        }
    }
}

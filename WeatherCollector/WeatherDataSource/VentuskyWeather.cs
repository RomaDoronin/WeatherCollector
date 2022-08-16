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

        private static double sin45 = Math.Sqrt(2) / 2;

        private readonly Dictionary<string, (double, double)> DirectionDict = new()
        {
            { "\\u0421", (0, 1) },
            { "С", (0, 1) },

            { "\\u0421\\u0412", (sin45, sin45) },
            { "СЗ", (sin45, sin45) },

            { "\\u0412", (1, 0) },
            { "З", (1, 0) },

            { "\\u042e\\u0412", (sin45, -sin45) },
            { "ЮЗ", (sin45, -sin45) },

            { "\\u042e", (0, -1) },
            { "Ю", (0, -1) },

            { "\\u042e\\u0417", (-sin45, -sin45) },
            { "ЮВ", (-sin45, -sin45) },

            { "\\u0417", (-1, 0) },
            { "В", (-1, 0) },

            { "\\u0421\\u0417", (-sin45, sin45) },
            { "СВ", (-sin45, sin45) }
        };

        private double CulcVectorLenght((double, double) vector)
        {
            return Math.Sqrt(vector.Item1 * vector.Item1 + vector.Item2 * vector.Item2);
        }


        private string CompareVectorLenght((double, double) originVector, string firstDirection, string secondDirection)
        {
            var firstVector = DirectionDict[firstDirection];
            var firstSumVector = (originVector.Item1 + firstVector.Item1, originVector.Item2 + firstVector.Item2);
            var firstSumLenght = CulcVectorLenght(firstSumVector);

            var secondVector = DirectionDict[secondDirection];
            var secondSumVector = (originVector.Item1 + secondVector.Item1, originVector.Item2 + secondVector.Item2);
            var secondSumLenght = CulcVectorLenght(secondSumVector);

            if (firstSumLenght > secondSumLenght)
            {
                return firstDirection;
            }
            else
            {
                return secondDirection;
            }
        }

        private string GetDirectionByVector((double, double) vector)
        {
            // Определенить четверть
            if (vector.Item1 >= 0 && vector.Item2 > 0)
            {
                // I четверть
                if (vector.Item1 > vector.Item2)
                {
                    // Верхняя половина
                    return CompareVectorLenght(vector, "С", "СВ");
                }
                else
                {
                    // Нижняя половина
                    return CompareVectorLenght(vector, "СВ", "В");
                }
            }
            else if (vector.Item1 > 0 && vector.Item2 <= 0)
            {
                // II четверть
                if (vector.Item1 < Math.Abs(vector.Item2))
                {
                    // Верхняя половина
                    return CompareVectorLenght(vector, "В", "ЮВ");
                }
                else
                {
                    // Нижняя половина
                    return CompareVectorLenght(vector, "ЮВ", "Ю");
                }
            }
            else if (vector.Item1 <= 0 && vector.Item2 < 0)
            {
                // III четверть
                if (vector.Item1 < vector.Item2)
                {
                    // Нижняя половина
                    return CompareVectorLenght(vector, "Ю", "ЮЗ");
                }
                else
                {
                    // Верхняя половина
                    return CompareVectorLenght(vector, "ЮЗ", "З");
                }
            }
            else
            {
                // IV четверть
                if (Math.Abs(vector.Item1) < vector.Item2)
                {
                    // Нижняя половина
                    return CompareVectorLenght(vector, "З", "СЗ");
                }
                else
                {
                    // Верхняя половина
                    return CompareVectorLenght(vector, "СЗ", "С");
                }
            }
        }

        public void FindWindDirection(string source, WeekWeather currentWeekWeather)
        {
            var commonKeyForParametr = "d_1\":";
            var beginKeys = new List<string>() { "\"vdId\":\"" };
            var endKey = '"';

            var directionParametrs = WeatherProvider.FindParametrs(source, commonKeyForParametr, beginKeys, endKey, dataAmount);
            for (var dayCount = 0; dayCount < dayInWeek; dayCount++)
            {
                (double, double) directionSum = (0, 0);
                for (var timestampCount = 0; timestampCount < timestampNumber; timestampCount++)
                {
                    var direction = directionParametrs[dayCount * timestampNumber + timestampCount];
                    directionSum.Item1 += DirectionDict[direction].Item1;
                    directionSum.Item2 += DirectionDict[direction].Item2;
                }
                (double, double) directionVector = (directionSum.Item1 / timestampNumber, directionSum.Item2 / timestampNumber);
                currentWeekWeather.SetWindDirection(GetDirectionByVector(directionVector), dayCount + 1, WeekWeather.TimeOfDay.Night);
            }
        }

        public void FindWindSpeed(string source, WeekWeather currentWeekWeather)
        {
            //var commonKeyForParametr = "widget-row-wind-speed\"";
            //var beginKeys = new List<string>() { "unit_wind_m_s\">\n", "unit_wind_m_s warning\">\n" };
            //var endKey = '\n';
            //var dataAmount = WeatherProvider.numberForecastDaysMax + 1;
            //var precipitationParametrs = WeatherProvider.FindParametrs(source, commonKeyForParametr, beginKeys, endKey, dataAmount);
            //for (int count = 0; count < precipitationParametrs.Count; count++)
            //{
            //    var speed = precipitationParametrs[count].Replace(" ", "");
            //    currentWeekWeather.SetWindSpeed(speed, count, false);
            //}
        }
    }
}

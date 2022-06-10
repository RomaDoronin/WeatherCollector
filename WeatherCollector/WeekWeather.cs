using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherCollector
{
    struct WeekWeather
    {
        public List<DayWeather> week;

        public WeekWeather()
        {
            week = new List<DayWeather>();
            var today = DateTime.Today;
            for (int i = 0; i < 7; i++)
            {
                var day = today.AddDays(i);
                DayWeather dayWeather = new DayWeather(day.Day, day.Month);
                
                week.Add(dayWeather);
            }
        }

        public void SetTemperature(int temperature, int dayIndex, bool isDay)
        {
            DayWeather day = week[dayIndex];
            if (isDay)
            {
                day.dayWeather.temperature = temperature;
            }
            else
            {
                day.nightWeather.temperature = temperature;
            }
            week[dayIndex] = day;
        }
    }

    struct DayWeather
    {
        public int day;
        public int month;
        public Weather nightWeather;
        public Weather dayWeather;

        public DayWeather(int _day, int _month)
        {
            day = _day;
            month = _month;
            nightWeather = new Weather();
            dayWeather = new Weather();
        }
    }

    struct Weather
    {
        public int? temperature;
        public double? precipitation; // Осадки
        public Wind wind;

        public Weather()
        {
            temperature = null;
            precipitation = null;
            wind = new Wind();
        }
    }

    struct Wind
    {
        public int? speed;
        public WindDirection? windDirection;

        public Wind()
        {
            speed = null;
            windDirection = null;
        }
    }

    enum WindDirection
    {
        North,
        NorthEast,
        East,
        SouthEast,
        South,
        SouthWest,
        West,
        NorthWest
    }
}

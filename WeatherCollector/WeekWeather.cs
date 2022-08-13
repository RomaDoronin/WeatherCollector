﻿namespace WeatherCollector
{
    public struct WeekWeather
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

        public void SetTemperature(string temperature, int dayIndex, bool isDay)
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

        public void SetPrecipitation(string precipitation, int dayIndex, bool isDay)
        {
            DayWeather day = week[dayIndex];
            if (isDay)
            {
                day.dayWeather.precipitation = precipitation;
            }
            else
            {
                day.nightWeather.precipitation = precipitation;
            }
            week[dayIndex] = day;
        }

        public void SetWindDirection(string windDirection, int dayIndex, bool isDay)
        {
            DayWeather day = week[dayIndex];
            if (isDay)
            {
                day.dayWeather.wind.direction = ParseStringToWindDirection(windDirection);
            }
            else
            {
                day.nightWeather.wind.direction = ParseStringToWindDirection(windDirection);
            }
            week[dayIndex] = day;
        }

        public void SetWindSpeed(string windSpeed, int dayIndex, bool isDay)
        {
            DayWeather day = week[dayIndex];
            if (isDay)
            {
                day.dayWeather.wind.speed = windSpeed;
            }
            else
            {
                day.nightWeather.wind.speed = windSpeed;
            }
            week[dayIndex] = day;
        }

        private static string ParseStringToWindDirection(string windDirection)
        {
            switch (windDirection)
            {
                case "С-З":
                    return "СЗ";
                case "Ю-З":
                    return "ЮЗ";
                case "Ю-В":
                    return "ЮВ";
                case "С-В":
                    return "СВ";
                default:
                    return windDirection;
            }
        }
    }

    public struct DayWeather
    {
        public int day;
        public int month;
        public Weather nightWeather;
        public Weather dayWeather;

        public DayWeather(int day, int month)
        {
            this.day = day;
            this.month = month;
            nightWeather = new Weather();
            dayWeather = new Weather();
        }
    }

    public struct Weather
    {
        public string? temperature;
        public string? precipitation; // Осадки
        public Wind wind;

        public Weather()
        {
            temperature = null;
            precipitation = null;
            wind = new Wind();
        }
    }

    public struct Wind
    {
        public string? speed;
        public string? direction;

        public Wind()
        {
            speed = null;
            direction = null;
        }

        public string? GetWindData()
        {
            if ((direction == null) || (speed == null))
            {
                return null;
            }
            return direction += ", " + speed;
        }
    }
}

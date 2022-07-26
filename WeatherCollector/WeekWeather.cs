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
                day.dayWeather.wind.windDirection = ParseStringToWindDirection(windDirection);
            }
            else
            {
                day.nightWeather.wind.windDirection = ParseStringToWindDirection(windDirection);
            }
            week[dayIndex] = day;
        }

        public void SetWindSpeed(int windSpeed, int dayIndex, bool isDay)
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

        private static WindDirection ParseStringToWindDirection(string windDirection)
        {
            switch (windDirection)
            {
                case "С":
                    return WindDirection.North;
                case "С-З":
                    return WindDirection.NorthWest;
                case "З":
                    return WindDirection.West;
                case "Ю-З":
                    return WindDirection.SouthWest;
                case "Ю":
                    return WindDirection.South;
                case "Ю-В":
                    return WindDirection.SouthEast;
                case "В":
                    return WindDirection.East;
                case "С-В":
                    return WindDirection.NorthEast;
                default:
                    return WindDirection.North;
            }
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

    struct Wind
    {
        public int? speed;
        public WindDirection? windDirection;

        public Wind()
        {
            speed = null;
            windDirection = null;
        }

        public string GetWindData()
        {
            var result = "";
            switch (windDirection)
            {
                case WindDirection.North:
                    result += "С";
                    break;
                case WindDirection.NorthEast:
                    result += "СВ";
                    break;
                case WindDirection.East:
                    result += "В";
                    break;
                case WindDirection.SouthEast:
                    result += "ЮВ";
                    break;
                case WindDirection.South:
                    result += "Ю";
                    break;
                case WindDirection.SouthWest:
                    result += "ЮЗ";
                    break;
                case WindDirection.West:
                    result += "З";
                    break;
                case WindDirection.NorthWest:
                    result += "СЗ";
                    break;
            }
            return result += ", " + speed;
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

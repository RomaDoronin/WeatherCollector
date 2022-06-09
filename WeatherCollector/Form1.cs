using System.Net;
using System.Text;

namespace WeatherCollector
{

    struct WeekWeather
    {
        public List<DayWeather> week;

        public WeekWeather()
        {
            week = new List<DayWeather>();
        }
    }

    struct DayWeather
    {
        public int day;
        public int month;
        public Weather nightWeather;
        public Weather dayWeather;

    }

    struct Weather
    {
        public int temperature;
        public double precipitation; // Осадки
        public Wind wind;
    }

    struct Wind
    {
        public int speed;
        public WindDirection windDirection;
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

    public partial class Form1 : Form
    {
        private WeekWeather weekWeather = new WeekWeather();

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SendRequestForWeather();
        }

        private void SendRequestForWeather()
        {
            String url = "https://meteoinfo.ru/forecasts5000/russia/nizhegorodskaya-area";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            PrintLog("Content length is" + response.ContentLength);
            PrintLog("Content type is" + response.ContentType);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                PrintLog("Всё норм.");

                // Get the stream associated with the response.
                Stream receiveStream = response.GetResponseStream();

                // Pipes the stream to a higher level stream reader with the required encoding format.
                StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);

                var str = readStream.ReadToEnd();
                ParseHtmlString(str);
                readStream.Close();
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                PrintLog("Такой страницы нет.");
            }
            response.Close();
        }

        private void PrintLog(string s)
        {
            outputLabel.Text += '\n';
            outputLabel.Text += s;
        }

        private int dayCount = 0;
        private DayWeather dayWeather = new DayWeather();
        private bool day = true;
        private bool temperatureIsFilled = false;

        private void ParseHtmlString(string str)
        {
            for (int i = 0; i < str.Length - 3; i++)
            {
                char str0 = str[i];
                char str1 = str[i + 1];
                char str2 = str[i + 2];
                char str3 = str[i + 3];

                if (str0 == '&' && str1 == 'd' && str2 == 'e' && str3 == 'g')
                {
                    if (!temperatureIsFilled)
                    {
                        FindTemperature(str, i);
                    }
                }
            }
            Console.WriteLine("Check");
        }

        private void FindTemperature(string source, int ampersandPosition)
        {
            int temperature = 0;
            int count = 1;
            char ch = source[ampersandPosition - count];

            while (ch != '>')
            {
                int mult = 10 * (count - 1);
                if (mult == 0)
                {
                    mult = 1;
                }
                temperature += (ch - '0') * mult;
                count++;
                ch = source[ampersandPosition - count];
            }

            if (day)
            {
                dayWeather.dayWeather.temperature = temperature;
                day = false;

                if (dayCount == 6)
                {
                    weekWeather.week.Add(dayWeather);
                    temperatureIsFilled = true;
                }
            }
            else
            {
                dayWeather.nightWeather.temperature = temperature;
                day = true;
                dayCount++;
                weekWeather.week.Add(dayWeather);
                dayWeather = new DayWeather();
            }
        }

    }
}
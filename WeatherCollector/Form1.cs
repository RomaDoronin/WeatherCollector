using System.Net;
using System.Text;

namespace WeatherCollector
{
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

        private bool precipitationIsFilled = false;

        private bool findDayPrecipitation = false;

        private void ParseHtmlString(string source)
        {
            FindTemperature(source);

            //for (int i = 0; i < source.Length - 3; i++)
            //{
            //    if (CollectSubstring("Осадки, мм (вероятность)", source, i))
            //    {
            //        if (!findDayPrecipitation)
            //        {
            //            findDayPrecipitation = true;
            //        }
            //    } else if (findDayPrecipitation)
            //    {
            //        FindPrecipitation(source, i);
            //    }
            //}
            Console.WriteLine("Check");
        }

        private void FindPrecipitation(string source, int index)
        {
            if (CollectSubstring("<nobr>", source, index))
            {
                int startIndex = index + 6;
                string stringPrecipitation = "";
                char ch = source[startIndex];
                int count = 0;
                while (ch != '<')
                {
                    stringPrecipitation += ch;
                    count++;
                    ch = source[startIndex + count];
                }

                double precipitation = Convert.ToDouble(stringPrecipitation);
            }
        }

        private bool CollectSubstring(string substring, string source, int index)
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

        private void FindTemperature(string source)
        {
            string key = "&deg";
            int dayCount = 0;
            bool isDay = true;

            for (int i = 0; i < source.Length - key.Length; i++)
            {
                if (CollectSubstring(key, source, i))
                {
                    int ampersandPosition = i;
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

                    if (isDay)
                    {
                        weekWeather.SetTemperature(temperature, dayCount, isDay);

                        if (dayCount == 6)
                        {
                            break;
                        }
                        isDay = false;
                    }
                    else
                    {
                        weekWeather.SetTemperature(temperature, dayCount, isDay);
                        dayCount++;
                        isDay = true;
                    }
                }
            }
        }
    }
}
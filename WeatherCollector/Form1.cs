using System.Net;
using System.Text;

namespace WeatherCollector
{
    enum FindState
    {
        NotFind,
        Finding,
        Found
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

        private void ParseHtmlString(string source)
        {
            FindTemperature(source);
            FindPrecipitation(source);
            Console.WriteLine("Check");
        }

        private void FindPrecipitation(string source)
        {
            var key = "Осадки, мм (вероятность)";
            var findDayPrecipitation = FindState.NotFind;
            var findNightPrecipitation = FindState.NotFind;
            int dayCount = 0;

            for (int i = 0; i < source.Length - key.Length; i++)
            {
                if (findDayPrecipitation == FindState.Finding || findNightPrecipitation == FindState.Finding)
                {
                    var nobrKey = "<nobr>";
                    if (CollectSubstring(nobrKey, source, i))
                    {
                        int startIndex = i + nobrKey.Length;
                        string stringPrecipitation = "";
                        char ch = source[startIndex];
                        int count = 0;
                        while (ch != '<' && ch != ')')
                        {
                            stringPrecipitation += ch;
                            count++;
                            ch = source[startIndex + count];
                        }

                        if (stringPrecipitation.EndsWith('%'))
                        {
                            var probability = stringPrecipitation; // !
                            continue;
                        }
                        else
                        {
                            double precipitation = Convert.ToDouble(stringPrecipitation.Replace('.', ','));
                            if (findDayPrecipitation == FindState.Finding)
                            {
                                weekWeather.SetPrecipitation(precipitation, dayCount, true);
                            }
                            else if (findNightPrecipitation == FindState.Finding)
                            {
                                weekWeather.SetPrecipitation(precipitation, dayCount, false);
                            }
                            if (dayCount == 6)
                            {
                                if (findDayPrecipitation == FindState.Finding)
                                {
                                    findDayPrecipitation = FindState.Found;
                                }
                                else if (findNightPrecipitation == FindState.Finding)
                                {
                                    findNightPrecipitation = FindState.Found;
                                }
                                dayCount = 0;
                                continue;
                            }
                            dayCount++;
                        }
                    }
                }
                else if (CollectSubstring(key, source, i))
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
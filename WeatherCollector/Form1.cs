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

        public delegate void MyDelegate(int value);
        public void DelegateMethod(int value)
        {
            this.progressBar.Value = value;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            SendRequestForWeather();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            CreateDoc();
        }

        private void CreateDoc()
        {
            CreateExcelDoc excelApp = new CreateExcelDoc();

            var temperatureRow = 4;
            excelApp.AddData(3, temperatureRow, "t°C");
            var precipitationRow = 5;
            excelApp.AddData(3, precipitationRow, "Осадки");
            var windRow = 6;
            excelApp.AddData(3, windRow, "Ветер");
            var colStart = 4;

            MergeNeededCell(excelApp);

            for (var dayCount = 0; dayCount < 5; dayCount++)
            {
                var currentDay = weekWeather.week[dayCount + 1];

                // Настройка заголовков
                var date = currentDay.month + "/" + currentDay.day;
                var dateCol = colStart + dayCount * 2;
                var dayCol = colStart + dayCount * 2;
                var nightCol = colStart + dayCount * 2 + 1;

                excelApp.AddData(dateCol, 2, date);

                excelApp.AddData(dayCol, 3, "День");
                excelApp.AddData(nightCol, 3, "Ночь");

                // Занесение данных о погоде
                excelApp.AddData(dayCol, temperatureRow, currentDay.dayWeather.temperature);
                excelApp.AddData(nightCol, temperatureRow, currentDay.nightWeather.temperature);

                excelApp.AddData(dayCol, precipitationRow, currentDay.dayWeather.precipitation);
                excelApp.AddData(nightCol, precipitationRow, currentDay.nightWeather.precipitation);

                excelApp.AddData(dayCol, windRow, currentDay.dayWeather.wind.GetWindData());
                excelApp.AddData(nightCol, windRow, currentDay.nightWeather.wind.GetWindData());
            }
        }

        private void MergeNeededCell(CreateExcelDoc excelApp)
        {
            excelApp.Merge("C2", "C3");
            excelApp.Merge("D2", "E2");
            excelApp.Merge("F2", "G2");
            excelApp.Merge("H2", "I2");
            excelApp.Merge("J2", "K2");
            excelApp.Merge("L2", "M2");
        }

        private void SendRequestForWeather()
        {
            String url = "https://meteoinfo.ru/forecasts5000/russia/nizhegorodskaya-area";
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

                var str = readStream.ReadToEnd();
                ParseHtmlString(str);
                readStream.Close();
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                Console.WriteLine("Такой страницы нет.");
            }
            response.Close();
        }

        private void ParseHtmlString(string source)
        {
            progressBar.BeginInvoke(new MyDelegate(DelegateMethod), 0);

            FindTemperature(source);

            progressBar.BeginInvoke(new MyDelegate(DelegateMethod), 25);

            FindPrecipitation(source);

            progressBar.BeginInvoke(new MyDelegate(DelegateMethod), 50);

            FindWindDirection(source);

            progressBar.BeginInvoke(new MyDelegate(DelegateMethod), 75);

            FindWindSpeed(source);

            progressBar.BeginInvoke(new MyDelegate(DelegateMethod), 100);

            button2.Enabled = true;

            Console.WriteLine("Check");
        }

        private void FindWindSpeed(string source)
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
                    if (CollectSubstring(nobrKey, source, i))
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
                        var windSpeed = Convert.ToInt16(stringWind);

                        if (findDayWind == FindState.Finding)
                        {
                            weekWeather.SetWindSpeed(windSpeed, dayCount, true);
                        }
                        else if (findNightWind == FindState.Finding)
                        {
                            weekWeather.SetWindSpeed(windSpeed, dayCount, false);
                        }
                        if (dayCount == 6)
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
                else if (CollectSubstring(key, source, i))
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

        private void FindWindDirection(string source)
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
                    if (CollectSubstring(titleKey, source, i))
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
                            weekWeather.SetWindDirection(stringWind, dayCount, true);
                        }
                        else if (findNightWind == FindState.Finding)
                        {
                            weekWeather.SetWindDirection(stringWind, dayCount, false);
                        }
                        if (dayCount == 6)
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
                else if (CollectSubstring(key, source, i))
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

        private void FindPrecipitation(string source)
        {
            var key = "Осадки, мм (вероятность)";
            int dayCount = 0;
            var findDayPrecipitation = FindState.NotFind;
            var findNightPrecipitation = FindState.NotFind;

            for (int i = 0; i < source.Length - key.Length; i++)
            {
                if (findDayPrecipitation == FindState.Finding || findNightPrecipitation == FindState.Finding)
                {
                    var nobrKey = "<nobr>";
                    if (CollectSubstring(nobrKey, source, i))
                    {
                        int startIndex = i + nobrKey.Length;
                        string precipitation = "";
                        char ch = source[startIndex];
                        int count = 0;
                        while (ch != '<' && ch != ')')
                        {
                            precipitation += ch;
                            count++;
                            ch = source[startIndex + count];
                        }

                        if (precipitation.EndsWith('%'))
                        {
                            var probability = precipitation; // !
                            continue;
                        }
                        else
                        {
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
                                dayCount = 1;
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

        private void FindTemperature(string source)
        {
            string key = "&deg";
            int dayCount = 0;
            bool isDay = true;
            var size = source.Length - key.Length;

            for (int i = 0; i < size; i++)
            {
                if (CollectSubstring(key, source, i))
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
                        weekWeather.SetTemperature(temperature, dayCount, isDay);
                        dayCount++;
                        isDay = false;
                    }
                    else
                    {
                        if (dayCount == 6)
                        {
                            break;
                        }
                        weekWeather.SetTemperature(temperature, dayCount, isDay);
                        isDay = true;
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
    }
}
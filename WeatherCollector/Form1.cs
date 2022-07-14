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
        static int numberForecastDays = 6;
        const int numberForecastDaysMax = 6;

        private List<String> stationList = new()
        {
            "sakunja",
            "vetluga",
            "krasnye-baki",
            "voskresenskoe",
            "volzskaja-gmo",
            "niznij-novgoro",
            "lyskovo",
            "arzamas",
            "pavlovo",
            "sergac",
            "vyksa",
            "lukojanov"
        };

        private readonly Dictionary<String, String> stationDict = new()
        {
            { "sakunja", "Шахунья" },
            { "vetluga", "Ветлуга" },
            { "krasnye-baki", "Красные баки" },
            { "voskresenskoe", "Воскресенское" },
            { "volzskaja-gmo", "Городец Волжская ГМО" },
            { "niznij-novgoro", "Нижний Новгород I" },
            { "lyskovo", "Лысково" },
            { "arzamas", "Арзамас" },
            { "pavlovo", "Павлово" },
            { "sergac", "Сергач" },
            { "vyksa", "Выкса" },
            { "lukojanov", "Лукоянов" }
        };

        private readonly Dictionary<int, String> monthNumberDict = new()
        {
            { 1, "января" },
            { 2, "февраля" },
            { 3, "марта" },
            { 4, "апреля" },
            { 5, "мая" },
            { 6, "июня" },
            { 7, "июля" },
            { 8, "августа" },
            { 9, "сентября" },
            { 10, "октября" },
            { 11, "ноября" },
            { 12, "декабря" }
        };

        private Dictionary<String, WeekWeather> weatherDict;

        private WeekWeather currentWeekWeather;

        public Form1()
        {
            InitializeComponent();
            SetupComboBox();

            weatherDict = new Dictionary<String, WeekWeather>();
        }

        private void SetupComboBox()
        {
            this.numberForecastDaysComboBox.Items.Add(3);
            this.numberForecastDaysComboBox.Items.Add(4);
            this.numberForecastDaysComboBox.Items.Add(5);
            this.numberForecastDaysComboBox.Items.Add(6);
            
            var index = this.numberForecastDaysComboBox.Items.IndexOf(numberForecastDays);
            this.numberForecastDaysComboBox.SelectedIndex = index;
        }

        public delegate void MyIntDelegate(int value);
        public delegate void MyBoolDelegate(bool value);
        public void DelegateMethod(int value)
        {
            this.progressBar.Value = value;
        }

        public void DelegateMethodButton2(bool value)
        {
            this.button2.Enabled = value;
        }

        private int progressCount = 0;

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            progressCount += 20;
            progressBar.BeginInvoke(new MyIntDelegate(DelegateMethod), progressCount);

            _ = GetDataFromStationsAsync();
        }

        void GetDataFromStations()
        {
            foreach (var station in stationList)
            {
                SendRequestForWeather(station);
            }
        }

        async Task GetDataFromStationsAsync()
        {
            await Task.Run(() => GetDataFromStations());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            UpdateNumberForecastDays();
            CreateDoc();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            MessageBox.Show("В случае возникновения вопросов обращаться по адресу:\n\nroman.doronin.sklexp@yandex.ru\n\nСсылка на исходный код:\n\nhttps://github.com/RomaDoronin/WeatherCollector", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void UpdateNumberForecastDays()
        {
            numberForecastDays = Int16.Parse(numberForecastDaysComboBox.SelectedItem.ToString());
        }

        private void CreateDoc()
        {
            CreateExcelDoc excelApp = new CreateExcelDoc();
            for (var stationCount = 0; stationCount < stationList.Count; stationCount++)
            {
                FillDoc(excelApp, stationCount);
            }
            MergeNeededCell(excelApp);
            BoldNeededCell(excelApp);
            SetColumnWidth(excelApp);
        }

        private void FillDoc(CreateExcelDoc excelApp, int stationCount)
        {
            var stationKey = stationList[stationCount];
            var weekWeather = weatherDict[stationKey];

            var temperatureRow = 4 + stationCount * 3;
            excelApp.AddData(3, temperatureRow, "Температура, °C");
            var precipitationRow = 5 + stationCount * 3;
            excelApp.AddData(3, precipitationRow, "Осадки, мм");
            var windRow = 6 + stationCount * 3;
            excelApp.AddData(3, windRow, "Ветер, м/с");
            var colStart = 4;

            excelApp.AddData(2, temperatureRow, stationDict[stationKey]);

            for (var dayCount = 0; dayCount < numberForecastDays; dayCount++)
            {
                var currentDay = weekWeather.week[dayCount + 1];

                var dayCol = colStart + dayCount * 2 + 1;
                var nightCol = colStart + dayCount * 2;
                if (stationCount == 0)
                {
                    // Настройка заголовков
                    var stringDay = currentDay.day.ToString();
                    stringDay = stringDay.Length == 1 ? '0' + stringDay : stringDay;
                    var date = stringDay + " " + monthNumberDict[currentDay.month];
                    var dateCol = colStart + dayCount * 2;
                    excelApp.AddData(dateCol, 2, date, HorizontalAlignment.Center);
                    excelApp.AddData(dayCol, 3, "День", HorizontalAlignment.Center);
                    excelApp.AddData(nightCol, 3, "Ночь", HorizontalAlignment.Center);
                }

                // Занесение данных о погоде
                excelApp.AddData(dayCol, temperatureRow, currentDay.dayWeather.temperature, HorizontalAlignment.Center);
                excelApp.AddData(nightCol, temperatureRow, currentDay.nightWeather.temperature, HorizontalAlignment.Center);

                excelApp.AddData(dayCol, precipitationRow, currentDay.dayWeather.precipitation, HorizontalAlignment.Center);
                excelApp.AddData(nightCol, precipitationRow, currentDay.nightWeather.precipitation, HorizontalAlignment.Center);

                excelApp.AddData(dayCol, windRow, currentDay.dayWeather.wind.GetWindData(), HorizontalAlignment.Center);
                excelApp.AddData(nightCol, windRow, currentDay.nightWeather.wind.GetWindData(), HorizontalAlignment.Center);
            }
        }

        private static void MergeNeededCell(CreateExcelDoc excelApp)
        {
            // Строки
            excelApp.Merge("D2", "E2");
            excelApp.Merge("F2", "G2");
            excelApp.Merge("H2", "I2");
            if (numberForecastDays > 3)
            {
                excelApp.Merge("J2", "K2");
            }
            if (numberForecastDays > 4)
            {
                excelApp.Merge("L2", "M2");
            }
            if (numberForecastDays > 5)
            {
                excelApp.Merge("N2", "O2");
            }

            // Столбцы
            excelApp.Merge("C2", "C3");
            excelApp.Merge("B2", "B3");
            for (int rowCount = 4; rowCount < 40; rowCount += 3)
            {
                excelApp.Merge("B" + rowCount.ToString(), "B" + (rowCount + 2).ToString());
            }
            excelApp.Merge("B4", "B6");
        }

        private void BoldNeededCell(CreateExcelDoc excelApp)
        {
            var mecricColumn = 3;
            var metricNumber = 3;
            var firstMecticRow = 5;
            for (var count = 0; count < stationList.Count; count++)
            {
                var boltRow = count * metricNumber + firstMecticRow;
                excelApp.EntireRowDoBold(boltRow, mecricColumn);
            }

            var dateRow = 2;
            var dayNightRow = 3;
            var firstDataColumn = 4;
            excelApp.EntireRowDoBold(dateRow, firstDataColumn);
            excelApp.EntireRowDoBold(dayNightRow, firstDataColumn);
        }

        private void SetColumnWidth(CreateExcelDoc excelApp)
        {
            excelApp.SetColumnWidth(2, 22);
            excelApp.SetColumnWidth(3, 15);
        }

        private void SendRequestForWeather(string station)
        {
            String url = "https://meteoinfo.ru/forecasts5000/russia/nizhegorodskaya-area/" + station;

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
                ParseHtmlString(str, station);
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
            currentWeekWeather = new WeekWeather();

            FindTemperature(source);
            FindPrecipitation(source);
            FindWindDirection(source);
            FindWindSpeed(source);

            weatherDict[station] = currentWeekWeather;

            progressCount += 10;
            progressBar.BeginInvoke(new MyIntDelegate(DelegateMethod), progressCount);
            if (progressCount == 140)
            {
                button2.BeginInvoke(new MyBoolDelegate(DelegateMethodButton2), true);
            }
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
                            currentWeekWeather.SetWindSpeed(windSpeed, dayCount, true);
                        }
                        else if (findNightWind == FindState.Finding)
                        {
                            currentWeekWeather.SetWindSpeed(windSpeed, dayCount, false);
                        }
                        if (dayCount == numberForecastDaysMax)
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
                            currentWeekWeather.SetWindDirection(stringWind, dayCount, true);
                        }
                        else if (findNightWind == FindState.Finding)
                        {
                            currentWeekWeather.SetWindDirection(stringWind, dayCount, false);
                        }
                        if (dayCount == numberForecastDaysMax)
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
                            if (ch == '.')
                            {
                                precipitation += ',';
                            } else
                            {
                                precipitation += ch;
                            }
                            count++;
                            ch = source[startIndex + count];
                        }

                        if (precipitation.EndsWith('%'))
                        {
                            continue;
                        }
                        else
                        {
                            if (findDayPrecipitation == FindState.Finding)
                            {
                                currentWeekWeather.SetPrecipitation(precipitation, dayCount, true);
                            }
                            else if (findNightPrecipitation == FindState.Finding)
                            {
                                currentWeekWeather.SetPrecipitation(precipitation, dayCount, false);
                            }
                            if (dayCount == numberForecastDaysMax)
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
                        currentWeekWeather.SetTemperature(temperature, dayCount, isDay);
                        dayCount++;
                        isDay = false;
                    }
                    else
                    {
                        if (dayCount == numberForecastDaysMax + 1)
                        {
                            break;
                        }
                        currentWeekWeather.SetTemperature(temperature, dayCount, isDay);
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

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
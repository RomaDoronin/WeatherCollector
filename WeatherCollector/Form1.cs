using System.Net;
using System.Text;
using WeatherCollector.WeatherDataSource;

namespace WeatherCollector
{
    enum FindState
    {
        NotFind,
        Finding,
        Found
    }

    public partial class Form1 : Form, IProgressBarInteraction
    {
        const int numberForecastDaysMax = 6;
        static int numberForecastDays = 6;

        private readonly Dictionary<String, String> stationDict = new()
        {
            { "sakunja", "�������" },
            { "vetluga", "�������" },
            { "krasnye-baki", "������� ����" },
            { "voskresenskoe", "�������������" },
            { "volzskaja-gmo", "������� �������� ���" },
            { "niznij-novgoro", "������ �������� I" },
            { "lyskovo", "�������" },
            { "arzamas", "�������" },
            { "pavlovo", "�������" },
            { "sergac", "������" },
            { "vyksa", "�����" },
            { "lukojanov", "��������" },

            { "shakhunya-4322", "�������" },
            { "nizhny-novgorod-4355", "������ ��������" },
            { "vyksa-4375", "�����" }
        };

        private readonly Dictionary<int, String> monthNumberDict = new()
        {
            { 1, "������" },
            { 2, "�������" },
            { 3, "�����" },
            { 4, "������" },
            { 5, "���" },
            { 6, "����" },
            { 7, "����" },
            { 8, "�������" },
            { 9, "��������" },
            { 10, "�������" },
            { 11, "������" },
            { 12, "�������" }
        };

        private Dictionary<String, WeekWeather> weatherDict;
        private GismeteoWeather gismeteoWeather;
        private GidroMCWeather gidroMCWeather;
        private WeatherProvider weatherProvider;

        private WeekWeather currentWeekWeather;

        public Form1()
        {
            InitializeComponent();
            SetupComboBox();

            weatherDict = new Dictionary<String, WeekWeather>();
            gismeteoWeather = new GismeteoWeather();
            gidroMCWeather = new GidroMCWeather();
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

        public void DelegateMethodcreateDocButton(bool value)
        {
            this.createDocButton.Enabled = value;
        }

        private int progressCount = 0;

        private void downloadWeatherButton_Click(object sender, EventArgs e)
        {
            downloadWeatherButton.Enabled = false;
            progressCount += 20;
            progressBar.BeginInvoke(new MyIntDelegate(DelegateMethod), progressCount);

            _ = GetDataFromStationsAsync();
        }

        void GetDataFromStations()
        {
            //weatherProvider = new WeatherProvider(gismeteoWeather, this);
            //weatherProvider.GetDataFromStations();

            weatherProvider = new WeatherProvider(gidroMCWeather, this);
            weatherProvider.GetDataFromStations();
        }

        public void IncrementProgressCount(int value, string url)
        {
            progressCount += 10;
            progressBar.BeginInvoke(new MyIntDelegate(DelegateMethod), progressCount);
            if (progressCount == 20 + value * 10)
            {
                createDocButton.BeginInvoke(new MyBoolDelegate(DelegateMethodcreateDocButton), true);
            }
        }

        async Task GetDataFromStationsAsync()
        {
            await Task.Run(() => GetDataFromStations());
        }

        private void createDocButton_Click(object sender, EventArgs e)
        {
            UpdateNumberForecastDays();
            CreateDoc();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            MessageBox.Show("� ������ ������������� �������� ���������� �� ������:\n\nroman.doronin.sklexp@yandex.ru\n\n������ �� �������� ���:\n\nhttps://github.com/RomaDoronin/WeatherCollector", "����������", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void UpdateNumberForecastDays()
        {
            numberForecastDays = Int16.Parse(numberForecastDaysComboBox.SelectedItem.ToString());
        }

        private void CreateDoc()
        {
            CreateExcelDoc excelApp = new CreateExcelDoc();
            var stationList = weatherProvider.GetStationList();
            for (var stationCount = 0; stationCount < stationList.Count; stationCount++)
            {
                var stationKey = stationList[stationCount];
                FillDoc(excelApp, stationCount, stationKey);
            }
            MergeNeededCell(excelApp);
            BoldNeededCell(excelApp);
            SetColumnWidth(excelApp);
        }

        private void FillDoc(CreateExcelDoc excelApp, int stationCount, string stationKey)
        {
            var weekWeather = weatherProvider.weatherDict[stationKey];

            var temperatureRow = 4 + stationCount * 3;
            excelApp.AddData(3, temperatureRow, "�����������, �C");
            var precipitationRow = 5 + stationCount * 3;
            excelApp.AddData(3, precipitationRow, "������, ��");
            var windRow = 6 + stationCount * 3;
            excelApp.AddData(3, windRow, "�����, �/�");
            var colStart = 4;

            excelApp.AddData(2, temperatureRow, stationDict[stationKey]);

            for (var dayCount = 0; dayCount < numberForecastDays; dayCount++)
            {
                var currentDay = weekWeather.week[dayCount + 1];

                var dayCol = colStart + dayCount * 2 + 1;
                var nightCol = colStart + dayCount * 2;
                if (stationCount == 0)
                {
                    // ��������� ����������
                    var stringDay = currentDay.day.ToString();
                    stringDay = stringDay.Length == 1 ? '0' + stringDay : stringDay;
                    var date = stringDay + " " + monthNumberDict[currentDay.month];
                    var dateCol = colStart + dayCount * 2;
                    excelApp.AddData(dateCol, 2, date, HorizontalAlignment.Center);
                    excelApp.AddData(dayCol, 3, "����", HorizontalAlignment.Center);
                    excelApp.AddData(nightCol, 3, "����", HorizontalAlignment.Center);
                }

                // ��������� ������ � ������
                excelApp.AddData(dayCol, temperatureRow, currentDay.dayWeather.temperature, HorizontalAlignment.Center);
                excelApp.AddData(nightCol, temperatureRow, currentDay.nightWeather.temperature, HorizontalAlignment.Center);

                if (currentDay.dayWeather.precipitation != null)
                {
                    excelApp.AddData(dayCol, precipitationRow, currentDay.dayWeather.precipitation, HorizontalAlignment.Center);
                }
                if (currentDay.nightWeather.precipitation != null)
                {
                    excelApp.AddData(nightCol, precipitationRow, currentDay.nightWeather.precipitation, HorizontalAlignment.Center);
                }

                var dayWind = currentDay.dayWeather.wind.GetWindData();
                if (dayWind != null)
                {
                    excelApp.AddData(dayCol, windRow, dayWind, HorizontalAlignment.Center);
                }
                var nightWind = currentDay.nightWeather.wind.GetWindData();
                if (nightWind != null)
                {
                    excelApp.AddData(nightCol, windRow, nightWind, HorizontalAlignment.Center);
                }
            }
        }

        private void MergeNeededCell(CreateExcelDoc excelApp)
        {
            // ������
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

            // �������
            excelApp.Merge("C2", "C3");
            excelApp.Merge("B2", "B3");
            var stationNumber = weatherProvider.GetStationList().Count;
            var rowSize = 4 + stationNumber * 3;
            for (int rowCount = 4; rowCount < rowSize; rowCount += 3)
            {
                excelApp.Merge("B" + rowCount.ToString(), "B" + (rowCount + 2).ToString());
            }
            excelApp.Merge("B4", "B6");

            // ������ �� ����
            if (!weatherProvider.IsDivideDayNight())
            {
                for (int countI = 0; countI < 3; countI++)
                {
                    var startRowList = new List<int>() { 5, 6 };
                    foreach (var startRow in startRowList)
                    {
                        var rowCount = startRow + countI * 3;
                        for (int countJ = 0; countJ < numberForecastDays; countJ++)
                        {
                            // (char)65 == A, 68 == D
                            var colCountFirst = (char)(68 + countJ * 2);
                            var colCountSecond = (char)(69 + countJ * 2);
                            excelApp.Merge(colCountFirst + rowCount.ToString(), colCountSecond + rowCount.ToString());
                        }
                    }
                }
            }
        }

        private void BoldNeededCell(CreateExcelDoc excelApp)
        {
            var mecricColumn = 3;
            var metricNumber = 3;
            var firstMecticRow = 5;
            var stationNumber = weatherProvider.GetStationList().Count;
            for (var count = 0; count < stationNumber; count++)
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
    }
}
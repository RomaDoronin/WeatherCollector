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
        private static int numberForecastDays = 6;
        private static int progressBarStartOffset = 20;
        private static int progressBarStep = 10;

        private readonly Dictionary<string, string> stationDict = new()
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
            { "lukojanov", "Лукоянов" },

            { "shakhunya-4322", "Шахунья" },
            { "nizhny-novgorod-4355", "Нижний Новгород" },
            { "vyksa-4375", "Выкса" },

            { "shakhunya", "Шахунья" },
            { "nizhny-novgorod", "Нижний Новгород" },
            //{ "vyksa", "Выкса" }

            { "Shakhun'ya", "Шахунья" },
            { "Nizhny%20Novgorod", "Нижний Новгород" },
            { "Vyksa", "Выкса" }
        };

        private readonly Dictionary<int, string> monthNumberDict = new()
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

        private GismeteoWeather gismeteoWeather;
        private GidroMCWeather gidroMCWeather;
        private VentuskyWeather ventuskyWeather;
        private YrWeather yrWeather;

        private WeatherProvider weatherProvider;

        public Form1()
        {
            InitializeComponent();
            SetupComboBox();

            Logs.ClearAll();
            Logs.WriteMetaData();
        }

        private void SetupComboBox()
        {
            numberForecastDaysComboBox.Items.Add(3);
            numberForecastDaysComboBox.Items.Add(4);
            numberForecastDaysComboBox.Items.Add(5);
            numberForecastDaysComboBox.Items.Add(6);
            
            var index = numberForecastDaysComboBox.Items.IndexOf(numberForecastDays);
            numberForecastDaysComboBox.SelectedIndex = index;
        }

        public delegate void MyIntDelegate(int value);
        public delegate void MyBoolDelegate(bool value);
        public void DelegateMethod(int value)
        {
            progressBar.Value = value;
        }

        public void DelegateMethodcreateDocButton(bool value)
        {
            createDocButton.Enabled = value;
        }

        private int progressCount = 0;

        private void downloadWeatherButton_Click(object sender, EventArgs e)
        {
            Logs.WriteLine("downloadWeatherButton_Click");

            dataSourceGroupBox.Enabled = false;
            InitWeatherProviders();
            SetprogressBarMaximumValue();
            downloadWeatherButton.Enabled = false;

            progressCount += progressBarStartOffset;
            progressBar.BeginInvoke(new MyIntDelegate(DelegateMethod), progressCount);

            _ = GetDataFromStationsAsync();
        }

        private void InitWeatherProviders()
        {
            if (gismeteoCheckBox.Checked)
            {
                gismeteoWeather = new GismeteoWeather();
            }
            if (gidroMCCheckBox.Checked)
            {
                gidroMCWeather = new GidroMCWeather();
            }
            if (ventuskyCheckBox.Checked)
            {
                ventuskyWeather = new VentuskyWeather();
            }
            if (yrCheckBox.Checked)
            {
                yrWeather = new YrWeather();
            }
        }

        private void SetprogressBarMaximumValue()
        { 
            var allStationCount =
                (gismeteoCheckBox.Checked ? gismeteoWeather.StationList.Count : 0)
                + (gidroMCCheckBox.Checked ? gidroMCWeather.StationList.Count : 0)
                + (ventuskyCheckBox.Checked ? ventuskyWeather.StationList.Count : 0)
                + (yrCheckBox.Checked ? (yrWeather.StationList.Count * 8) : 0);
            progressBar.Maximum = progressBarStartOffset + (allStationCount) * progressBarStep;
        }

        private Dictionary<string, WeekWeather> gidroMCoWeatherDict;
        private Dictionary<string, WeekWeather> gismeteoWeatherDict;
        private Dictionary<string, WeekWeather> ventuskyWeatherDict;
        private Dictionary<string, WeekWeather> yrWeatherDict;

        void GetDataFromStations()
        {
            if (gismeteoCheckBox.Checked)
            {
                Logs.WriteLine("gismeteoWeather start");
                weatherProvider = new WeatherProvider(gismeteoWeather, this);
                weatherProvider.GetDataFromStations();
                gismeteoWeatherDict = weatherProvider.weatherDict;
            }
            if (gidroMCCheckBox.Checked)
            {
                Logs.WriteLine("gidroMCWeather start");
                weatherProvider = new WeatherProvider(gidroMCWeather, this);
                weatherProvider.GetDataFromStations();
                gidroMCoWeatherDict = weatherProvider.weatherDict;
            }
            if (ventuskyCheckBox.Checked)
            {
                Logs.WriteLine("ventuskyWeather start");
                weatherProvider = new WeatherProvider(ventuskyWeather, this);
                weatherProvider.GetDataFromStations();
                ventuskyWeatherDict = weatherProvider.weatherDict;
            }
            if (yrCheckBox.Checked)
            {
                Logs.WriteLine("yrWeather start");
                weatherProvider = new WeatherProvider(yrWeather, this);
                weatherProvider.GetDataFromStations();
                yrWeatherDict = weatherProvider.weatherDict;

                Logs.WriteLine("yrWeatherDict start");
                var yrWeatherWindDirection = new YrWeatherWindDirection(yrWeather, yrWeatherDict, this);
                yrWeatherWindDirection.GetDataFromStations();
            }
        }

        public void IncrementProgressCount()
        {
            progressCount += progressBarStep;
            progressBar.BeginInvoke(new MyIntDelegate(DelegateMethod), progressCount);

            Logs.WriteLine(progressCount.ToString() + " / " + progressBar.Maximum.ToString());

            if (progressCount == progressBar.Maximum)
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
            Logs.WriteLine("createDocButton_Click");

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
            var currentCol = 2;
            var stepBetweenTabels = 2;
            if (gidroMCCheckBox.Checked)
            {
                currentCol = CreateTabel(excelApp, currentCol, gidroMCoWeatherDict, gidroMCWeather, "Гидрометцентр") + stepBetweenTabels;
            }
            if (gismeteoCheckBox.Checked)
            {
                currentCol = CreateTabel(excelApp, currentCol, gismeteoWeatherDict, gismeteoWeather, "Gismeteo") + stepBetweenTabels;
            }
            if (ventuskyCheckBox.Checked)
            {
                currentCol = CreateTabel(excelApp, currentCol, ventuskyWeatherDict, ventuskyWeather, "Ventusky") + stepBetweenTabels;
            }
            if (yrCheckBox.Checked)
            {
                _ = CreateTabel(excelApp, currentCol, yrWeatherDict, yrWeather, "Yr") + stepBetweenTabels;
            }
        }

        private int CreateTabel(CreateExcelDoc excelApp, int startedRow, Dictionary<string, WeekWeather> weatherDict, IWeatherDataSource dataSource, string sourceName)
        {
            FillTableHeader(excelApp, startedRow, sourceName);

            var stationNumber = dataSource.StationList.Count;
            for (var stationCount = 0; stationCount < stationNumber; stationCount++)
            {
                var stationKey = dataSource.StationList[stationCount];
                var localizedStataion = stationDict[stationKey];
                var weekWeather = weatherDict[stationKey];
                FillDoc(excelApp, startedRow, stationCount, localizedStataion, weekWeather);
            }
            MergeNeededCell(excelApp, startedRow, stationNumber, dataSource.IsDivideDayNight);
            BoldNeededCell(excelApp, startedRow, stationNumber);
            SetColumnWidth(excelApp);

            return startedRow + 2 + stationNumber * 3;
        }

        private static void FillTableHeader(CreateExcelDoc excelApp, int startedRow, string sourceName)
        {
            var sourceRow = startedRow - 1;
            excelApp.AddData(2, sourceRow, sourceName, HorizontalAlignment.Center);

            var territoryRow = startedRow;
            excelApp.AddData(2, territoryRow, "Территория области", HorizontalAlignment.Center);

            var parametersRow = startedRow;
            excelApp.AddData(3, parametersRow, "Параметры", HorizontalAlignment.Center);
            excelApp.EntireRowDoBold(startedRow, 3);
        }

        private void FillDoc(CreateExcelDoc excelApp, int startedRow, int stationCount, string localizedStataion, WeekWeather weekWeather)
        {
            int temperatureRow, precipitationRow, windRow;
            FillParametrHeader(excelApp, startedRow, stationCount, out temperatureRow, out precipitationRow, out windRow, weekWeather.PrecipitationHeader);

            var colStart = 4;

            excelApp.AddData(2, temperatureRow, localizedStataion);

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
                    excelApp.AddData(dateCol, startedRow, date, HorizontalAlignment.Center);
                    excelApp.AddData(dayCol, startedRow + 1, "День", HorizontalAlignment.Center);
                    excelApp.AddData(nightCol, startedRow + 1, "Ночь", HorizontalAlignment.Center);
                }

                // Занесение данных о погоде
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

        private static void FillParametrHeader(CreateExcelDoc excelApp, int startedRow, int stationCount, out int temperatureRow, out int precipitationRow, out int windRow, string precipitationHeader)
        {
            temperatureRow = startedRow + 2 + stationCount * 3;
            excelApp.AddData(3, temperatureRow, "Температура, °C");

            precipitationRow = startedRow + 3 + stationCount * 3;
            //excelApp.AddData(3, precipitationRow, "Осадки, мм");
            excelApp.AddData(3, precipitationRow, precipitationHeader);

            windRow = startedRow + 4 + stationCount * 3;
            excelApp.AddData(3, windRow, "Ветер, м/с");
        }

        private void MergeNeededCell(CreateExcelDoc excelApp, int startedRow, int stationNumber, bool isDivideDayNight)
        {
            for (var dayCount = 0; dayCount < numberForecastDays; dayCount++)
            {
                var firstColNumber = 68;
                var firstCol = (char)(firstColNumber + dayCount * 2);
                var secondCol = (char)(firstColNumber + dayCount * 2 + 1);
                excelApp.Merge(firstCol + startedRow.ToString(), secondCol + startedRow.ToString());
            }

            // Столбцы
            excelApp.Merge("B" + startedRow.ToString(), "B" + (startedRow + 1).ToString());
            excelApp.Merge("C" + startedRow.ToString(), "C" + (startedRow + 1).ToString());
            var rowSize = startedRow + 2 + stationNumber * 3;
            for (int rowCount = startedRow + 2; rowCount < rowSize; rowCount += 3)
            {
                excelApp.Merge("B" + rowCount.ToString(), "B" + (rowCount + 2).ToString());
            }

            // Строки по дням
            if (!isDivideDayNight)
            {
                for (int countI = 0; countI < 3; countI++)
                {
                    var startRowList = new List<int>() { startedRow + 3, startedRow + 4 };
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

            // Название источника
            var sourceNameRow = startedRow - 1;
            excelApp.Merge("B" + sourceNameRow.ToString(), "O" + sourceNameRow.ToString());
        }

        private void BoldNeededCell(CreateExcelDoc excelApp, int startedRow, int stationNumber)
        {
            var mecricColumn = 3;
            var metricNumber = 3;
            var firstMecticRow = startedRow + 3;
            for (var count = 0; count < stationNumber; count++)
            {
                var boltRow = count * metricNumber + firstMecticRow;
                excelApp.EntireRowDoBold(boltRow, mecricColumn);
            }

            var dayNightRow = startedRow + 1;
            var firstDataColumn = 2;
            excelApp.EntireRowDoBold(dayNightRow, firstDataColumn);
        }

        private void SetColumnWidth(CreateExcelDoc excelApp)
        {
            excelApp.SetColumnWidth(2, 22);
            excelApp.SetColumnWidth(3, 19);
        }
    }
}
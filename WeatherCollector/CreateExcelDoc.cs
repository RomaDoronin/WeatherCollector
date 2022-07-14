using Excel = Microsoft.Office.Interop.Excel;

namespace WeatherCollector
{
    public class CreateExcelDoc
    {
        private Excel.Application app;
        private Excel.Workbook workbook;
        private Excel.Worksheet worksheet;
        private Excel.Range workSheet_range; 

        public CreateExcelDoc()
        {
            CreateDoc();
        }

        public void CreateDoc()
        {
            try
            {
                app = new Excel.Application();
                app.Visible = true;
                workbook = app.Workbooks.Add(1);
                worksheet = (Excel.Worksheet)workbook.Sheets[1];
                worksheet.Cells.NumberFormat = "@";
            }
            catch (Exception)
            {
                Console.Write("Error");
            }
        }

        public void AddData(int col, int row, string data)
        {
            try
            {
                worksheet.Cells[row, col] = data;
            }
            catch (Exception)
            {
                Console.Write("Error");
            }
        }

        public void Merge(string cell1, string cell2)
        {
            var firstCell = ParseStringCell(cell1);
            var secondCell = ParseStringCell(cell2);

            worksheet.Range[worksheet.Cells[firstCell.Item1, firstCell.Item2], worksheet.Cells[secondCell.Item1, secondCell.Item2]].Merge();
        }

        public void EntireRowDoBold(int row, int column)
        {
            //worksheet.get_Range("C5", "C5").Cells.Style.Bold = true;
            worksheet.Cells[row, column].EntireRow.Font.Bold = true;
        }

        private (int, int) ParseStringCell(string cell)
        {
            var letter = cell[0];
            int columnNumber = letter - 'A' + 1;
            var number = cell[1..];
            int rowNumber = Int32.Parse(number);
            return (rowNumber, columnNumber);
        }
    }
}

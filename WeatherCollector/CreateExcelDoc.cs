using Excel = Microsoft.Office.Interop.Excel;

namespace WeatherCollector
{
    public enum HorizontalAlignment
    {
        Left,
        Center
    }

    public class CreateExcelDoc
    {
        private Excel.Application app;
        private Excel.Workbook workbook;
        private Excel.Worksheet worksheet;

        private static Excel.XlHAlign GetExcelHorizontalAlignment(HorizontalAlignment align)
        {
            if (align == HorizontalAlignment.Center)
            {
                return Excel.XlHAlign.xlHAlignCenter;
            } else
            {
                return Excel.XlHAlign.xlHAlignLeft;
            }
        }

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

        public void AddData(int col, int row, string data, HorizontalAlignment horizontalAlignment = HorizontalAlignment.Left)
        {
            try
            {
                worksheet.Cells[row, col] = data;
                worksheet.Cells[row, col].HorizontalAlignment = GetExcelHorizontalAlignment(horizontalAlignment);
                worksheet.Cells[row, col].VerticalAlignment = Excel.XlHAlign.xlHAlignGeneral;
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
            worksheet.Cells[row, column].EntireRow.Font.Bold = true;
        }

        public void SetColumnWidth(int column, int width)
        {
            worksheet.Columns[column].ColumnWidth = width;
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

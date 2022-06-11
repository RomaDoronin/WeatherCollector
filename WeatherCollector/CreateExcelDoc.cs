using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;

namespace WeatherCollector
{
    public class CreateExcelDoc
    {
        private Excel.Application app = null;
        private Excel.Workbook workbook = null;
        private Excel.Worksheet worksheet = null;
        private Excel.Range workSheet_range = null;

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
            catch (Exception e)
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
            catch (Exception e)
            {
                Console.Write("Error");
            }
        }

        public void Merge(string cell1, string cell2)
        {
            workSheet_range = worksheet.get_Range(cell1, cell2);
            workSheet_range.Merge(2);
        }
    }
}

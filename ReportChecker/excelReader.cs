using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;



namespace ReportChecker
{
    class ExcelReader
    {

        Excel.Application xlApp;                        
       
        public ExcelReader(string path, string filename)
        {
            _contents = new List<string>();
            xlApp = new Excel.Application();
            xlWorkbook = xlApp.Workbooks.Open(path+filename);
            SetPage(1);
        }

        private void AddResults(string input)
        {
            _contents.Add(input);
        }

        public List<string> GetAllResults()
        {
            return _contents;
        }

        public int GetResultsCount()
        {
            return _contents.Count;
        }
        public string GetResults(int lineNumber)
        {
            if (lineNumber < 0 || lineNumber >= GetResultsCount()) { return ""; }
            
                return _contents[lineNumber];                                    
        }

        public void ReadExcel()
        {

            for (int sheetNo = xlWorkbook.Sheets.Count ; sheetNo >= 1; sheetNo--)
            {
                SetPage(sheetNo);
                int xRows = xlRange.Rows.Count;
                int xCols = xlRange.Columns.Count;
                string output = "";

                for (int row = 1; row <= xRows; row++)
                {
                    output += GetSheetName(xlWorkbook.Sheets[sheetNo]);                    

                    for (int col = 1; col <= xCols; col++)
                    {
                        string cellContents = GetCell(row, col);
                        output += "\t" + cellContents; 
                    }
                    AddResults(output);
                    output = "";
                }

            }

        }


        public Excel.Range xlRange { get; set; }

        public Excel.Workbook xlWorkbook { get; set; }

        public Excel._Worksheet xlWorksheet { get; set; }

        public void SetPage(int sheetNo)
        {
            xlWorksheet = xlWorkbook.Sheets[sheetNo];
            xlRange = xlWorksheet.UsedRange;
        }

        public string GetCell(int row, int col)
        {
            if (xlRange.Cells[row, col] != null && xlRange.Cells[row, col].Value2 != null)
            {
                return xlRange.Cells[row, col].Value2.ToString();
            }
            else

            {
                return "";
            }
                

        }

        private string GetSheetName(Excel._Worksheet sheet)
        {
            return sheet.Name;
        }

        ~ExcelReader()
        {
            //You have to release references to these objects manually otherwise file access writes to the workbook will persist.

            //cleanup
            GC.Collect();
            GC.WaitForPendingFinalizers();

            //release com objects
            System.Runtime.InteropServices.Marshal.ReleaseComObject(xlRange);
            Marshal.ReleaseComObject(xlWorksheet);

            //close and release
            xlWorkbook.Close();
            Marshal.ReleaseComObject(xlWorkbook);

            //quit and release
            xlApp.Quit();
            Marshal.ReleaseComObject(xlApp);

        }

        List<string> _contents;
    }


}

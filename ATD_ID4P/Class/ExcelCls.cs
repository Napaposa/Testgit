using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Office.Interop.Excel;


// Microsoft.Office.Interop.Excel.Range is a type that represents a range of cells in an Excel spreadsheet
// System.Range is a type that represents a range of elements in a collection
namespace ATD_ID4P.Class
{
    public class ExcelCls
    {
        private Microsoft.Office.Interop.Excel.Application Excel;

        public void Init()
        {
            CreateNewExcel();
            CreateNewWorkBook();
        }

        public Microsoft.Office.Interop.Excel.Application CreateNewExcel(bool IsVisible = false)
        {
            if (Excel == null)
            {
                Excel = new Microsoft.Office.Interop.Excel.Application();
                Excel.Visible = IsVisible;
                Excel.DisplayAlerts = false;
            }

            return Excel;
        }

        public Workbook CreateNewWorkBook()
        {
            Workbook Wb = null;
            if (Excel != null)
            {
                if (Excel.Workbooks == null || Excel.Workbooks.Count == 0)
                {
                    Wb = (Workbook)Excel.Workbooks.Add(Type.Missing);
                }
            }
            return Wb;
        }

        public Worksheet CreateNewWorkSheet(string SheetName = "")
        {
            Worksheet ws = null;

            if (Excel != null)
            {
                Workbook wb = Excel.Workbooks.Item[0];
                if (wb != null)
                {
                    ws = (Worksheet)wb.Worksheets.Add();
                    if (!String.IsNullOrEmpty(SheetName))
                    {
                        ws.Name = SheetName;
                    }
                }
            }

            return ws;
        }

        public void SaveFile(string SavePath = "", string FileName = "")
        {
            if (Excel == null)
                return;

            if (String.IsNullOrEmpty(SavePath))
            {
                SaveFileDialog SFD = new SaveFileDialog();
                SFD.RestoreDirectory = true;
                SFD.Filter = "Excel 2010|*.xlsx|Excel|*.xls";
                if (!string.IsNullOrEmpty(FileName))
                    SFD.FileName = FileName;

                if (SFD.ShowDialog() == DialogResult.Cancel)
                    return;

                SavePath = SFD.FileName;
            }

            if (Excel.ActiveWorkbook.FullName == SavePath)
            {
                Excel.ActiveWorkbook.Save();
                Excel.ActiveWorkbook.Close();
            }
            else
            {
                Excel.ActiveWorkbook.SaveAs(SavePath);
                Excel.ActiveWorkbook.Close();
            }
        }

        public Workbook OpenWorkBook(string FilePath = "", bool IsReadOnly = true)
        {
            Workbook Wb = null;

            if (Excel == null)
                Init();

            if (String.IsNullOrEmpty(FilePath))
            {
                OpenFileDialog OPF = new OpenFileDialog();
                OPF.Filter = "Excel 2010|*.xlsx|Excel|*.xls";
                if (OPF.ShowDialog() == DialogResult.Cancel)
                {
                    return Wb;
                }
                FilePath = OPF.FileName;
            }

            Excel.Workbooks.Open(FilePath, null, IsReadOnly);
            Wb = Excel.ActiveWorkbook;

            return Wb;
        }

        public Worksheet FindWorkSheet(string SheetName)
        {
            Worksheet Ws = null;
            if (Excel != null)
            {
                if (Excel.ActiveWorkbook != null)
                {
                    foreach (Worksheet _ws in Excel.ActiveWorkbook.Worksheets)
                    {
                        if (_ws.Name == SheetName)
                        {
                            Ws = _ws;
                            break;
                        }
                    }
                }
            }
            return Ws;
        }

        public void WriteCell(object Value, int ROW, int COL, string SheetName = "")
        {
            if (Excel != null)
            {
                Worksheet ws = (Worksheet)Excel.ActiveWorkbook.ActiveSheet;
                if (!String.IsNullOrEmpty(SheetName))
                {
                    ws = FindWorkSheet(SheetName);
                }

                if (ws != null)
                {
                    ws.Cells[ROW, COL] = Value;
                }
            }
        }

        public object ReadCell(int ROW, int COL, string SheetName = "")
        {
            object Value = null;
            if (Excel != null)
            {
                Worksheet ws = (Worksheet)Excel.ActiveWorkbook.ActiveSheet;
                if (!String.IsNullOrEmpty(SheetName))
                {
                    ws = FindWorkSheet(SheetName);
                }

                if (ws != null)
                {
                    Value = ws.Cells[ROW, COL];
                }
            }
            return Value;
        }

        public Microsoft.Office.Interop.Excel.Range FindCell(Worksheet ws, string FindKey)
        {
            Microsoft.Office.Interop.Excel.Range Cells = null;
            Microsoft.Office.Interop.Excel.Range UsedRange = ws.UsedRange;
            Cells = UsedRange.Find(FindKey);
            return Cells;
        }

        public System.Data.DataTable WorkSheetToDataTable(string HeaderKey = "", string SheetName = "")
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            Worksheet ws = (Worksheet)Excel.ActiveWorkbook.ActiveSheet;
            if (!string.IsNullOrEmpty(SheetName))
                ws = FindWorkSheet(SheetName);

            if (ws != null)
            {
                Microsoft.Office.Interop.Excel.Range UsedRange = ws.UsedRange;
                if (!string.IsNullOrEmpty(HeaderKey))
                {
                    Microsoft.Office.Interop.Excel.Range HeaderCell = FindCell(ws, HeaderKey);
                    Microsoft.Office.Interop.Excel.Range LastCell = (Microsoft.Office.Interop.Excel.Range)ws.Cells[UsedRange.Rows.Count, UsedRange.Columns.Count];

                    UsedRange = ws.Range[HeaderCell, LastCell];
                }

                //--Create Column--
                for (int r = 1; r <= UsedRange.Rows.Count; r++)
                {
                    //int rIndex = UsedRange.Rows[r].Row;
                    Range buf_temp = UsedRange.Rows[r] as Microsoft.Office.Interop.Excel.Range;
                    int rIndex = buf_temp.Row;
                    System.Data.DataRow _Row = null;
                    if (r > 1)
                        _Row = dt.Rows.Add();

                    for (int c = 1; c <= UsedRange.Columns.Count; c++)
                    {
                        buf_temp = UsedRange.Columns[c] as Microsoft.Office.Interop.Excel.Range;
                        int cIndex = buf_temp.Column;
                        //--Create Column--
                        if (r == 1)
                        {
                            buf_temp = ws.Cells[rIndex, cIndex] as Microsoft.Office.Interop.Excel.Range;
                            dt.Columns.Add(buf_temp.Value.ToString());
                        }
                        else
                        {
                            buf_temp = ws.Cells[rIndex, cIndex] as Microsoft.Office.Interop.Excel.Range;
                            _Row[c - 1] = buf_temp.Value.ToString();
                        }
                    }
                }

            }

            return dt;
        }

        public void DataTableToWorkSheet(System.Data.DataTable dt, bool WriteHeader = true, int ROW = 1, int COL = 1, string SheetName = "")
        {
            if (Excel == null)
                Init();

            Worksheet ws = (Worksheet)Excel.ActiveWorkbook.ActiveSheet;
            if (!string.IsNullOrEmpty(SheetName))
                ws = FindWorkSheet(SheetName);

            Microsoft.Office.Interop.Excel.Range StartCell = (Microsoft.Office.Interop.Excel.Range)ws.Cells[ROW, COL];

            int rIndex = ROW;
            int cIndex = COL;

            if (WriteHeader == true)
            {
                foreach (System.Data.DataColumn _c in dt.Columns)
                {
                    WriteCell(_c.ColumnName, rIndex, cIndex, SheetName);
                    cIndex++;
                }
                rIndex = ROW + 1;
            }

            foreach (System.Data.DataRow _r in dt.Rows)
            {
                cIndex = COL;
                foreach (System.Data.DataColumn _c in dt.Columns)
                {
                    WriteCell(_r[_c].ToString(), rIndex, cIndex, SheetName);
                    cIndex++;
                }
                rIndex++;
            }
        }

        public void Close()
        {
            Excel.Quit();
            Excel = null;
        }

    }
}

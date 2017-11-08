using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;
using System.Data;

namespace ReportESF
{
    class XLSExport
    {
        private Excel.Application xls;
        private Excel.Workbook wb;
        private DataModel d;
        private frmProgress pb;

        public XLSExport()
        {
            d = new DataModel();
        }

        #region Properties
        
        public Excel.Worksheets Sheets
        {
            get { return (Excel.Worksheets)this.wb.Worksheets; }
        }

        #endregion

        #region Methods

        public void OutputFixed(List<string> selectedParams, DateTime dtStart, DateTime dtEnd, bool withKtr, bool measuredOnly)
        {
            pb = new frmProgress();
            Excel.Range c;
            int percent;
            int firstRow = 5;
            int totalParams = selectedParams.Count;
            TimeSpan delta = TimeSpan.FromDays(1);
            int totalRows = (int)(dtEnd.AddDays(1).Subtract(dtStart).TotalSeconds / delta.TotalSeconds);
            int totalData = totalRows * totalParams;
            int completed = 0;
            int currentColumn, currentRow;
            string val;
            DataTable halfhourVals, paramInfo;
            Excel.Worksheet ws = PrepareTable1("Показания", dtStart, dtEnd, delta);
            currentColumn = 3;
            pb.Show();
            foreach (string pp_id in selectedParams)
            {
                paramInfo = d.ParamInfo(pp_id);
                currentRow = firstRow;
                c = (Excel.Range)(ws.Cells[1, currentColumn]);
                c.ColumnWidth = 24;
                c.Value = paramInfo.Rows[0][0].ToString(); // Substation name
                ws.Cells[2, currentColumn] = paramInfo.Rows[0][1].ToString(); // Meter name
                ws.Cells[3, currentColumn] = paramInfo.Rows[0][2].ToString(); // Param name
                c = (Excel.Range)(ws.Cells[firstRow - 1, currentColumn]);
                c.FormulaR1C1 = string.Format("=R[{0}]C-R[1]C", totalRows);
                c.NumberFormat = "#,##0.00";
                c.Font.Bold = true;
                c.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                c.Interior.Color = Excel.XlRgbColor.rgbGrey;
                halfhourVals = d.FixedValues(pp_id, dtStart, dtEnd, withKtr, measuredOnly);
                foreach (DataRow row in halfhourVals.Rows)
                {
                    c = ws.Cells[currentRow, currentColumn];
                    if (row[1] == null || Convert.IsDBNull(row[1]))
                        val = "--";
                    else
                    {
                        c.NumberFormat = "#,##0.00";
                        val = row[2].ToString().Replace(',', '.');
                    }
                    c.Value = val;
                    currentRow++;
                    completed++;
                    percent = 100 * completed / totalData;
                    pb.SetProgress(percent);
                }
                currentColumn++;
            }
            pb.Close();
            FinishTable(ws, firstRow, 3, totalParams,true);
            releaseObject(ws);
        }

        public void OutputHalfhours(List<string> selectedParams, DateTime dtStart, DateTime dtEnd)
        {
            pb = new frmProgress();
            Excel.Range c;
            int percent;
            int firstRow = 5;
            TimeSpan delta = TimeSpan.FromMinutes(30);
            int totalParams = selectedParams.Count;
            int totalRows = (int)(dtEnd.AddDays(1).Subtract(dtStart).TotalSeconds / delta.TotalSeconds);
            int totalData = totalRows * totalParams;
            int completed = 0;
            int currentColumn, currentRow;
            string val;
            DataTable halfhourVals, paramInfo;
            Excel.Worksheet ws = PrepareTable1("Получасовки", dtStart, dtEnd, delta);
            currentColumn = 3;
            pb.Show();
            foreach (string pp_id in selectedParams)
            {
                paramInfo = d.ParamInfo(pp_id);
                currentRow = firstRow;
                c = (Excel.Range)(ws.Cells[1, currentColumn]);
                c.ColumnWidth = 24;
                c.Value = paramInfo.Rows[0][0].ToString(); // Substation name
                ws.Cells[2, currentColumn] = paramInfo.Rows[0][1].ToString(); // Meter name
                ws.Cells[3, currentColumn] = paramInfo.Rows[0][2].ToString(); // Param name
                c = (Excel.Range)(ws.Cells[firstRow - 1, currentColumn]);
                c.FormulaR1C1 = string.Format("=SUM(R[1]C:R[{0}]C)", totalRows);
                c.NumberFormat = "#,##0.00";
                c.Font.Bold = true;
                c.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                c.Interior.Color = Excel.XlRgbColor.rgbGrey;
                halfhourVals = d.HalfhourValues(pp_id, dtStart, dtEnd);
                foreach (DataRow row in halfhourVals.Rows)
                {
                    c = ws.Cells[currentRow, currentColumn];
                    if (row[1] == null || Convert.IsDBNull(row[1]))
                        val = "--";
                    else
                    {
                        c.NumberFormat = "#,##0.00";
                        val = row[1].ToString().Replace(',', '.');
                    }
                    c.Value = val;
                    currentRow++;
                    completed++;
                    percent = 100 * completed / totalData;
                    pb.SetProgress(percent);
                }
                currentColumn++;
            }
            pb.Close();
            FinishTable(ws, firstRow, 3, totalParams,true);
            releaseObject(ws);
        }

        public void OutputHours(List<string> selectedParams, DateTime dtStart, DateTime dtEnd)
        {
            pb = new frmProgress();
            Excel.Range c;
            int percent;
            int firstRow = 5;
            TimeSpan delta = TimeSpan.FromHours(1);
            int totalParams = selectedParams.Count;
            int totalRows = (int)(dtEnd.AddDays(1).Subtract(dtStart).TotalSeconds / delta.TotalSeconds);
            int totalData = totalRows * totalParams;
            int completed = 0;
            int currentColumn, currentRow;
            string val;   
            DataTable hourVals, paramInfo;
            Excel.Worksheet ws = PrepareTable1("Часовки", dtStart, dtEnd, delta);
            currentColumn = 3;
            pb.Show();
            foreach (string pp_id in selectedParams)
            {
                paramInfo = d.ParamInfo(pp_id);
                currentRow = firstRow;
                c = (Excel.Range)(ws.Cells[1, currentColumn]);
                c.ColumnWidth = 24;
                c.Value = paramInfo.Rows[0][0].ToString(); // Substation name
                ws.Cells[2, currentColumn] = paramInfo.Rows[0][1].ToString(); // Meter name
                ws.Cells[3, currentColumn] = paramInfo.Rows[0][2].ToString(); // Param name
                c = (Excel.Range)(ws.Cells[firstRow - 1, currentColumn]);
                c.FormulaR1C1 = string.Format("=SUM(R[1]C:R[{0}]C)", totalRows);
                c.NumberFormat = "#,##0.00";
                c.Font.Bold = true;
                c.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                c.Interior.Color = Excel.XlRgbColor.rgbGrey;
                hourVals = d.HourValues(pp_id, dtStart, dtEnd);
                foreach(DataRow row in hourVals.Rows)
                {
                    c = ws.Cells[currentRow, currentColumn];
                    if (row[1] == null || Convert.IsDBNull(row[1]))
                        val = "--";
                    else
                    {
                        c.NumberFormat = "#,##0.00";
                        val = row[1].ToString().Replace(',', '.');
                    }
                    c.Value = val;
                    currentRow++;
                    completed++;
                    percent = 100 * completed / totalData;
                    pb.SetProgress(percent);
                }
                currentColumn++;
            }
            pb.Close();
            FinishTable(ws, firstRow, 3, totalParams, true);
            releaseObject(ws);
        }

        public void OutputDaily(List<string> selectedParams, DateTime dtStart, DateTime dtEnd)
        {
            pb = new frmProgress();
            Excel.Range c;
            int percent;
            int firstColumn = 5;
            int totalParams = selectedParams.Count;
            int totalColumns = (int)(dtEnd - dtStart).TotalDays + 1;
            int totalData = totalColumns * totalParams;
            int completed = 0;
            int currentColumn, currentRow;
            string val;
            DataTable dailyVals, paramInfo;
            Excel.Worksheet ws = PrepareTable2("Посуточно", dtStart, dtEnd);
            currentRow = 2;
            pb.Show();
            foreach (string pp_id in selectedParams)
            {
                paramInfo = d.ParamInfo(pp_id);
                c = (Excel.Range)(ws.Cells[currentRow,1]);
                c.Value = paramInfo.Rows[0][0].ToString(); // Substation name
                ws.Cells[currentRow,2] = paramInfo.Rows[0][1].ToString(); // Meter name
                ws.Cells[currentRow,3] = paramInfo.Rows[0][2].ToString(); // Param name
                c = (Excel.Range)(ws.Cells[currentRow,4]);
                c.FormulaR1C1 = string.Format("=SUM(RC[1]:RC[{0}])", totalColumns);
                c.Font.Bold = true;
                c.NumberFormat = "#,##0.00";
                c.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                c.Interior.Color = Excel.XlRgbColor.rgbGrey;
                dailyVals = d.DailyValues(pp_id, dtStart, dtEnd);
                currentColumn = 5;
                foreach (DataRow row in dailyVals.Rows)
                {
                    c = ws.Cells[currentRow, currentColumn];
                    if (row[1] == null || Convert.IsDBNull(row[1]))
                        val = "--";
                    else
                    {
                        c.NumberFormat = "#,##0.00";
                        val = row[1].ToString().Replace(',', '.');
                    }
                    c.Value = val;
                    currentColumn++;
                    completed++;
                    percent = 100 * completed / totalData;
                    pb.SetProgress(percent);
                }
                currentRow++;
            }
            pb.Close();
            FinishTable(ws, 2, firstColumn, totalParams, false);
            releaseObject(ws);
        }


        /// <summary>
        /// Table1 is for reports where we have two leftmost columns for date and time and first three rows contain 
        /// information about measuring channel
        /// </summary>
        /// <param name="sheetName"></param>
        /// <param name="dtStart"></param>
        /// <param name="dtEnd"></param>
        /// <param name="increment"></param>
        /// <returns></returns>
        private Excel.Worksheet PrepareTable1(string sheetName, DateTime dtStart, DateTime dtEnd, TimeSpan increment)
        {
            Excel.Range c;
            int firstRow = 5;
            int totalRows;
            xls = new Excel.Application();
            xls.SheetsInNewWorkbook = 1;
            wb = xls.Workbooks.Add();
            Excel.Worksheet ws = (Excel.Worksheet)wb.Worksheets[1];
            ws.Name = sheetName;
            c = (Excel.Range)(ws.Cells[1, 1]);
            c.Value = "Период с";
            c.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
            c = (Excel.Range)(ws.Cells[2, 1]);
            c.Value = "по";
            c.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
            c = (Excel.Range)(ws.Cells[1, 2]);
            c.Value = dtStart.ToShortDateString();
            c.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            c = (Excel.Range)(ws.Cells[2, 2]);
            c.Value = dtEnd.ToShortDateString();
            c.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;

            c = (Excel.Range)(ws.Cells[firstRow - 1, 1]);
            c.Value = "Дата";
            c.ColumnWidth = 12;
            c.Interior.Color = Excel.XlRgbColor.rgbGrey;
            c = (Excel.Range)(ws.Cells[firstRow - 1, 2]);
            c.Value = "Время";
            c.ColumnWidth = 13;
            c.Interior.Color = Excel.XlRgbColor.rgbGrey;
            DateTime currentDate = dtStart;
            int currentRow = firstRow;
            totalRows = (int)(dtEnd.AddDays(1).Subtract(dtStart).TotalSeconds / increment.TotalSeconds);
            string[,] leftColumns = new string[totalRows, 2];

            while (currentDate < dtEnd.AddDays(1))
            {
                leftColumns[currentRow - firstRow, 0] =
                    currentDate.Date.ToShortDateString();
                leftColumns[currentRow - firstRow, 1] =
                    string.Format("{0:00}:{1:00} - {2:00}:{3:00}",
                                  currentDate.TimeOfDay.Hours,
                                  currentDate.TimeOfDay.Minutes,
                                  (currentDate + increment).TimeOfDay.Hours,
                                  (currentDate + increment).TimeOfDay.Minutes);
                currentDate = currentDate + increment;
                currentRow++;
            }
            c = (Excel.Range)ws.Cells[firstRow, 1];
            c = c.Resize[totalRows, 2];
            c.Value = leftColumns;
            return ws;
        }

        /// <summary>
        /// Table2 is for reports where we have three leftmost columns for information about measuring channel
        /// and the top row contains date and increment is 1 day
        /// </summary>
        /// <param name="sheetName"></param>
        /// <param name="dtStart"></param>
        /// <param name="dtEnd"></param>
        /// <returns></returns>
        private Excel.Worksheet PrepareTable2(string sheetName, DateTime dtStart, DateTime dtEnd)
        {
            Excel.Range c;
            int firstColumn = 5;
            xls = new Excel.Application();
            xls.SheetsInNewWorkbook = 1;
            wb = xls.Workbooks.Add();
            Excel.Worksheet ws = (Excel.Worksheet)wb.Worksheets[1];
            ws.Name = sheetName;
            c = (Excel.Range)(ws.Cells[1, 1]);
            c.Value = "Подстанция";
            c.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            c.ColumnWidth = 24;
            c = (Excel.Range)(ws.Cells[1, 2]);
            c.Value = "Присоединение";
            c.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            c.ColumnWidth = 24;
            c = (Excel.Range)(ws.Cells[1, 3]);
            c.Value = "Канал";
            c.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            c.ColumnWidth = 8;
            c = (Excel.Range)(ws.Cells[1, 4]);
            c.Value = "Сумма";
            c.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            c.ColumnWidth = 16;

            c.Interior.Color = Excel.XlRgbColor.rgbGrey;
            DateTime currentDate = dtStart;
            int currentColumn = firstColumn;
            while (currentDate < dtEnd.AddDays(1))
            {
                c = (Excel.Range)(ws.Cells[1, currentColumn]);
                c.Value = currentDate;
                c.ColumnWidth = 18;
                c.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                c.Font.Bold = true;
                currentDate = currentDate.AddDays(1);
                currentColumn++;
            }
            return ws;
        }


        private void FinishTable(Excel.Worksheet ws, int firstRow,int firstColumn, int totalParams, bool horizontal)
        {
            Excel.Range c;
            ws.UsedRange.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            xls.Visible = true;
            c = (Excel.Range)ws.Cells[firstRow, firstColumn];
            c.Select();
            Excel.Windows xlsWindows = wb.Windows;
            Excel.Window xlsWindow = xlsWindows[1];
            xlsWindow.FreezePanes = true;
            wb.Activate();
            xlsWindow.Activate();
            releaseObject(wb);
            releaseObject(xls);
        }


       
        private void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception e)
            {
                obj = null;
                System.Windows.Forms.MessageBox.Show("Ошибка при освобождении ресурса Excel " + e.ToString());
            }
            finally
            {
                GC.Collect();
            }
        }
        #endregion
    }
}

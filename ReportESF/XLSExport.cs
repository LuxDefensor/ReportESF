using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;
using System.Data;
using Energosphere;

namespace ReportESF
{
    public enum Reports
    {
        Hours,
        Halfhours,
        Daily,
        Fixed,
        FixedWithoutKtr,
        PairOfFixed,
        Measured,
        Log
    }

    class XLSExport
    {
        private Excel.Application xls;
        private Excel.Workbook wb;
        private frmProgress pb;
        private Calculator c;
        private DataModel m;

        public XLSExport(string settingsFile)
        {
            c = new Calculator(settingsFile);
            m = new DataModel(settingsFile);
        }

        public XLSExport()
        {
            c = new Calculator();
            m = new DataModel();
        }
        #region Properties
        
        public Excel.Worksheets Sheets
        {
            get { return (Excel.Worksheets)this.wb.Worksheets; }
        }

        #endregion

        #region Methods

        public void OutputMeterLogs(List<Parameter> selectedParams, DateTime dtStart, DateTime dtEnd)
        {
            pb = new frmProgress();
            int totalRows, complete;
            Excel.Range cell;
            DataTable logs;
            xls = new Excel.Application();
            xls.SheetsInNewWorkbook = 1;
            wb = xls.Workbooks.Add();
            Excel.Worksheet ws = (Excel.Worksheet)wb.Worksheets[1];
            logs = c.MeterLogs(selectedParams, dtStart, dtEnd);
            totalRows = logs.Rows.Count;
            complete = 0;
            string[,] entries = new string[logs.Rows.Count, 5];
            for (int i = 0; i < logs.Rows.Count; i++)
            {
                entries[i, 0] = logs.Rows[i][0].ToString();
                entries[i, 1] = logs.Rows[i][1].ToString();
                entries[i, 2] = logs.Rows[i][2].ToString();
                entries[i, 3] = logs.Rows[i][3].ToString();
                entries[i, 4] = logs.Rows[i][4].ToString();
                pb.SetProgress(100 * complete / totalRows);
            }
            cell = (Excel.Range)(ws.Cells[2, 1]);
            cell = cell.Resize[logs.Rows.Count, 5];
            cell.Value = entries;
            cell = (Excel.Range)(ws.Cells[1, 1]);
            cell.ColumnWidth = 40;
            cell.Value = "Подстанция";
            cell = (Excel.Range)(ws.Cells[1, 2]);
            cell.ColumnWidth = 20;
            cell.Value = "Счетчик";
            cell = (Excel.Range)(ws.Cells[1, 3]);
            cell.ColumnWidth = 18;
            cell.Value = "Дата";
            cell = (Excel.Range)(ws.Cells[1, 4]);
            cell.ColumnWidth = 45;
            cell.Value = "Запись";
            cell = (Excel.Range)(ws.Cells[1, 5]);
            cell.ColumnWidth = 36;
            cell.Value = "Дополнительно";
            cell = (Excel.Range)ws.Range["A1:E1"];
            cell.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            cell.Font.Bold = true;
            ws.UsedRange.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            xls.Visible = true;
            int firstRow = 2, firstColumn = 1;
            cell = (Excel.Range)ws.Cells[firstRow, firstColumn];
            cell.Select();
            Excel.Windows xlsWindows = wb.Windows;
            Excel.Window xlsWindow = xlsWindows[1];
            xlsWindow.FreezePanes = true;
            wb.Activate();
            xlsWindow.Activate();
            pb.Close();
            releaseObject(ws);
            releaseObject(wb);
            releaseObject(xls);
        }

        /// <summary>
        /// Dates vertically in the two left columns, parameters horizontally in the three top rows
        /// </summary>
        /// <param name="selectedParams"></param>
        /// <param name="dtStart"></param>
        /// <param name="dtEnd"></param>
        /// <param name="delta"></param>
        /// <param name="title"></param>
        public void OutputPortrait(List<Parameter> selectedParams, Reports reportType,
            DateTime dtStart, DateTime dtEnd, TimeSpan delta, string title, bool integral)
        {
            pb = new frmProgress();
            Excel.Range cell;
            int percent;
            int firstRow = 5;
            int totalParams = selectedParams.Count;
            int totalRows = (int)(dtEnd.AddDays(1).Subtract(dtStart).TotalSeconds / delta.TotalSeconds);
            int totalData = totalRows * totalParams;
            int completed = 0;
            int currentColumn, currentRow;
            string val;
            DataTable values;
            #region Prepare table
            xls = new Excel.Application();
            xls.SheetsInNewWorkbook = 1;
            wb = xls.Workbooks.Add();
            Excel.Worksheet ws = (Excel.Worksheet)wb.Worksheets[1];
            ws.Name = title;
            cell = (Excel.Range)(ws.Cells[1, 1]);
            cell.Value = "Период с";
            cell.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
            cell = (Excel.Range)(ws.Cells[2, 1]);
            cell.Value = "по";
            cell.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
            cell = (Excel.Range)(ws.Cells[1, 2]);
            cell.Value = dtStart.ToShortDateString();
            cell.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            cell = (Excel.Range)(ws.Cells[2, 2]);
            cell.Value = dtEnd.ToShortDateString();
            cell.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            cell = (Excel.Range)(ws.Cells[firstRow - 1, 1]);
            cell.Value = "Дата";
            cell.ColumnWidth = 12;
            cell.Interior.Color = Excel.XlRgbColor.rgbGrey;
            cell = (Excel.Range)(ws.Cells[firstRow - 1, 2]);
            cell.Value = "Время";
            cell.ColumnWidth = 13;
            cell.Interior.Color = Excel.XlRgbColor.rgbGrey;
            DateTime currentDate = dtStart;
            currentRow = firstRow;
            totalRows = (int)(dtEnd.AddDays(1).Subtract(dtStart).TotalSeconds / delta.TotalSeconds);
            string[,] leftColumns = new string[totalRows, 2];
            while (currentDate < dtEnd.AddDays(1))
            {
                leftColumns[currentRow - firstRow, 0] =
                    currentDate.Date.ToShortDateString();
                leftColumns[currentRow - firstRow, 1] =
                    string.Format("{0:00}:{1:00} - {2:00}:{3:00}",
                                  currentDate.TimeOfDay.Hours,
                                  currentDate.TimeOfDay.Minutes,
                                  (currentDate + delta).TimeOfDay.Hours,
                                  (currentDate + delta).TimeOfDay.Minutes);
                currentDate = currentDate + delta;
                currentRow++;
            }
            cell = (Excel.Range)ws.Cells[firstRow, 1];
            cell = cell.Resize[totalRows, 2];
            cell.Value = leftColumns;
            #endregion
            currentColumn = 3;
            pb.Show();
            foreach (Parameter p in selectedParams)
            {
                currentRow = firstRow;
                cell = (Excel.Range)(ws.Cells[1, currentColumn]);
                cell.ColumnWidth = 24;
                cell.Value = p.ParentPoint.GetAncestor(PointTypes.Substation).Name;
                ws.Cells[2, currentColumn] = p.ParentPoint.Name;
                ws.Cells[3, currentColumn] = p.TypeName;
                cell = (Excel.Range)(ws.Cells[firstRow - 1, currentColumn]);
                if (integral)
                    cell.FormulaR1C1 = string.Format("=R[{0}]C-R[1]C", totalRows);
                else
                    cell.FormulaR1C1 = string.Format("=SUM(R[1]C:R[{0}]C)", totalRows);
                cell.NumberFormat = "#,##0.00";
                cell.Font.Bold = true;
                cell.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                cell.Interior.Color = Excel.XlRgbColor.rgbGrey;
                values = null;
                try
                {
                    switch (reportType)
                    {
                        case Reports.Hours:
                            values = c.HourValues(p.Id.ToString(), dtStart, dtEnd);
                            break;
                        case Reports.Halfhours:
                            values = c.HalfhourValues(p.Id.ToString(), dtStart, dtEnd);
                            break;
                        case Reports.Daily:
                            values = c.DailyValues(p.Id.ToString(), dtStart, dtEnd);
                            break;
                        case Reports.Fixed:
                            values = c.FixedValues(p.Id.ToString(), dtStart, dtEnd, true, false);
                            break;
                        case Reports.FixedWithoutKtr:
                            values = c.FixedValues(p.Id.ToString(), dtStart, dtEnd, false, false);
                            break;
                        case Reports.PairOfFixed:
                            values = c.PairOfFixedValues(p.Id.ToString(), dtStart, dtEnd);
                            break;
                        case Reports.Measured:
                            values = c.FixedValues(p.Id.ToString(), dtStart, dtEnd, false, true);
                            break;
                        case Reports.Log:
                            throw new Exception("PortraitOutput: this method cannot otuput <Meters' logs> report");
                    }
                }
                catch (Exception ex)
                {
                    string details = Settings.ErrorInfo(ex, "XLSExport.OutputPortrait") + Environment.NewLine +
                        "id_point = " + p.ParentPoint.ID.ToString() + ", id_pp = " + p.Id.ToString();
                    formError frm = new formError("Ошибка при выгрузке значений из БД", "Ошибка!", details);
                    frm.ShowDialog();
                    return;
                }
                foreach (DataRow row in values.Rows)
                {
                    cell = ws.Cells[currentRow, currentColumn];
                    if (row[1] == null || Convert.IsDBNull(row[1]))
                        val = "--";
                    else
                    {
                        cell.NumberFormat = "#,##0.00";
                        val = row[1].ToString().Replace(',', '.');
                    }
                    if (row[2]==null || Convert.IsDBNull(row[2]) || (int)row[2] != 0)
                        cell.Font.Color = Excel.XlRgbColor.rgbRed;
                    cell.Value = val;
                    currentRow++;
                    completed++;
                    percent = 100 * completed / totalData;
                    pb.SetProgress(percent);
                }
                currentColumn++;
            }
            #region Finish table
            ws.UsedRange.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            xls.Visible = true;
            cell = (Excel.Range)ws.Cells[firstRow, 3];
            cell.Select();
            Excel.Windows xlsWindows = wb.Windows;
            Excel.Window xlsWindow = xlsWindows[1];
            xlsWindow.FreezePanes = true;
            wb.Activate();
            xlsWindow.Activate();
            #endregion
            pb.Close();
            releaseObject(ws);
            releaseObject(wb);
            releaseObject(xls);
        }

        /// <summary>
        /// Dates horizontally in the two top rows, parameters vertically in the three left columns
        /// </summary>
        /// <param name="selectedParams"></param>
        /// <param name="dtStart"></param>
        /// <param name="dtEnd"></param>
        /// <param name="delta"></param>
        /// <param name="title"></param>
        public void OutputLandscape(List<Parameter> selectedParams, Reports reportType,
            DateTime dtStart, DateTime dtEnd, TimeSpan delta, string title, bool integral)
        {
            pb = new frmProgress();
            Excel.Range cell;
            int percent;
            int firstColumn = 5, firstRow = 2;
            int totalParams = selectedParams.Count;
            int totalColumns = (int)(dtEnd.AddDays(1).Subtract(dtStart).TotalSeconds / delta.TotalSeconds);
            int totalData = totalColumns * totalParams;
            int completed = 0;
            int currentColumn, currentRow;
            string val;
            DataTable values;
            #region Prepare table
            xls = new Excel.Application();
            xls.SheetsInNewWorkbook = 1;
            wb = xls.Workbooks.Add();
            Excel.Worksheet ws = (Excel.Worksheet)wb.Worksheets[1];
            ws.Name = title;
            cell = (Excel.Range)(ws.Cells[1, 1]);
            cell.Value = "Подстанция";
            cell.ColumnWidth = 24;
            cell.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            cell = (Excel.Range)(ws.Cells[1, 2]);
            cell.Value = "Присоединение";
            cell.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            cell.ColumnWidth = 24;
            cell = (Excel.Range)(ws.Cells[1, 3]);
            cell.Value = "Канал";
            cell.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            cell.ColumnWidth = 8;
            cell = (Excel.Range)(ws.Cells[1, 4]);
            cell.Value = "Сумма";
            cell.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            cell.ColumnWidth = 16;
            cell.Interior.Color = Excel.XlRgbColor.rgbGray;
            DateTime currentDate = dtStart;
            currentColumn = firstColumn;
            while (currentDate < dtEnd.AddDays(1))
            {
                cell = (Excel.Range)(ws.Cells[1, currentColumn]);
                cell.Value = currentDate;
                cell.NumberFormat = (delta.TotalDays >= 1) ? "dd.mm.yyyy" : "dd.mm.yyyy HH:mm;@";
                cell.ColumnWidth = 18;
                cell.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                cell.Font.Bold = true;
                currentDate = currentDate.Add(delta);
                currentColumn++;
            }
            if (reportType == Reports.PairOfFixed)
                totalColumns = 2;
            else
                totalColumns = (int)(dtEnd.AddDays(1).Subtract(dtStart).TotalSeconds / delta.TotalSeconds);
            #endregion
            currentRow = firstRow;
            pb.Show();
            foreach (Parameter p in selectedParams)
            {
                currentColumn = firstColumn;
                ws.Cells[currentRow, 1] = p.ParentPoint.GetAncestor(PointTypes.Substation).Name;
                ws.Cells[currentRow, 2] = p.ParentPoint.Name;
                ws.Cells[currentRow, 3] = p.TypeName;
                cell = (Excel.Range)(ws.Cells[currentRow, 4]);
                if (integral)
                    cell.FormulaR1C1 = string.Format("=RC[{0}]-RC[1]", totalColumns);
                else
                    cell.FormulaR1C1 = string.Format("=SUM(RC[1]:RC[{0}])", totalColumns);
                cell.NumberFormat = "#,##0.00";
                cell.Font.Bold = true;
                cell.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                cell.Interior.Color = Excel.XlRgbColor.rgbGrey;
                values = null;
                try
                {
                    switch (reportType)
                    {
                        case Reports.Hours:
                            values = c.HourValues(p.Id.ToString(), dtStart, dtEnd);
                            break;
                        case Reports.Halfhours:
                            values = c.HalfhourValues(p.Id.ToString(), dtStart, dtEnd);
                            break;
                        case Reports.Daily:
                            values = c.DailyValues(p.Id.ToString(), dtStart, dtEnd);
                            break;
                        case Reports.Fixed:
                            values = c.FixedValues(p.Id.ToString(), dtStart, dtEnd, true, false);
                            break;
                        case Reports.FixedWithoutKtr:
                            values = c.FixedValues(p.Id.ToString(), dtStart, dtEnd, false, false);
                            break;
                        case Reports.PairOfFixed:
                            values = c.PairOfFixedValues(p.Id.ToString(), dtStart, dtEnd);
                            break;
                        case Reports.Measured:
                            values = c.FixedValues(p.Id.ToString(), dtStart, dtEnd, false, true);
                            break;
                        case Reports.Log:
                            throw new Exception("PortraitOutput: this method cannot otuput <Meters' logs> report");
                    }
                }
                catch (Exception ex)
                {
                    string details = Settings.ErrorInfo(ex, "XLSExport.OutputLandscape") + Environment.NewLine +
                        "id_point = " + p.ParentPoint.ID.ToString() + ", id_pp = " + p.Id.ToString();
                    formError frm = new formError("Ошибка при выгрузке значений из БД", "Ошибка!", details);
                    frm.ShowDialog();
                    return;
                }
                foreach (DataRow row in values.Rows)
                {
                    cell = ws.Cells[currentRow, currentColumn];
                    if (row[1] == null || Convert.IsDBNull(row[1]))
                        val = "--";
                    else
                    {
                        cell.NumberFormat = "#,##0.00";
                        val = row[1].ToString().Replace(',', '.');
                    }
                    if (row[2]==null || Convert.IsDBNull(row[2]) || (int)row[2] != 0)
                        cell.Font.Color = Excel.XlRgbColor.rgbRed;
                    cell.Value = val;
                    currentColumn++;
                    completed++;
                    percent = 100 * completed / totalData;
                    pb.SetProgress(percent);
                }
                currentRow++;
            }
            #region Finish table
            ws.UsedRange.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            xls.Visible = true;
            cell = (Excel.Range)ws.Cells[firstRow, firstColumn];
            cell.Select();
            Excel.Windows xlsWindows = wb.Windows;
            Excel.Window xlsWindow = xlsWindows[1];
            xlsWindow.FreezePanes = true;
            wb.Activate();
            xlsWindow.Activate();
            #endregion
            pb.Close();
            releaseObject(ws);
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

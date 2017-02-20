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

        public void OutputHours(List<string> selectedParams, DateTime dtStart, DateTime dtEnd)
        {
            Excel.Range c;
            int percent;
            int firstRow = 5;
            int totalParams = selectedParams.Count;
            int totalRows;
            int totalData; // totalRows * totalSensors
            int completed = 0;
            string val;            
            DataTable hourVals, paramInfo;
            xls = new Excel.Application();
            xls.SheetsInNewWorkbook = 1;
            wb = xls.Workbooks.Add();
            Excel.Worksheet ws = (Excel.Worksheet)wb.Worksheets[1];
            ws.Name = "Часовки";
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
            int currentColumn = 3;
            totalRows = (int)dtEnd.AddDays(1).Subtract(dtStart).TotalHours;
            totalData = totalRows * totalParams;
            string[,] leftColumns = new string[totalRows, 2];

            pb = new frmProgress();
            pb.Show();
            pb.SetProgress(1);
            while (currentDate < dtEnd.AddDays(1))
            {
                leftColumns[currentRow - firstRow, 0] =
                    currentDate.Date.ToShortDateString();
                leftColumns[currentRow - firstRow, 1] =
                    string.Format("{0:00}:{1:00} - {2:00}:{3:00}",
                                  currentDate.TimeOfDay.Hours,
                                  currentDate.TimeOfDay.Minutes,
                                  currentDate.AddHours(1).TimeOfDay.Hours,
                                  currentDate.AddHours(1).TimeOfDay.Minutes);
                currentDate = currentDate.AddHours(1);
                currentRow++;
            }
            c = (Excel.Range)ws.Cells[firstRow, 1];
            c = c.Resize[totalRows, 2];
            c.Value = leftColumns;
            currentColumn = 3;
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
                    currentDate = currentDate.AddMinutes(30);
                    currentRow++;
                    completed++;
                    percent = 100 * completed / totalData;
                    pb.SetProgress(percent);
                }
                currentColumn++;
            }
            pb.Close();
            ws.UsedRange.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            xls.Visible = true;
            c = (Excel.Range)ws.Cells[firstRow, 3];
            c.Select();
            Excel.Windows xlsWindows = wb.Windows;
            Excel.Window xlsWindow = xlsWindows[1];
            xlsWindow.FreezePanes = true;
            c = (Excel.Range)ws.Cells[firstRow - 1, 3];
            c = c.Resize[1, totalParams];
            c.NumberFormat = "#,##0";
            xlsWindow.Activate();
            releaseObject(ws);
            releaseObject(wb);
            releaseObject(xls);
        }

        #region Methods
        /*
         * 
        public void OutputHours1(List<string> selectedParams, DateTime dtStart, DateTime dtEnd)
        {
            Cell c;
            int percent;
            int firstRow = 5;
            int totalParams = selectedParams.Count;
            int totalRows;
            int totalData; // totalRows * totalSensors
            int completed = 0;
            string val;
            DataTable hourVals, paramInfo;
            #region Creating the xlsx file
            string fileName = string.Format("Часовки за {0} - {1}.xlsx",
                dtStart.ToString("yyyy-MM-dd"), dtEnd.ToString("yyyy-MM-dd"));
            SpreadsheetDocument doc = SpreadsheetDocument.Create(
                fileName, DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook, true);
            WorkbookPart part = doc.AddWorkbookPart();
            part.Workbook = new Workbook();
            WorksheetPart wspart = part.AddNewPart<WorksheetPart>();
            wspart.Worksheet = new Worksheet(new SheetData());
            Sheets ss = doc.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());
            Sheet s = new Sheet()
            {
                Id = doc.WorkbookPart.GetIdOfPart(wspart),
                SheetId = 1,
                Name = "Часовки"
            };
            ss.Append(s);
            #endregion
            SheetData sheetData = wspart.Worksheet.GetFirstChild<SheetData>();
            Row row = new Row() { RowIndex = 1 };
            sheetData.Append(row);
            c = new Cell() { CellReference = "A1", CellValue = new CellValue("Период с") };
            c.DataType = CellValues.String;
            CellFormats formats = new CellFormats();
            c.AppendChild<CellFormats>(formats);
            CellFormat format = new CellFormat()
            {
                Alignment = new Alignment() { Horizontal = HorizontalAlignmentValues.Center }
            };
            formats.Append(format);
            row.InsertAt(c, 0);


            part.Workbook.Save();
            doc.Close();
        }


        public void OutputHalves(List<long> selectedSensors, DateTime dtStart, DateTime dtEnd)
        {                    
            Excel.Range c;
        	int percent;
            int firstRow = 4;
            int totalSensors = selectedSensors.Count;
			int totalRows;
            int totalData; // totalRows * totalSensors
            int completed = 0;
            double halfhour;
            Dictionary<string, string> sensorInfo;
            DataSet halfhours;
            xls = new Excel.Application();
            xls.SheetsInNewWorkbook = 1;
            wb = xls.Workbooks.Add();            
			Excel.Worksheet ws = (Excel.Worksheet)wb.Worksheets[1];
            ws.Name = "Получасовки";			
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
            DateTime currentDate = dtStart.AddMinutes(30);
            int currentRow = firstRow;
            int currentColumn = 3;
            totalRows = (int)dtEnd.AddDays(1).Subtract(dtStart).TotalMinutes / 30;
            totalData = totalRows * totalSensors;
            string[,] leftColumns = new string[totalRows,2];
            
            pb = new frmProgress();
            pb.Show();
            pb.SetProgress(1);
            while (currentDate <= dtEnd.AddDays(1))
            {
            	leftColumns[currentRow - firstRow, 0] = 
            		currentDate.AddMinutes(-30).Date.ToShortDateString();
                leftColumns[currentRow - firstRow, 1] = 
                	string.Format("{0:00}:{1:00} - {2:00}:{3:00}", 
                	              currentDate.AddMinutes(-30).TimeOfDay.Hours, 
                	              currentDate.AddMinutes(-30).TimeOfDay.Minutes,
                	              currentDate.TimeOfDay.Hours, 
                	              currentDate.TimeOfDay.Minutes);
                currentDate = currentDate.AddMinutes(30);
                currentRow++;
            }
            c = (Excel.Range)ws.Cells[firstRow,1];
            c = c.Resize[totalRows,2];
            c.Value = leftColumns;
            currentColumn = 3;
            foreach (long sensorID in selectedSensors)
            {
                sensorInfo = d.SensorInfo(sensorID);
                currentDate = dtStart.AddMinutes(30);
                currentRow = firstRow;
                c = (Excel.Range)(ws.Cells[1, currentColumn]);
                c.ColumnWidth = 18;
                c.Value = sensorInfo["DeviceName"];
                ws.Cells[2, currentColumn] = sensorInfo["SensorName"];
                c = (Excel.Range)(ws.Cells[firstRow - 1, currentColumn]);
                c.FormulaR1C1 = string.Format("=SUM(R[1]C:R[{0}]C)/2", totalRows);
                c.Font.Bold = true;
                c.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                c.Interior.Color = Excel.XlRgbColor.rgbGrey;
                while (currentDate <= dtEnd.AddDays(1))
                {
                    //halfhours = d.GetHalfhoursDaily(long.Parse(sensorInfo["DeviceCode"]),
                    //           long.Parse(sensorInfo["SensorCode"]),
                    //           currentDate);
                    halfhour = d.GetSingleHalfhour(long.Parse(sensorInfo["DeviceCode"]),
                        long.Parse(sensorInfo["SensorCode"]),
                        currentDate);
                    if (halfhour < 0)
                        ws.Cells[currentRow, currentColumn] = "";
                    else
                        ws.Cells[currentRow, currentColumn] = halfhour;
                    //foreach (DataRow row in halfhours.Tables[0].Rows)
                    //{
                    //    ws.Cells[currentRow, currentColumn] = row["value0"];
                    //    currentRow++;
                    //    completed++;
                    //}
                    //currentDate = currentDate.AddDays(1);
                    currentDate = currentDate.AddMinutes(30);
                    currentRow++;
                    completed++;
                    percent = 100 * completed / totalData;
                    pb.SetProgress(percent);
                }
                currentColumn++;
            }
            pb.Close();
            ws.UsedRange.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
			xls.Visible = true;
			c = (Excel.Range)ws.Cells[firstRow, 3];
			c.Select();
			Excel.Windows xlsWindows = wb.Windows;
			Excel.Window xlsWindow = xlsWindows[1];
			xlsWindow.FreezePanes = true;   
			c = (Excel.Range)ws.Cells[firstRow - 1, 3];			
            c = c.Resize[1, totalSensors];
            c.NumberFormat = "#,##0";
            xlsWindow.Activate();
            releaseObject(ws);
            releaseObject(wb);
            releaseObject(xls);
        }

        public void OutputHalvesPlusHours(List<long> selectedSensors, DateTime dtStart, DateTime dtEnd)
        {
            Excel.Range c;
            int percent;
            int firstRow = 4;
            int totalSensors = selectedSensors.Count;
            int totalRows;
            int totalData;
            int completed = 0;
            double firstHalf = 0; // The first halfhour of the two forming hour value as their average
            Dictionary<string, string> sensorInfo;
            long deviceCode;
            long sensorCode;
            double halfhour;
            xls = new Excel.Application();
            xls.SheetsInNewWorkbook = 2;
            wb = xls.Workbooks.Add();
            Excel.Worksheet ws1 = (Excel.Worksheet)wb.Worksheets[1];
            Excel.Worksheet ws2 = (Excel.Worksheet)wb.Worksheets[2];
            ws1.Name = "Получасовки";
            ws2.Name = "Часовки";
            #region Prepare headers of halfhours worksheet
            c = (Excel.Range)(ws1.Cells[1, 1]);
            c.Value = "Период с";
            c.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
            c = (Excel.Range)(ws1.Cells[2, 1]);
            c.Value = "по";
            c.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
            c = (Excel.Range)(ws1.Cells[1, 2]);
            c.Value = dtStart.ToShortDateString();
            c.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            c = (Excel.Range)(ws1.Cells[2, 2]);
            c.Value = dtEnd.ToShortDateString();
            c.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            c = (Excel.Range)(ws1.Cells[firstRow - 1, 1]);
            c.Value = "Дата";
            c.ColumnWidth = 12;
            c.Interior.Color = Excel.XlRgbColor.rgbGrey;
            c = (Excel.Range)(ws1.Cells[firstRow - 1, 2]);
            c.Value = "Время";
            c.ColumnWidth = 13;
            c.Interior.Color = Excel.XlRgbColor.rgbGrey;
            #endregion

            #region Prepare headers of hours worksheet
            c = (Excel.Range)(ws2.Cells[1, 1]);
            c.Value = "Период с";
            c.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
            c = (Excel.Range)(ws2.Cells[2, 1]);
            c.Value = "по";
            c.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
            c = (Excel.Range)(ws2.Cells[1, 2]);
            c.Value = dtStart.ToShortDateString();
            c.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            c = (Excel.Range)(ws2.Cells[2, 2]);
            c.Value = dtEnd.ToShortDateString();
            c.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            c = (Excel.Range)(ws2.Cells[firstRow - 1, 1]);
            c.Value = "Дата";
            c.ColumnWidth = 12;
            c.Interior.Color = Excel.XlRgbColor.rgbGrey;
            c = (Excel.Range)(ws2.Cells[firstRow - 1, 2]);
            c.Value = "Время";
            c.ColumnWidth = 13;
            c.Interior.Color = Excel.XlRgbColor.rgbGrey;
            #endregion

            DateTime currentDate = dtStart;

            pb = new frmProgress();
            pb.Show();
            pb.SetProgress(1);

            #region Write dates and times into two leftmost coumns of halfhours sheet
            int currentRow = 0;
            int currentColumn = 3;
            totalRows = (int)dtEnd.AddDays(1).Subtract(dtStart).TotalMinutes / 30;
            string[,] leftColumns = new string[totalRows, 2];
            while (currentDate < dtEnd.AddDays(1))
            {
                leftColumns[currentRow, 0] =
                    currentDate.Date.ToShortDateString();
                leftColumns[currentRow, 1] =
                    string.Format("{0:00}:{1:00} - {2:00}:{3:00}",
                                  currentDate.TimeOfDay.Hours,
                                  currentDate.TimeOfDay.Minutes,
                                  currentDate.AddMinutes(30).TimeOfDay.Hours,
                                  currentDate.AddMinutes(30).TimeOfDay.Minutes);
                currentDate = currentDate.AddMinutes(30);
                currentRow++;
            }
            c = (Excel.Range)ws1.Cells[firstRow, 1];
            c = c.Resize[totalRows, 2];
            c.Value = leftColumns;
            #endregion

            #region Write dates and times into two leftmost coumns of hours sheet
            currentDate = dtStart;
            currentRow = 0;
            totalRows = (int)dtEnd.AddDays(1).Subtract(dtStart).TotalHours;
            leftColumns = new string[totalRows, 2];
            while (currentDate < dtEnd.AddDays(1))
            {
                leftColumns[currentRow, 0] =
                    currentDate.Date.ToShortDateString();
                leftColumns[currentRow, 1] =
                    string.Format("{0:00}:00 - {1:00}:00",
                                   currentDate.TimeOfDay.Hours,
                                   currentDate.AddHours(1).TimeOfDay.Hours);
                currentDate = currentDate.AddHours(1);
                currentRow++;
            }
            c = (Excel.Range)ws2.Cells[firstRow, 1];
            c = c.Resize[totalRows, 2];
            c.Value = leftColumns;
            #endregion

            totalRows = (int)dtEnd.AddDays(1).Subtract(dtStart).TotalMinutes / 30;
            totalData = totalRows * totalSensors;
            currentColumn = 3;
            foreach (long sensorID in selectedSensors)
            {
                sensorInfo = d.SensorInfo(sensorID);
                deviceCode = long.Parse(sensorInfo["DeviceCode"]);
                sensorCode = long.Parse(sensorInfo["SensorCode"]);
                currentDate = dtStart.AddMinutes(30);
                currentRow = firstRow;
                #region Write devices' and sensors' names into first two rows
                // halfhours column headers
                c = (Excel.Range)(ws1.Cells[1, currentColumn]);
                c.ColumnWidth = 18;
                c.Value = sensorInfo["DeviceName"];
                ws1.Cells[2, currentColumn] = sensorInfo["SensorName"];
                c = (Excel.Range)(ws1.Cells[firstRow - 1, currentColumn]);
                c.FormulaR1C1 = string.Format("=SUM(R[1]C:R[{0}]C)/2", totalRows);
                c.Font.Bold = true;
                c.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                c.Interior.Color = Excel.XlRgbColor.rgbGrey;
                // hours column headers
                c = (Excel.Range)(ws2.Cells[1, currentColumn]);
                c.ColumnWidth = 18;
                c.Value = sensorInfo["DeviceName"];
                ws2.Cells[2, currentColumn] = sensorInfo["SensorName"];
                c = (Excel.Range)(ws2.Cells[firstRow - 1, currentColumn]);
                c.FormulaR1C1 = string.Format("=SUM(R[1]C:R[{0}]C)", totalRows / 2);
                c.Font.Bold = true;
                c.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                c.Interior.Color = Excel.XlRgbColor.rgbGrey;
                #endregion
                while (currentDate <= dtEnd.AddDays(1))
                {
                    halfhour = d.GetSingleHalfhour(deviceCode, sensorCode, currentDate);
                    if (halfhour < 0)
                        ws1.Cells[currentRow, currentColumn] = "";
                    else
                        ws1.Cells[currentRow, currentColumn] = halfhour;
                    if ((currentRow - firstRow) % 2 == 0)
                    {
                        firstHalf = (halfhour < 0) ? 0 : halfhour;
                    }
                    else
                    {
                        c = (Excel.Range)ws2.Cells[(currentRow - firstRow) / 2 + firstRow, currentColumn];

                        c.Value = (firstHalf + ((halfhour < 0) ? 0 : halfhour)) / 2;
                        c.NumberFormat = "#,##0.00";
                        firstHalf = 0;
                    }
                    currentDate = currentDate.AddMinutes(30);
                    currentRow++;
                    completed++;
                    percent = 100 * completed / totalData;
                    pb.SetProgress(percent);
                }
                currentColumn++;
            }
            pb.Close();
            ws1.UsedRange.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            ws2.UsedRange.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            xls.Visible = true;
            c = (Excel.Range)ws2.Cells[firstRow, 3];
            ws2.Activate();
            c.Select();
            Excel.Windows xlsWindows = wb.Windows;
            Excel.Window xlsWindow = xlsWindows[1];
            xlsWindow.FreezePanes = true;
            c = (Excel.Range)ws1.Cells[firstRow, 3];
            ws1.Activate();
            c.Select();
            //xlsWindow = xlsWindows[1];
            xlsWindow.FreezePanes = true;
            c = (Excel.Range)ws1.Cells[firstRow - 1, 3];
            c = c.Resize[1, totalSensors];
            c.NumberFormat = "#,##0";
            c = (Excel.Range)ws2.Cells[firstRow - 1, 3];
            c = c.Resize[1, totalSensors];
            c.NumberFormat = "#,##0";
            xlsWindow.Activate();
            releaseObject(ws1);
            releaseObject(ws2);
            releaseObject(wb);
            releaseObject(xls);
        }

        public void OutputConsumption(List<long> selectedSensors, DateTime dtStart, DateTime dtEnd)
        {
            Excel.Range c;
            int percent;
            int firstRow = 4;
            int totalSensors = selectedSensors.Count;
            int totalRows;
            double consumption;
            long compositeCode;
            Dictionary<string, string> sensorInfo;
            xls = new Excel.Application();
            xls.SheetsInNewWorkbook = 1;
            wb = xls.Workbooks.Add();
            Excel.Worksheet ws = (Excel.Worksheet)wb.Worksheets[1];
            ws.Name = "Потребление";
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
            c.Value = "Объект";
            c.ColumnWidth = 50;
            c.Font.Bold = true;
            c.Interior.Color = Excel.XlRgbColor.rgbGrey;
            c.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            c = (Excel.Range)(ws.Cells[firstRow - 1, 2]);
            c.Value = "Канал";
            c.ColumnWidth = 45;
            c.Font.Bold = true;
            c.Interior.Color = Excel.XlRgbColor.rgbGrey;
            c.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            DateTime currentDate = dtStart.AddMinutes(30);
            int currentRow = firstRow;
            totalRows = (int)dtEnd.AddDays(1).Subtract(dtStart).TotalMinutes / 30;
            pb = new frmProgress();
            pb.Show();
            pb.SetProgress(1);
            c = (Excel.Range)(ws.Cells[firstRow - 1, 3]);
            c.ColumnWidth = 22;
            c.Value = "Потребление, кВт·ч";
            c.Font.Bold = true;
            c.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            c.Interior.Color = Excel.XlRgbColor.rgbGrey;
            foreach (long sensorID in selectedSensors)
            {
                sensorInfo = d.SensorInfo(sensorID);
                compositeCode = long.Parse(sensorInfo["DeviceCode"]) * 1000 +
                    long.Parse(sensorInfo["SensorCode"]);
                consumption = d.GetConsumption(long.Parse(sensorInfo["DeviceCode"]),
                                               long.Parse(sensorInfo["SensorCode"]),
                                               dtStart, dtEnd);
                ws.Cells[currentRow, 1] = sensorInfo["DeviceName"];
                ws.Cells[currentRow, 2] = sensorInfo["SensorName"];
                ws.Cells[currentRow, 3] = consumption;             
                currentRow++;                
                percent = 100 * (currentRow - firstRow) / totalSensors;
                pb.SetProgress(percent);
            }
            pb.Close();
            ws.UsedRange.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            c = (Excel.Range)ws.Cells[firstRow, 3];
            c = c.Resize[totalSensors,1];
            c.NumberFormat = "#,##0";
            c.Font.Bold = true;
            xls.Visible = true;
            c = (Excel.Range)ws.Cells[firstRow, 3];
            c.Select();
            Excel.Windows xlsWindows = wb.Windows;
            Excel.Window xlsWindow = xlsWindows[1];
            xlsWindow.FreezePanes = true;
            xlsWindow.Activate();
            releaseObject(ws);
            releaseObject(wb);
            releaseObject(xls);
        }

        public void OutputConsumptionDaily(List<long> selectedSensors, DateTime dtStart, DateTime dtEnd)
        {
            Excel.Range c;
            int percent;
            int firstRow = 4;
            int totalSensors = selectedSensors.Count;
            int totalRows;
            int totalData;
            int completed = 0;
            Dictionary<string, string> sensorInfo;
            double consumption;
            xls = new Excel.Application();
            xls.SheetsInNewWorkbook = 1;
            wb = xls.Workbooks.Add();
            Excel.Worksheet ws = (Excel.Worksheet)wb.Worksheets[1];
            ws.Name = "Потребление";
            c = (Excel.Range)(ws.Cells[1, 1]);
            c.Value =  string.Format("Период с {0}",dtStart.ToShortDateString());
            c.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
            c = (Excel.Range)(ws.Cells[2, 1]);
            c.Value = string.Format("по {0}",dtEnd.ToShortDateString());
            c.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;

            c = (Excel.Range)(ws.Cells[firstRow - 1, 1]);
            c.Value = "Дата";
            c.Font.Bold = true;
            c.ColumnWidth = 20;
            c.Interior.Color = Excel.XlRgbColor.rgbGrey;
            c.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            DateTime currentDate = dtStart.AddMinutes(30);
            int currentRow = firstRow;
            int currentColumn = 2;
            totalRows = (int)dtEnd.AddDays(1).Subtract(dtStart).TotalDays;
            totalData = totalRows * totalSensors;
            pb = new frmProgress();
            pb.Show();
            pb.SetProgress(1);
            while (currentDate <= dtEnd.AddDays(1))
            {
                ws.Cells[currentRow, 1] = currentDate.ToShortDateString();
                currentDate = currentDate.AddDays(1);
                currentRow++;
            }            
            currentColumn = 2;
            foreach (long sensorID in selectedSensors)
            {
                sensorInfo = d.SensorInfo(sensorID);
                currentDate = dtStart;
                currentRow = firstRow;
                while (currentDate <= dtEnd)
                {
                    consumption = d.GetDayConsumption(long.Parse(sensorInfo["DeviceCode"]),
                               long.Parse(sensorInfo["SensorCode"]), currentDate);
                    if (currentRow == firstRow)
                    {
                        c = (Excel.Range)(ws.Cells[1, currentColumn]);
                        c.ColumnWidth = 18;
                        c.Value = sensorInfo["DeviceName"];
                        ws.Cells[2, currentColumn] = sensorInfo["SensorName"];
                        c = (Excel.Range)(ws.Cells[firstRow - 1, currentColumn]);
                        c.FormulaR1C1 = string.Format("=SUM(R[1]C:R[{0}]C)", totalRows);
                        c.Font.Bold = true;
                        c.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                        c.Interior.Color = Excel.XlRgbColor.rgbGrey;
                    }
                    ws.Cells[currentRow, currentColumn] = consumption;
                    currentRow++;
                    completed++;
                    currentDate = currentDate.AddDays(1);
                    percent = 100 * completed / totalData;
                    pb.SetProgress(percent);
                }
                currentColumn++;
            }
            pb.Close();
            ws.UsedRange.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            xls.Visible = true;
            c = (Excel.Range)ws.Cells[firstRow, 2];
            c.Select();
            Excel.Windows xlsWindows = wb.Windows;
            Excel.Window xlsWindow = xlsWindows[1];
            xlsWindow.FreezePanes = true;
            c = (Excel.Range)ws.Cells[firstRow - 1, 2];
            c = c.Resize[totalRows + 1, totalSensors];
            c.NumberFormat = "#,##0.00";
            xlsWindow.Activate();
            releaseObject(ws);
            releaseObject(wb);
            releaseObject(xls);        
    }

        public void OutputFixed(List<long> selectedSensors, DateTime dtStart, DateTime dtEnd)
        {
            Excel.Range c;
            int percent;
            int firstRow = 4;
            int totalSensors = selectedSensors.Count;
            int totalRows;
            Dictionary<string, string> sensorInfo;
            DataSet fixedData;
            double ktr;
            xls = new Excel.Application();
            xls.SheetsInNewWorkbook = 1;
            wb = xls.Workbooks.Add();
            Excel.Worksheet ws = (Excel.Worksheet)wb.Worksheets[1];
            ws.Name = "Показания";
            c = (Excel.Range)(ws.Cells[1, 1]);
            c.Value = string.Format("Период с {0}", dtStart.ToShortDateString());
            c.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
            c = (Excel.Range)(ws.Cells[2, 1]);
            c.Value = string.Format("по {0}", dtEnd.ToShortDateString());
            c.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;

            c = (Excel.Range)(ws.Cells[firstRow - 1, 1]);
            c.Value = "Дата";
            c.Font.Bold = true;
            c.ColumnWidth = 20;
            c.Interior.Color = Excel.XlRgbColor.rgbGrey;
            c.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            DateTime currentDate = dtStart;
            int currentRow = firstRow;
            int currentColumn = 2;
            totalRows = (int)dtEnd.AddDays(1).Subtract(dtStart).TotalDays;
            pb = new frmProgress();
            pb.Show();
            pb.SetProgress(1);
            while (currentDate <= dtEnd)
            {
                c = (Excel.Range)ws.Cells[currentRow, 1];
                c.Value = currentDate.ToShortDateString();
                c.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                currentDate = currentDate.AddDays(1);
                currentRow++;
            }
            currentColumn = 2;
            foreach (long sensorID in selectedSensors)
            {
                sensorInfo = d.SensorInfo(sensorID);
                currentDate = dtStart;
                currentRow = firstRow;
                fixedData = d.GetFixedData(long.Parse(sensorInfo["DeviceCode"]),
                               long.Parse(sensorInfo["SensorCode"]), dtStart, dtEnd);
                ktr = d.GetKTR(sensorID);
                foreach (DataRow row in fixedData.Tables[0].Rows)
                {
                    currentDate = DateTime.Parse(row["data_date"].ToString());
                    currentRow = (int)currentDate.Subtract(dtStart).TotalDays + firstRow;
                    if (currentRow == firstRow)
                    {
                        c = (Excel.Range)(ws.Cells[1, currentColumn]);
                        c.ColumnWidth = 22;
                        c.Value = sensorInfo["DeviceName"];
                        ws.Cells[2, currentColumn] = sensorInfo["SensorName"];
                        c = (Excel.Range)(ws.Cells[firstRow - 1, currentColumn]);
                        c.FormulaR1C1 = string.Format("=(R[{0}]C-R[1]C)*{1}", totalRows, ktr);
                        c.Font.Bold = true;
                        c.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                        c.Interior.Color = Excel.XlRgbColor.rgbGrey;
                    }
                    ws.Cells[currentRow, currentColumn] = row["value0"];               
                }               
                percent = 100 * (currentColumn - 1) / totalSensors;
                pb.SetProgress(percent);
                currentColumn++;
            }
            pb.Close();
            ws.UsedRange.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            xls.Visible = true;
            c = (Excel.Range)ws.Cells[firstRow, 2];
            c.Select();
            Excel.Windows xlsWindows = wb.Windows;
            Excel.Window xlsWindow = xlsWindows[1];
            xlsWindow.FreezePanes = true;
            c = (Excel.Range)ws.Cells[firstRow - 1, 2];
            c = c.Resize[totalRows + 1, totalSensors];
            c.NumberFormat = "#,##0.0000";
            xlsWindow.Activate();
            releaseObject(ws);
            releaseObject(wb);
            releaseObject(xls);
        }

        public void OutputPair(List<long> selectedSensors, DateTime dtStart, DateTime dtEnd)
        {
            Excel.Range c;
            int percent;
            int firstRow = 4;
            int totalSensors = selectedSensors.Count;
            Dictionary<string, string> sensorInfo;
            double fixedData;
            double ktr;
            xls = new Excel.Application();
            xls.SheetsInNewWorkbook = 1;
            wb = xls.Workbooks.Add();
            Excel.Worksheet ws = (Excel.Worksheet)wb.Worksheets[1];
            ws.Name = "Показания";
            c = (Excel.Range)(ws.Cells[1, 1]);
            c.Value = string.Format("Период с {0}", dtStart.ToShortDateString());
            c.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
            c = (Excel.Range)(ws.Cells[2, 1]);
            c.Value = string.Format("по {0}", dtEnd.ToShortDateString());
            c.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;

            #region Column headers
            c = (Excel.Range)(ws.Cells[firstRow - 1, 1]);
            c.Value = "Объект";
            c.Font.Bold = true;
            c.ColumnWidth = 40;
            c.Interior.Color = Excel.XlRgbColor.rgbGrey;
            c.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

            c = (Excel.Range)(ws.Cells[firstRow - 1, 2]);
            c.Value = "Канал учёта";
            c.Font.Bold = true;
            c.ColumnWidth = 40;
            c.Interior.Color = Excel.XlRgbColor.rgbGrey;
            c.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

            c = (Excel.Range)(ws.Cells[firstRow - 1, 3]);
            c.Value = dtStart.ToShortDateString();
            c.Font.Bold = true;
            c.ColumnWidth = 15;
            c.Interior.Color = Excel.XlRgbColor.rgbGrey;
            c.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

            c = (Excel.Range)(ws.Cells[firstRow - 1, 4]);
            c.Value = dtEnd.ToShortDateString();
            c.Font.Bold = true;
            c.ColumnWidth = 15;
            c.Interior.Color = Excel.XlRgbColor.rgbGrey;
            c.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

            c = (Excel.Range)(ws.Cells[firstRow - 1, 5]);
            c.Value = "Разность";
            c.Font.Bold = true;
            c.ColumnWidth = 15;
            c.Interior.Color = Excel.XlRgbColor.rgbGrey;
            c.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

            c = (Excel.Range)(ws.Cells[firstRow - 1, 6]);
            c.Value = "Ктр";
            c.Font.Bold = true;
            c.ColumnWidth = 20;
            c.Interior.Color = Excel.XlRgbColor.rgbGrey;
            c.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

            c = (Excel.Range)(ws.Cells[firstRow - 1, 7]);
            c.Value = "Потребление, кВт·ч";
            c.Font.Bold = true;
            c.ColumnWidth = 20;
            c.Interior.Color = Excel.XlRgbColor.rgbGrey;
            c.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            #endregion

            int currentRow = firstRow;
            pb = new frmProgress();
            pb.Show();
            pb.SetProgress(1);
            foreach (long sensorID in selectedSensors)
            {
                sensorInfo = d.SensorInfo(sensorID);
                ws.Cells[currentRow, 1] = sensorInfo["DeviceName"];
                ws.Cells[currentRow, 2] = sensorInfo["SensorName"];
                fixedData = d.GetOneFixedData(long.Parse(sensorInfo["DeviceCode"]),
                               long.Parse(sensorInfo["SensorCode"]), dtStart);
                if (fixedData < 0)
                    ws.Cells[currentRow, 3] = "=NA()";
                else
                    ws.Cells[currentRow, 3] = fixedData;
                fixedData = d.GetOneFixedData(long.Parse(sensorInfo["DeviceCode"]),
                               long.Parse(sensorInfo["SensorCode"]), dtEnd);
                if (fixedData < 0)
                    ws.Cells[currentRow, 4] = "=NA()";
                else
                    ws.Cells[currentRow, 4] = fixedData;
                ws.Cells[currentRow, 5] = "=RC[-1]-RC[-2]";
                ktr = d.GetKTR(sensorID);
                c = (Excel.Range)ws.Cells[currentRow, 6];
                c.Value = ktr;
                c.NumberFormat = "#,##0";
                c = (Excel.Range)ws.Cells[currentRow, 7];
                c.FormulaR1C1 = "=RC[-2]*RC[-1]";
                c.NumberFormat = "#,##0.00";
                percent = 100 * (currentRow - firstRow) / totalSensors;
                pb.SetProgress(percent);
                currentRow++;
            }
            c = (Excel.Range)ws.Cells[firstRow, 3];
            c = c.Resize[totalSensors, 3];
            c.NumberFormat = "#,##0.00";
            pb.Close();
            ws.UsedRange.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            xls.Visible = true;
            c = (Excel.Range)ws.Cells[firstRow, 3];
            c.Select();
            Excel.Windows xlsWindows = wb.Windows;
            Excel.Window xlsWindow = xlsWindows[1];
            xlsWindow.FreezePanes = true;                        
            xlsWindow.Activate();
            releaseObject(ws);
            releaseObject(wb);
            releaseObject(xls);
        }

        public void OutputCompare(List<long> selectedSensors, DateTime dtStart, DateTime dtEnd)
        {
            Excel.Range c;
            Excel.FormatCondition fc;
            int percent;
            int firstRow = 4;
            int totalSensors = selectedSensors.Count;
            Dictionary<string, string> sensorInfo;
            double fixedData, ktr, consumption;            
            xls = new Excel.Application();
            xls.SheetsInNewWorkbook = 1;
            wb = xls.Workbooks.Add();
            Excel.Worksheet ws = (Excel.Worksheet)wb.Worksheets[1];
            ws.Name = "Показания";
            c = (Excel.Range)(ws.Cells[1, 1]);
            c.Value = string.Format("Период с {0}", dtStart.ToShortDateString());
            c.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
            c = (Excel.Range)(ws.Cells[2, 1]);
            c.Value = string.Format("по {0}", dtEnd.ToShortDateString());
            c.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;

            #region Column headers
            c = (Excel.Range)(ws.Cells[firstRow - 1, 1]);
            c.Value = "Объект";
            c.Font.Bold = true;
            c.ColumnWidth = 40;
            c.Interior.Color = Excel.XlRgbColor.rgbGrey;
            c.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

            c = (Excel.Range)(ws.Cells[firstRow - 1, 2]);
            c.Value = "Канал учёта";
            c.Font.Bold = true;
            c.ColumnWidth = 40;
            c.Interior.Color = Excel.XlRgbColor.rgbGrey;
            c.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

            c = (Excel.Range)(ws.Cells[firstRow - 1, 3]);
            c.Value = dtStart.ToShortDateString();
            c.Font.Bold = true;
            c.ColumnWidth = 15;
            c.Interior.Color = Excel.XlRgbColor.rgbGrey;
            c.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

            c = (Excel.Range)(ws.Cells[firstRow - 1, 4]);
            c.Value = dtEnd.ToShortDateString();
            c.Font.Bold = true;
            c.ColumnWidth = 15;
            c.Interior.Color = Excel.XlRgbColor.rgbGrey;
            c.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

            c = (Excel.Range)(ws.Cells[firstRow - 1, 5]);
            c.Value = "Потребление по показаниям, кВт·ч";
            c.WrapText = true;
            c.Font.Bold = true;
            c.ColumnWidth = 20;
            c.Interior.Color = Excel.XlRgbColor.rgbGrey;
            c.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

            c = (Excel.Range)(ws.Cells[firstRow - 1, 6]);
            c.Value = "Потребление по получасовкам, кВт·ч";
            c.WrapText = true;
            c.Font.Bold = true;
            c.ColumnWidth = 20;
            c.Interior.Color = Excel.XlRgbColor.rgbGrey;
            c.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

            c = (Excel.Range)(ws.Cells[firstRow - 1, 7]);
            c.Value = "Расхождение, %";
            c.Font.Bold = true;
            c.ColumnWidth = 20;
            c.Interior.Color = Excel.XlRgbColor.rgbGrey;
            c.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

            #endregion

            int currentRow = firstRow;
            pb = new frmProgress();
            pb.Show();
            pb.SetProgress(1);
            foreach (long sensorID in selectedSensors)
            {
                sensorInfo = d.SensorInfo(sensorID);
                ws.Cells[currentRow, 1] = sensorInfo["DeviceName"];
                ws.Cells[currentRow, 2] = sensorInfo["SensorName"];
                fixedData = d.GetOneFixedData(long.Parse(sensorInfo["DeviceCode"]),
                               long.Parse(sensorInfo["SensorCode"]), dtStart);
                if (fixedData < 0)
                    ws.Cells[currentRow, 3] = "=NA()";
                else
                    ws.Cells[currentRow, 3] = fixedData;
                fixedData = d.GetOneFixedData(long.Parse(sensorInfo["DeviceCode"]),
                               long.Parse(sensorInfo["SensorCode"]), dtEnd);
                if (fixedData < 0)
                    ws.Cells[currentRow, 4] = "=NA()";
                else
                    ws.Cells[currentRow, 4] = fixedData;                
                ktr = d.GetKTR(sensorID);
                ws.Cells[currentRow, 5] = string.Format("=(RC[-1]-RC[-2])*{0}", ktr);
                consumption = d.GetConsumption(long.Parse(sensorInfo["DeviceCode"]),
                               long.Parse(sensorInfo["SensorCode"]), dtStart, dtEnd.AddDays(-1));
                if (consumption < 0)
                    ws.Cells[currentRow, 6] = "=NA()";
                else
                    ws.Cells[currentRow, 6] = consumption;
                c = (Excel.Range)ws.Cells[currentRow, 7];
                c.FormulaR1C1 = "=IF(RC[-2] = RC[-1], 0, 100*ABS(RC[-1]-RC[-2])/RC[-1])";
                fc = (Excel.FormatCondition)c.FormatConditions.Add(Excel.XlFormatConditionType.xlCellValue,
                                                                   Excel.XlFormatConditionOperator.xlGreater, "1");
                fc.Font.Color = Excel.XlRgbColor.rgbRed;
                percent = 100 * (currentRow - firstRow) / totalSensors;
                pb.SetProgress(percent);
                currentRow++;
            }
            c = (Excel.Range)ws.Cells[firstRow, 3];
            c = c.Resize[totalSensors, 5];
            c.NumberFormat = "#,##0.00";
            pb.Close();
            ws.UsedRange.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            xls.Visible = true;
            c = (Excel.Range)ws.Cells[firstRow, 3];
            c.Select();
            Excel.Windows xlsWindows = wb.Windows;
            Excel.Window xlsWindow = xlsWindows[1];
            xlsWindow.FreezePanes = true;
            xlsWindow.Activate();
            releaseObject(ws);
            releaseObject(wb);
            releaseObject(xls);
        }

        #endregion

        #region Private methods


        private int GetRowNumber(DateTime dtStart, DateTime dtCurrent, bool halves, int offset)
        {
            int result = 0;
            TimeSpan dif = (dtCurrent - dtStart);
            result = (int)dif.TotalMinutes;
            if (halves) result = result / 30; else result = result / 60;
            result += (offset - 1);
            return result;            
        }
        */

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

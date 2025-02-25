namespace HRM.Services
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Web;
    using HRM.Models.Report;
    using OfficeOpenXml;

    public interface IExportExcelShiftStatisticService
    {
        byte[] ExportDetail(IEnumerable<ShiftStatisticDetail> shiftDetails);

        byte[] Export(IEnumerable<ShiftStatistic> shiftStatistics);
    }

    [SuppressMessage("Major Code Smell", "S109:Magic numbers should not be used", Justification = "Pending")]
    public class ExportExcelShiftStatisticService : IExportExcelShiftStatisticService
    {
        public byte[] Export(IEnumerable<ShiftStatistic> shiftStatistics)
        {
            const int FirstDataTableRow = 4;
            const int DataTableTitleRow = 3;
            const int DataTbaleTitleFirstCol = 2;
            const int SumaryTemplateRowIndex = 6;

            shiftStatistics = shiftStatistics.OrderBy(statistic => statistic.WorkDate);

            var tableTitles = shiftStatistics.GroupBy(shift => shift.WorkDate).ToArray();
            var tableShifts = shiftStatistics.GroupBy(shift => shift.Code);
            int totalCol = tableTitles.Count();
            int totalTableRow = tableShifts.Count();
            int lastTableRow = totalTableRow + FirstDataTableRow;

            string templatePath = HttpContext.Current.Server.MapPath(@"~/ExcelTemplates/ShiftStatisticTemplate.xlsx");

            using (var excelPackage = new ExcelPackage(new FileInfo(templatePath)))
            {
                ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets[1];

                //Table Titles
                int titleCol = DataTbaleTitleFirstCol;
                var titleStyle = workSheet.Cells[DataTableTitleRow, DataTbaleTitleFirstCol];

                var workDates = tableTitles.Select(x => x.Key).ToList();
                foreach (var workDate in workDates)
                {
                    var titleCell = workSheet.Cells[DataTableTitleRow, titleCol++];

                    titleStyle.Copy(titleCell);
                    titleCell.Style.Font.Color.SetColor(workDate.DayOfWeek == DayOfWeek.Saturday || workDate.DayOfWeek == DayOfWeek.Sunday ? Color.Red : Color.White);
                    titleCell.Value = workDate.ToString("dd MMM, yyyy (ddd)");
                }

                //Table Summary
                var sumaryRowStyle = workSheet.Cells[SumaryTemplateRowIndex, 1];

                sumaryRowStyle.Copy(workSheet.Cells[lastTableRow, 1]);
                for (int i = 2; i <= totalCol + 1; i++)
                {
                    sumaryRowStyle.Copy(workSheet.Cells[lastTableRow, i]);
                    workSheet.Cells[lastTableRow, i].Formula = "SUM(" + workSheet.Cells[FirstDataTableRow, i, lastTableRow - 1, i] + ")";
                }
                workSheet.Cells[SumaryTemplateRowIndex, 1, SumaryTemplateRowIndex, 2].Clear();

                //Table data
                var evenRowStyle = workSheet.Cells[FirstDataTableRow, 1];
                var oddRowStyle = workSheet.Cells[FirstDataTableRow + 1, 1];
                int rowIndex = FirstDataTableRow;
                int colIndex = 1;

                var shifts = tableShifts.Select(x => x.Key).ToList();
                foreach (var shift in shifts)
                {
                    var rowTableStyle = rowIndex % 2 != 0 ? oddRowStyle : evenRowStyle;
                    rowTableStyle.Copy(workSheet.Cells[rowIndex, colIndex]);
                    workSheet.Cells[rowIndex, colIndex++].Value = shift;

                    for (int i = 0; i < totalCol; i++)
                    {
                        rowTableStyle.Copy(workSheet.Cells[rowIndex, colIndex]);
                        var data = shiftStatistics.FirstOrDefault(shiftStatistic => shiftStatistic.Code == shift && shiftStatistic.WorkDate == tableTitles[i].Key);
                        workSheet.Cells[rowIndex, colIndex++].Value = data is null ? 0 : data.Total;
                    }

                    colIndex = 1;
                    rowIndex++;
                }

                return excelPackage.GetAsByteArray();
            }
        }

        public byte[] ExportDetail(IEnumerable<ShiftStatisticDetail> shiftDetails)
        {
            const int FirstDataTableRow = 3;
            const int TotalDataTableCol = 6;

            string templatePath = HttpContext.Current.Server.MapPath(@"~/ExcelTemplates/ShiftStatisticDetailTemplate.xlsx");

            using (var excelPackage = new ExcelPackage(new FileInfo(templatePath)))
            {
                ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets[1];

                int rowIndex = FirstDataTableRow;
                int colIndex = 1;
                int count = 1;

                var oddRowTableStyle = workSheet.Cells[FirstDataTableRow, 1, FirstDataTableRow, TotalDataTableCol];
                var evenRowTableStyle = workSheet.Cells[FirstDataTableRow + 1, 1, FirstDataTableRow + 1, TotalDataTableCol];

                foreach (var detail in shiftDetails)
                {
                    //Copy row style
                    var rowTableStyle = rowIndex % 2 != 0 ? oddRowTableStyle : evenRowTableStyle;
                    rowTableStyle.Copy(workSheet.Cells[rowIndex, 1, rowIndex, TotalDataTableCol]);

                    //Fill data
                    workSheet.Cells[rowIndex, colIndex++].Value = count;
                    workSheet.Cells[rowIndex, colIndex++].Value = detail.WorkDate;
                    workSheet.Cells[rowIndex, colIndex++].Value = detail.Nickname;
                    workSheet.Cells[rowIndex, colIndex++].Value = detail.FullName;
                    workSheet.Cells[rowIndex, colIndex++].Value = detail.Team;
                    workSheet.Cells[rowIndex, colIndex].Value = detail.Shift;

                    rowIndex++;
                    colIndex = 1;
                    count++;
                }

                return excelPackage.GetAsByteArray();
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using HRM.Models.Report.ShiftStatistic;
using Microsoft.AspNetCore.Hosting;
using OfficeOpenXml;

namespace HRM.Services.ExportExcel;

public interface IExportExcelShiftStatisticService
{
    byte[] ExportDetail(IEnumerable<ShiftStatisticDetail> shiftDetails);

    byte[] Export(IEnumerable<ShiftStatistic> shiftStatistics);
}

public class ExportExcelShiftStatisticService(IWebHostEnvironment hostingEnvironment) : IExportExcelShiftStatisticService
{
    public byte[] Export(IEnumerable<ShiftStatistic> shiftStatistics)
    {
        const int firstDataTableRow = 4;
        const int dataTableTitleRow = 3;
        const int dataTableTitleFirstCol = 2;
        const int summaryTemplateRowIndex = 6;

        shiftStatistics = shiftStatistics.OrderBy(statistic => statistic.WorkDate);

        var tableTitles = shiftStatistics.GroupBy(shift => shift.WorkDate).ToArray();
        var tableShifts = shiftStatistics.GroupBy(shift => shift.Code);
        var totalCol = tableTitles.Count();
        var totalTableRow = tableShifts.Count();
        var lastTableRow = totalTableRow + firstDataTableRow;

        var templatePath = Path.Combine(hostingEnvironment.ContentRootPath, "ExcelTemplates/ShiftStatisticTemplate.xlsx");

        using var excelPackage = new ExcelPackage(new FileInfo(templatePath));
        var workSheet = excelPackage.Workbook.Worksheets[0];

        // Table Titles
        var titleCol = dataTableTitleFirstCol;
        var titleStyle = workSheet.Cells[dataTableTitleRow, dataTableTitleFirstCol];

        var workDates = tableTitles.Select(x => x.Key).ToList();
        foreach (var workDate in workDates)
        {
            var titleCell = workSheet.Cells[dataTableTitleRow, titleCol++];

            titleStyle.Copy(titleCell);
            titleCell.Style.Font.Color.SetColor(workDate.DayOfWeek == DayOfWeek.Saturday || workDate.DayOfWeek == DayOfWeek.Sunday ? Color.Red : Color.White);
            titleCell.Value = workDate.ToString("dd MMM, yyyy (ddd)");
        }

        // Table Summary
        var summaryRowStyle = workSheet.Cells[summaryTemplateRowIndex, 1];

        summaryRowStyle.Copy(workSheet.Cells[lastTableRow, 1]);
        for (var i = 2; i <= totalCol + 1; i++)
        {
            summaryRowStyle.Copy(workSheet.Cells[lastTableRow, i]);
            workSheet.Cells[lastTableRow, i].Formula = "SUM(" + workSheet.Cells[firstDataTableRow, i, lastTableRow - 1, i] + ")";
        }
        workSheet.Cells[summaryTemplateRowIndex, 1, summaryTemplateRowIndex, 2].Clear();

        // Table data
        var evenRowStyle = workSheet.Cells[firstDataTableRow, 1];
        var oddRowStyle = workSheet.Cells[firstDataTableRow + 1, 1];
        var rowIndex = firstDataTableRow;
        var colIndex = 1;

        var shifts = tableShifts.Select(x => x.Key).ToList();
        foreach (var shift in shifts)
        {
            var rowTableStyle = rowIndex % 2 != 0 ? oddRowStyle : evenRowStyle;
            rowTableStyle.Copy(workSheet.Cells[rowIndex, colIndex]);
            workSheet.Cells[rowIndex, colIndex++].Value = shift;

            for (var i = 0; i < totalCol; i++)
            {
                rowTableStyle.Copy(workSheet.Cells[rowIndex, colIndex]);
                var data = shiftStatistics.FirstOrDefault(shiftStatistic => shiftStatistic.Code == shift && shiftStatistic.WorkDate == tableTitles[i].Key);
                workSheet.Cells[rowIndex, colIndex++].Value = data?.Total ?? 0;
            }

            colIndex = 1;
            rowIndex++;
        }

        return excelPackage.GetAsByteArray();
    }

    public byte[] ExportDetail(IEnumerable<ShiftStatisticDetail> shiftDetails)
    {
        const int firstDataTableRow = 3;
        const int totalDataTableCol = 6;

        var templatePath = Path.Combine(hostingEnvironment.ContentRootPath, "ExcelTemplates/ShiftStatisticDetailTemplate.xlsx");

        using var excelPackage = new ExcelPackage(new FileInfo(templatePath));
        var workSheet = excelPackage.Workbook.Worksheets[0];

        var rowIndex = firstDataTableRow;
        var colIndex = 1;
        var count = 1;

        var oddRowTableStyle = workSheet.Cells[firstDataTableRow, 1, firstDataTableRow, totalDataTableCol];
        var evenRowTableStyle = workSheet.Cells[firstDataTableRow + 1, 1, firstDataTableRow + 1, totalDataTableCol];

        foreach (var detail in shiftDetails)
        {
            //Copy row style
            var rowTableStyle = rowIndex % 2 != 0 ? oddRowTableStyle : evenRowTableStyle;
            rowTableStyle.Copy(workSheet.Cells[rowIndex, 1, rowIndex, totalDataTableCol]);

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
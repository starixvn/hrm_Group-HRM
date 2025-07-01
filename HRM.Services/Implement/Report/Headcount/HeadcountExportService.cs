using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using HRM.Core.Constants;
using HRM.Models.Report.HeadcountReport;
using HRM.Services.Interfaces.Report.Headcount;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using HRM.Core.Helpers;
using System.Globalization;

namespace HRM.Services.Implement.Report.Headcount;

public class HeadcountExportService : IHeadcountExportService
{
    private static readonly string[] monthNames = CultureInfo.InvariantCulture.DateTimeFormat.AbbreviatedMonthNames.Take(12).ToArray();

    private static readonly Color[] MonthColors = Enumerable.Range(0, 12)
        .Select(i => i % 2 == 0 ? HeadcountConstants.Colors.White : HeadcountConstants.Colors.LightGray)
        .ToArray();

    private HeadcountReportModel data;

    public byte[] Export(string templatePath, HeadcountReportModel headcountData)
    {
        this.data = headcountData;
        using var package = new ExcelPackage(new FileInfo(templatePath));
        var worksheet1 = package.Workbook.Worksheets[0];
        var worksheet2 = package.Workbook.Worksheets[1];
        var worksheet3 = package.Workbook.Worksheets[2];
        var worksheet4 = package.Workbook.Worksheets[3];

        RenderUnitSheet(worksheet1);
        RenderCompanySheet(worksheet2);
        RenderDetailSheet(worksheet3);
        RenderCountryUnitSheet(worksheet4);

        return package.GetAsByteArray();
    }

    private void RenderUnitSheet(ExcelWorksheet worksheet)
    {
        string[] headers = [HeadcountConstants.PropertyNames.Label1];
        RenderSheet(worksheet, data.ReportByUnit, headers);
    }

    private void RenderCompanySheet(ExcelWorksheet worksheet)
    {
        string[] headers = [HeadcountConstants.PropertyNames.Label1, HeadcountConstants.PropertyNames.Label2];
        RenderSheet(worksheet, data.ReportByCompany, headers);
    }

    private void RenderDetailSheet(ExcelWorksheet worksheet)
    {
        string[] headers = [HeadcountConstants.PropertyNames.Label1, HeadcountConstants.PropertyNames.Label3, HeadcountConstants.PropertyNames.Label2];
        RenderSheet(worksheet, data.ReportByDetails, headers);
    }

    private void RenderCountryUnitSheet(ExcelWorksheet worksheet)
    {
        var sheetData = data.ReportByUnitCountry;
        var units = sheetData.Select(x => x.Label1).Distinct().ToList();
        var label3s = sheetData.Select(x => x.Label3).Distinct().OrderBy(x => x).ToList();
        string[] headers = [HeadcountConstants.PropertyNames.Label1];

        for (var col = 0; col < units.Count; col++)
        {
            var unitObj = sheetData.Find(x => x.Label1 == units[col]);
            RenderHeader(worksheet, headers, unitObj, col + 3);
        }

        var startRow = HeadcountConstants.StartRow;
        for (var month = 0; month < HeadcountConstants.TotalMonths; month++)
        {
            RenderMonthHeader(worksheet, month, label3s, startRow);
            RenderMonthData(worksheet, sheetData, units, label3s, month, startRow);
            startRow += label3s.Count;
        }
    }

    private static void RenderMonthHeader(ExcelWorksheet worksheet, int month, List<string> label3s, int startRow)
    {
        var monthText = monthNames[month];
        worksheet.Cells[startRow, 1].WriteHeader(monthText);

        if (label3s.Count > 0)
        {
            worksheet.Cells[startRow, 1, startRow + label3s.Count - 1, 1].Merge = true;
        }
    }

    private static void RenderMonthData(ExcelWorksheet worksheet, List<UnitReport> sheetData, List<string> units, List<string> label3s, int month, int startRow)
    {
        var alternatingColor = MonthColors[month];
        var targetMonth = month + 1;

        for (var i = 0; i < label3s.Count; i++)
        {
            var label3 = label3s[i];
            var currentRow = startRow + i;
            worksheet.Cells[currentRow, 2].WriteDataToCell(label3, alternatingColor, true);
            if (targetMonth >= DateTimeOffset.Now.Month)
            {
                continue;
            }
            for (var j = 0; j < units.Count; j++)
            {
                var value = FindUnitReportForMonth(sheetData, units[j], label3, targetMonth);
                var reportDetail = value.Details?.FirstOrDefault(d => d.Month == targetMonth);
                var (isBold, backgroundColor) = GetCellStyle(value, alternatingColor);
                worksheet.Cells[currentRow, j + 3].WriteDataToCell(reportDetail?.CurrentHeadCount ?? 0, backgroundColor, isBold);
            }
        }
    }

    private static UnitReport FindUnitReportForMonth(List<UnitReport> sheetData, string unit, string label3, int targetMonth)
    {
        return sheetData.Find(report =>
            report.Label1 == unit &&
            report.Label3 == label3 &&
            report.Details != null &&
            report.Details.Any(d => d.Month == targetMonth)) ?? new UnitReport();
    }

    public static void RenderSheet(ExcelWorksheet worksheet, List<UnitReport> sheetData, string[] headers)
    {
        for (var col = 0; col < sheetData.Count; col++)
        {
            var value = sheetData[col];
            var colIndex = HeadcountConstants.StartColumn + col;
            RenderHeader(worksheet, headers, value, colIndex);
            RenderValue(worksheet, headers.Length + 1, value, colIndex);
        }

        MergeVerticalConsecutiveCells(worksheet, HeadcountConstants.StartColumn, headers.Length - 1);
    }

    private static void RenderHeader(ExcelWorksheet worksheet, string[] headers, UnitReport data, int colIndex)
    {
        for (var i = 0; i < headers.Length; i++)
        {
            var header = headers[i];
            var value = GetPropertyValue(data, header);
            var backgroundColor = data.IsUnitCategory ? HeadcountConstants.Colors.MidnightBlue : HeadcountConstants.Colors.LightBlue;
            worksheet.Cells[i + 1, colIndex].WriteDataToCell(value, backgroundColor, true);
            worksheet.Row(i + 1).CustomHeight = false;
        }
    }

    private static void RenderValue(ExcelWorksheet worksheet, int startRow, UnitReport unitData, int colIdx)
    {
        for (var month = 0; month < HeadcountConstants.TotalMonths; month++)
        {
            var rowIdx = month * HeadcountConstants.PropertyNumber + startRow;
            var dataByMonth = unitData.Details?.FirstOrDefault(x => x.Month == month + 1) ?? new ReportModel();
            var isBold = unitData.IsGrandTotal || unitData.IsUnitCategory;

            var alternatingColor = MonthColors[month];
            var backgroundColor = unitData.IsGrandTotal ? HeadcountConstants.Colors.GrandTotalBlue : alternatingColor;

            worksheet.Cells[rowIdx + 0, colIdx].WriteDataToCell(dataByMonth.NewHire, backgroundColor, isBold);
            worksheet.Cells[rowIdx + 1, colIdx].WriteDataToCell(dataByMonth.ProbationResign, backgroundColor, isBold);
            worksheet.Cells[rowIdx + 2, colIdx].WriteDataToCell(dataByMonth.ConfirmedResign, backgroundColor, isBold);
            worksheet.Cells[rowIdx + 3, colIdx].WriteDataToCell(dataByMonth.CurrentHeadCount, backgroundColor, isBold);
        }
    }

    public static void MergeVerticalConsecutiveCells(ExcelWorksheet worksheet, int startColumn, int nRows)
    {
        for (var row = 1; row <= nRows; row++)
        {
            var startMergeColumn = startColumn;
            for (var col = startColumn + 1; col <= worksheet.Dimension.End.Column + 1; col++)
            {
                var currentCellValue = worksheet.Cells[row, col - 1].Text;
                var nextCellValue = worksheet.Cells[row, col].Text;

                if (currentCellValue == nextCellValue &&
                    (row == 1 || worksheet.Cells[row - 1, col - 1].Text == worksheet.Cells[row - 1, col].Text))
                {
                    continue;
                }

                if (col - 1 > startMergeColumn)
                {
                    worksheet.Cells[row, startMergeColumn, row, col - 1].Merge = true;
                }
                startMergeColumn = col;
                MergeGrandTotalHeader(worksheet, col);
            }
        }
    }

    private static void MergeGrandTotalHeader(ExcelWorksheet worksheet, int colIdx)
    {
        var cellValue1 = worksheet.Cells[1, colIdx].Text;
        var cellValue2 = worksheet.Cells[2, colIdx].Text;
        var shouldMerge = cellValue1 == HeadcountConstants.GrandTotalLabel && cellValue2 == HeadcountConstants.GrandTotalLabel;

        if (shouldMerge)
        {
            worksheet.Cells[1, colIdx, 2, colIdx].Merge = true;
            worksheet.Cells[1, colIdx, 2, colIdx].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        }
    }

    private static object GetPropertyValue(object data, string propertyName)
    {
        var propertyInfo = data.GetType().GetProperty(propertyName);
        return propertyInfo?.GetValue(data);
    }

    private static (bool isBold, Color backgroundColor) GetCellStyle(UnitReport value, Color alternatingColor)
    {
        var isBold = value.IsGrandTotal || value.IsUnitCategory;
        var backgroundColor = value.IsGrandTotal
            ? HeadcountConstants.Colors.GrandTotalBlue
            : alternatingColor;
        return (isBold, backgroundColor);
    }
}

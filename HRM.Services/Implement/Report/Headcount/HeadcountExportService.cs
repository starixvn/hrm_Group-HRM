using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using HRM.Core.Constants;
using HRM.Models.Report.HeadcountReport;
using HRM.Services.Interfaces.Report.Headcount;
using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.SS.Formula.Functions;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using HRM.Core.Helpers;

namespace HRM.Services.Implement.Report.Headcount;

public class HeadcountExportService : IHeadcountExportService
{
    private HeadcountReportModel data;

    public byte[] Export(string templatePath, HeadcountReportModel data)
    {
        this.data = data;
        using var package = new ExcelPackage(new FileInfo(templatePath));
        var worksheet1 = package.Workbook.Worksheets[0];
        var worksheet2 = package.Workbook.Worksheets[1];
        var worksheet3 = package.Workbook.Worksheets[2];
        var worksheet4 = package.Workbook.Worksheets[3];

        RenderUnitSheet(worksheet1);
        RenderCompanySheet(worksheet2);
        RenderDetailSheet(worksheet3);
        RenderCompanyUnitSheet(worksheet4);

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

    private void RenderCompanyUnitSheet(ExcelWorksheet worksheet)
    {
        var sheetData = data.ReportByUnitCountry;
        var units = sheetData.Select(x => x.Label1).Distinct().ToList();
        var months = Enumerable.Range(1, 12).ToList();
        var label3s = sheetData.Select(x => x.Label3).Distinct().ToList();
        string[] headers = [HeadcountConstants.PropertyNames.Label1];

        for (var col = 0; col < units.Count; col++)
        {
            var unitObj = sheetData.FirstOrDefault(x => x.Label1 == units[col]);
            RenderHeader(worksheet, headers, unitObj, col + 3);
        }

        var startRow = 2;
        foreach (var month in months)
        {
            var monthCell = worksheet.Cells[startRow, 1];
            var monthText = new DateTime(2000, month, 1, 0, 0, 0, DateTimeKind.Unspecified).ToString("MMM");
            monthCell.WriteDataToCell(monthText, HeadcountConstants.Colors.LightBlue, true);
            worksheet.Cells[startRow, 1, startRow + label3s.Count - 1, 1].Merge = true; // Merge month cell

            for (var i = 0; i < label3s.Count; i++)
            {
                worksheet.Cells[startRow + i, 2].Value = label3s[i];
                for (var j = 0; j < units.Count; j++)
                {
                    var value = sheetData.FirstOrDefault(x =>
                        x.Label1 == units[j] &&
                        x.Label3 == label3s[i] &&
                        x.Details != null &&
                        x.Details.Any(d => d.Month == month));
                    var report = value?.Details?.FirstOrDefault(d => d.Month == month);
                    var valueToWrite = report?.CurrentHeadCount ?? 0;
                    worksheet.Cells[startRow + i, j + 3].WriteDataToCell(valueToWrite, HeadcountConstants.Colors.White, false);
                }
            }
            startRow += label3s.Count;
        }
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

            var alternatingColor = (month + 1) % 2 != 0 ? HeadcountConstants.Colors.White : HeadcountConstants.Colors.LightGray;
            var backgroundColor = unitData.IsGrandTotal ? HeadcountConstants.Colors.GrandTotalBlue : alternatingColor;

            worksheet.Cells[rowIdx + 0, colIdx].WriteDataToCell(dataByMonth.NewHire, backgroundColor, isBold);
            worksheet.Cells[rowIdx + 1, colIdx].WriteDataToCell(dataByMonth.ProbationResign, backgroundColor, isBold);
            worksheet.Cells[rowIdx + 2, colIdx].WriteDataToCell(dataByMonth.ConfirmedResign, backgroundColor, isBold);
            worksheet.Cells[rowIdx + 3, colIdx].WriteDataToCell(dataByMonth.CurrentHeadCount, backgroundColor, isBold);
        }
    }

    //public static void RenderSpecialValue(ExcelWorksheet worksheet, int startRow, UnitReport unitData, int colIdx, IEnumerable<string> properties)
    //{
    //    var nProperties = properties.Count();
    //    for (var month = 0; month < HeadcountConstants.TotalMonths; month++)
    //    {
    //        var rowIdx = month * nProperties + startRow;
    //        var dataByMonth = unitData.Details?.FirstOrDefault(x => x.Month == month + 1) ?? new ReportModel();
    //        Color backgroundColor;
    //        var isBold = unitData.IsGrandTotal || unitData.IsUnitCategory;
    //        if (unitData.IsGrandTotal)
    //        {
    //            backgroundColor = HeadcountConstants.Colors.GrandTotalBlue;
    //        }
    //        else
    //        {
    //            backgroundColor = (month + 1) % 2 != 0 ? HeadcountConstants.Colors.White : HeadcountConstants.Colors.LightGray;
    //        }

    //        for (var i = 0; i < nProperties; i++)
    //        {
    //            var property = properties.ElementAt(i);
    //            var data = GetPropertyValue(dataByMonth, property);
    //            var propertyInfo = dataByMonth.GetType().GetProperty(property);
    //            var propertyValue = propertyInfo?.GetValue(dataByMonth);

    //            WriteDataToCell(worksheet.Cells[rowIdx + i, colIdx], data, backgroundColor, isBold);
    //            WriteDataToCell(worksheet.Cells[rowIdx + i, colIdx], propertyValue, backgroundColor, isBold);
    //        }
    //    }
    //}

    public static void MergeVerticalConsecutiveCells(ExcelWorksheet worksheet, int startColumn, int nRows)
    {
        for (var row = 1; row <= nRows; row++)
        {
            var startMergeColumn = startColumn;
            for (var col = startColumn + 1; col <= worksheet.Dimension.End.Column; col++)
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
}

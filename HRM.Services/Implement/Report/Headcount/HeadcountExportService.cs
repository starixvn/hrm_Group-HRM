using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using HRM.Core.Constants;
using HRM.Models.Report.HeadcountReport;
using HRM.Services.Interfaces.Report.Headcount;
using OfficeOpenXml;
using OfficeOpenXml.Style;

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

        RenderUnitSheet(worksheet1);
        RenderCompanySheet(worksheet2);
        RenderDetailSheet(worksheet3);

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
        string[] headers = [HeadcountConstants.PropertyNames.Label1];
        RenderSheet(worksheet, data.ReportByDetails, headers);
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
            var cell = worksheet.Cells[i + 1, colIndex];
            var value = GetPropertyValue(data, header);
            var backgroundColor = data.IsUnitCategory ? HeadcountConstants.Colors.MidnightBlue : HeadcountConstants.Colors.LightBlue;
            WriteDataToCell(cell, value, backgroundColor, true);
            worksheet.Row(i + 1).CustomHeight = false;
        }
    }

    private static void RenderValue(ExcelWorksheet worksheet, int startRow, UnitReport unitData, int colIdx)
    {
        for (var month = 0; month < HeadcountConstants.TotalMonths; month++)
        {
            var rowIdx = month * HeadcountConstants.PropertyNumber + startRow;
            var dataByMonth = unitData.Details.FirstOrDefault(x => x.Month == month + 1) ?? new ReportModel();
            Color backgroundColor;
            var isBold = unitData.IsGrandTotal || unitData.IsUnitCategory;
            if (unitData.IsGrandTotal)
            {
                backgroundColor = HeadcountConstants.Colors.GrandTotalBlue;
            }
            else
            {
                backgroundColor = (month + 1) % 2 != 0 ? HeadcountConstants.Colors.White : HeadcountConstants.Colors.LightGray;
            }

            WriteDataToCell(worksheet.Cells[rowIdx + 0, colIdx], dataByMonth.NewHire, backgroundColor, isBold);
            WriteDataToCell(worksheet.Cells[rowIdx + 1, colIdx], dataByMonth.ProbationResign, backgroundColor, isBold);
            WriteDataToCell(worksheet.Cells[rowIdx + 2, colIdx], dataByMonth.ConfirmedResign, backgroundColor, isBold);
            WriteDataToCell(worksheet.Cells[rowIdx + 3, colIdx], dataByMonth.CurrentHeadCount, backgroundColor, isBold);
        }
    }

    private static void WriteDataToCell(ExcelRange cell, dynamic value, Color backgroundColor, bool isBold = false)
    {
        cell.Value = value ?? string.Empty;
        cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
        cell.Style.Border.BorderAround(ExcelBorderStyle.Thin, HeadcountConstants.Colors.DarkGray);
        cell.Style.Font.Color.SetAuto();
        cell.Style.WrapText = true;
        cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
        cell.Style.Fill.BackgroundColor.SetColor(backgroundColor);
        cell.Style.Font.Bold = isBold;
    }

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
        if (cellValue1 == HeadcountConstants.GrandTotalLabel && cellValue2 == HeadcountConstants.GrandTotalLabel)
        {
            worksheet.Cells[1, colIdx, 2, colIdx].Merge = true;
            worksheet.Cells[1, colIdx, 2, colIdx].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        }
    }

    private static object GetPropertyValue(UnitReport data, string propertyName)
    {
        var propertyInfo = data.GetType().GetProperty(propertyName);
        return propertyInfo?.GetValue(data);
    }
}

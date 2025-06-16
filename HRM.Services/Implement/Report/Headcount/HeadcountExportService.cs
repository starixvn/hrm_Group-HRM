using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using HRM.Models.Report.HeadcountReport;
using HRM.Services.Interfaces.Report.Headcount;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace HRM.Services.Implement.Report.Headcount;

public class HeadcountExportService : IHeadcountExportService
{
    private const int StartColumn = 3; // C column
    private const int PropertyNumber = 4;
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
        string[] headers = ["Unit"];
        RenderSheet(worksheet, data.ReportByUnit, headers);
    }

    private void RenderCompanySheet(ExcelWorksheet worksheet)
    {
        string[] headers = ["Country", "Company"];
        RenderSheet(worksheet, data.ReportByCompany, headers);
    }

    private void RenderDetailSheet(ExcelWorksheet worksheet)
    {
        string[] headers = ["Unit", "LocationCode", "Company"];
        RenderSheet(worksheet, data.ReportByDetails, headers);
    }

    public void RenderSheet(ExcelWorksheet worksheet, List<UnitReport> sheetData, string[] headers)
    {
        for (var col = 0; col < sheetData.Count; col++)
        {
            var value = sheetData[col];
            var colIndex = StartColumn + col;
            RenderHeader(worksheet, headers, value, colIndex);
            RenderValue(worksheet, headers.Length + 1, value, colIndex);
        }

        MergeVerticalConsecutiveCells(worksheet, StartColumn, headers.Length - 1);
    }

    private static void RenderHeader(ExcelWorksheet worksheet, string[] headers, UnitReport data, int colIndex)
    {
        for (var i = 0; i < headers.Length; i++)
        {
            var header = headers[i];
            var value = GetPropertyValue(data, header);
            worksheet.Cells[i + 1, colIndex].Value = value;
            worksheet.Cells[i + 1, colIndex].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[i + 1, colIndex].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            if (data.IsUnitCategory)
            {
                worksheet.Cells[i + 1, colIndex].Style.Font.Bold = true;
                worksheet.Cells[i + 1, colIndex].Style.Fill.BackgroundColor.SetColor(Color.MidnightBlue);
                worksheet.Cells[i + 1, colIndex].Style.Font.Color.SetAuto();
            }
            else
            {
                worksheet.Cells[i + 1, colIndex].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#a4c2f4"));
            }
        }
    }

    private static void RenderValue(ExcelWorksheet worksheet, int startRow, UnitReport unitData, int colIdx)
    {
        for (var month = 0; month < 12; month++)
        {
            var rowIdx = month * PropertyNumber + startRow;
            var dataByMonth = unitData.Details.FirstOrDefault(x => x.Month == month + 1) ?? new ReportModel();
            WriteDataToCell(worksheet, rowIdx + 0, colIdx, dataByMonth.NewHire);
            WriteDataToCell(worksheet, rowIdx + 1, colIdx, dataByMonth.ProbationResign);
            WriteDataToCell(worksheet, rowIdx + 2, colIdx, dataByMonth.ConfirmedResign);
            WriteDataToCell(worksheet, rowIdx + 3, colIdx, dataByMonth.CurrentHeadCount);

            if ((month + 1) % 2 != 0)
            {
                continue;
            }
            var backgroundColor = Color.FromArgb(217, 231, 253);
            worksheet.Cells[rowIdx, colIdx, rowIdx + 3, colIdx].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[rowIdx, colIdx, rowIdx + 3, colIdx].Style.Fill.BackgroundColor
                .SetColor(backgroundColor);
        }
    }

    private static void WriteDataToCell(ExcelWorksheet worksheet, int rowIdx, int colIdx, dynamic value)
    {
        worksheet.Cells[rowIdx, colIdx].Value = value ?? string.Empty;
        worksheet.Cells[rowIdx, colIdx].Style.Border.BorderAround(ExcelBorderStyle.Thin);
    }

    public void MergeVerticalConsecutiveCells(ExcelWorksheet worksheet, int startColumn, int nRows)
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
            }
        }
    }

    private static object GetPropertyValue(UnitReport data, string propertyName)
    {
        var propertyInfo = data.GetType().GetProperty(propertyName);
        return propertyInfo?.GetValue(data);
    }
}

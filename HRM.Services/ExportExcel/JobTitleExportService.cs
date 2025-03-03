using System.Collections.Generic;
using System.IO;
using HRM.Models;
using HRM.Resources;
using Microsoft.AspNetCore.Hosting;
using OfficeOpenXml;

namespace HRM.Services.ExportExcel;

public interface IJobTitleExportService
{
    byte[] Export(IEnumerable<LookupData> jobTitles);
}

#pragma warning disable S109 // Magic numbers should not be used
public class JobTitleExportService(IWebHostEnvironment hostingEnvironment) : IJobTitleExportService
{
    private const byte HeaderRowIndex = 2;
    private const byte BodyRowIndex = 3;
    private const byte OddRowIndex = 3;
    private const byte EvenRowIndex = 4;
    private const byte MinimumWidth = 10;
    private const byte MaximumWidth = 50;
    private const int TotalColumns = 6;

    public byte[] Export(IEnumerable<LookupData> jobTitles)
    {
        var templatePath = Path.Combine(hostingEnvironment.ContentRootPath, "ExcelTemplates/JobTitleSummary.xlsx");
        using var templatePackage = new ExcelPackage(new FileInfo(templatePath));
        using var xlPackage = new ExcelPackage();
        var templateWorksheet = templatePackage.Workbook.Worksheets[0];
        var worksheet = xlPackage.Workbook.Worksheets.Add(Labels.JobTitle);
        var rowIndex = 1;
        var colIndex = 1;

        // title & header
        templateWorksheet.Cells[rowIndex, colIndex, HeaderRowIndex, TotalColumns].Copy(worksheet.Cells[rowIndex, colIndex, HeaderRowIndex, TotalColumns]);

        // data
        rowIndex = BodyRowIndex;
        var recordIndex = 1;
        foreach (var jobTitle in jobTitles)
        {
            colIndex = 1;
            if (recordIndex % 2 != 0)
            {
                templateWorksheet.Cells[OddRowIndex, 1, OddRowIndex, TotalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns]);
            }
            else
            {
                templateWorksheet.Cells[EvenRowIndex, 1, EvenRowIndex, TotalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns]);
            }

            worksheet.Cells[rowIndex, colIndex++].Value = recordIndex++;
            worksheet.Cells[rowIndex, colIndex++].Value = jobTitle.Name;
            worksheet.Cells[rowIndex, colIndex++].Value = jobTitle.CategoryName;
            worksheet.Cells[rowIndex, colIndex++].Value = jobTitle.TotalExpat;
            worksheet.Cells[rowIndex, colIndex++].Value = jobTitle.TotalLocal;
            worksheet.Cells[rowIndex, colIndex].Value = jobTitle.Value;

            rowIndex++;
        }

        worksheet.Cells.AutoFitColumns(MinimumWidth, MaximumWidth);
        worksheet.View.FreezePanes(BodyRowIndex, 1);

        return xlPackage.GetAsByteArray();
    }
}
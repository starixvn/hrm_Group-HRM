using System.Collections.Generic;
using System.IO;
using HRM.Models;
using Microsoft.AspNetCore.Hosting;
using OfficeOpenXml;

namespace HRM.Services.ExportExcel;

public interface IExportExcelTimeRecordService
{
    byte[] Export(IEnumerable<TimeRecordModel> timeRecords);
}

public class ExportExcelTimeRecordService(IWebHostEnvironment hostingEnvironment) : IExportExcelTimeRecordService
{
    public byte[] Export(IEnumerable<TimeRecordModel> timeRecords)
    {
        const byte startBodyRow = 4;
        const byte totalColumns = 10;
        const byte headerAndTitleRow = 2;
        var templatePath = Path.Combine(hostingEnvironment.ContentRootPath, "ExcelTemplates/TimeRecordTemplate.xlsx");

        using var templatePackage = new ExcelPackage(new FileInfo(templatePath));
        using var xlPackage = new ExcelPackage();
        var templateWorksheet = templatePackage.Workbook.Worksheets[0];
        var workSheet = xlPackage.Workbook.Worksheets.Add("Time Record Report");
        var rowIndex = 1;
        var colIndex = 1;
        var count = 1;

        // title
        templateWorksheet.Cells[rowIndex, colIndex, rowIndex, totalColumns].Copy(workSheet.Cells[rowIndex, colIndex, rowIndex, totalColumns]);
        rowIndex += headerAndTitleRow;

        // header
        templateWorksheet.Cells[rowIndex, colIndex, rowIndex + 1, totalColumns].Copy(workSheet.Cells[rowIndex, colIndex, rowIndex + 1, totalColumns]);
        rowIndex++;

        foreach (var detail in timeRecords)
        {
            templateWorksheet.Cells[startBodyRow, 1, startBodyRow, totalColumns].Copy(workSheet.Cells[rowIndex, colIndex, rowIndex, totalColumns]);

            workSheet.Cells[rowIndex, colIndex++].Value = count;
            workSheet.Cells[rowIndex, colIndex++].Value = detail.DepartmentCode;
            workSheet.Cells[rowIndex, colIndex++].Value = detail.Team;
            workSheet.Cells[rowIndex, colIndex++].Value = detail.Code;
            workSheet.Cells[rowIndex, colIndex++].Value = detail.FullName;
            workSheet.Cells[rowIndex, colIndex++].Value = detail.Nickname;
            workSheet.Cells[rowIndex, colIndex++].Value = detail.RecordTime.ToString(@"ddd, dd MMM, yyyy - HH") + "h" + detail.RecordTime.ToString(@"mm\'");
            workSheet.Cells[rowIndex, colIndex++].Value = detail.InOutName;
            workSheet.Cells[rowIndex, colIndex].Value = detail.IsCheckInOutOnline ? "Yes" : "No";
            rowIndex++;
            colIndex = 1;
            count++;
        }

        workSheet.Cells.AutoFitColumns();
        return xlPackage.GetAsByteArray();
    }
}
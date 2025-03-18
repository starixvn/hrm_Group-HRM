using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Fanex.Logging;
using HRM.Core.Constants;
using HRM.Models.Overtime;
using Microsoft.AspNetCore.Hosting;
using OfficeOpenXml;

namespace HRM.Services.ExportExcel;

public interface IExportOvertimeService
{
    byte[] Export(IEnumerable<MonthlyOvertimeRequest> memberRequests, bool isMemberOvertime);
}

[SuppressMessage("Major Code Smell", "S109:Magic numbers should not be used", Justification = "Pending")]
public class ExportOvertimeService(IWebHostEnvironment hostingEnvironment) : IExportOvertimeService
{
    public byte[] Export(IEnumerable<MonthlyOvertimeRequest> memberRequests, bool isMemberOvertime)
    {
        const byte totalColumns = 14;
        const byte startBodyRow = 3;
        const byte startTotal = 8;
        const byte lineOfTotal = 4;

        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
		var sheetName = "Employee Overtime Report";
        var templatePath = Path.Combine(hostingEnvironment.ContentRootPath, "ExcelTemplates/EmployeeOvertimeTemplate.xlsx"); 
       
        if (isMemberOvertime)
        {
            sheetName = "Member Overtime Report";
        }
		using var templatePackage = new ExcelPackage(new FileInfo(templatePath));
		using var xlPackage = new ExcelPackage();
		var templateWorksheet = templatePackage.Workbook.Worksheets[0];
        var workSheet = xlPackage.Workbook.Worksheets.Add(sheetName);

        var rowIndex = 1;
        var colIndex = 1;
        var count = 1;

        // title
        templateWorksheet.Cells[rowIndex, colIndex, rowIndex, totalColumns].Copy(workSheet.Cells[rowIndex, colIndex, rowIndex, totalColumns]);
        workSheet.Cells[rowIndex, colIndex].Value = string.Format(sheetName);
        rowIndex++;

        // header
        templateWorksheet.Cells[rowIndex, colIndex, rowIndex + 1, totalColumns].Copy(workSheet.Cells[rowIndex, colIndex, rowIndex + 1, totalColumns]);
        rowIndex++;

        var monthlyOvertimeRequests = memberRequests.ToList();
        foreach (var detail in monthlyOvertimeRequests)
        {
            templateWorksheet.Cells[startBodyRow, 1, startBodyRow, totalColumns].Copy(workSheet.Cells[rowIndex, colIndex, rowIndex, totalColumns]);

            workSheet.Cells[rowIndex, colIndex++].Value = count;
            workSheet.Cells[rowIndex, colIndex++].Value = detail.Requester.Department;
            workSheet.Cells[rowIndex, colIndex++].Value = detail.Requester.Section;
            workSheet.Cells[rowIndex, colIndex++].Value = detail.Team;
            workSheet.Cells[rowIndex, colIndex++].Value = detail.FullName;
            workSheet.Cells[rowIndex, colIndex++].Value = detail.Nickname;
            workSheet.Cells[rowIndex, colIndex++].Value = detail.Month.ToString("MMM-yyyy");
            workSheet.Cells[rowIndex, colIndex++].Value = detail.NormalHours;
            workSheet.Cells[rowIndex, colIndex++].Value = detail.OffHours;
            workSheet.Cells[rowIndex, colIndex++].Value = detail.PublicHours;
            workSheet.Cells[rowIndex, colIndex++].Value = detail.SpecialHours;
            workSheet.Cells[rowIndex, colIndex++].Value = detail.Verifier.Nickname;
            workSheet.Cells[rowIndex, colIndex++].Value = detail.Approver.Nickname;
            workSheet.Cells[rowIndex, colIndex].Value = detail.Status;

            rowIndex++;
            colIndex = 1;
            count++;
        }

        // summary
        if (monthlyOvertimeRequests.Any())
        {
            templateWorksheet.Cells[lineOfTotal, 1, lineOfTotal, totalColumns].Copy(workSheet.Cells[rowIndex, 1, rowIndex, totalColumns]);
            const byte total = 3;
            for (var i = 0; i <= total; i++)
            {
                colIndex = startTotal + i;
                var excelAddress = $"{workSheet.Cells[startBodyRow, colIndex].Address}:{workSheet.Cells[rowIndex - 1, colIndex].Address}";
                workSheet.Cells[rowIndex, colIndex].Formula = string.Format(Constants.SummaryFormula, excelAddress);
            }
        }

        workSheet.Cells.AutoFitColumns();
        return xlPackage.GetAsByteArray();
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GO.UserControl.Client.Core.Session;
using HRM.Core.Constants;
using HRM.Core.Services;
using HRM.Criteria.Report;
using HRM.Models.Report;
using Microsoft.AspNetCore.Hosting;
using OfficeOpenXml;

namespace HRM.Services.Implement.Report;

public interface IManpowerReportService
{
    Task<IEnumerable<ManpowerModel>> GetManpowerAsync(int year);

    Task<IEnumerable<ManpowerCategoryModel>> GetManpowerCategoryAsync(int year);

    byte[] ExportManpower(IEnumerable<ManpowerModel> manpowers, int year);

    byte[] ExportManpowerCategory(IEnumerable<ManpowerCategoryModel> manpowers, int year);
}

public class ManpowerReportService(
    IWebHostEnvironment env,
    IUserControlSession session,
    IBaseService baseService) : IManpowerReportService
{
    private readonly short currentCompanyId = session.Get<short>(SessionKey.CurrentCompanyId);

    public async Task<IEnumerable<ManpowerModel>> GetManpowerAsync(int year)
    {
        var criteria = new GetManpowerCriteria
        {
            CompanyId = currentCompanyId,
            Year = year
        };
        return await baseService.GetDataAsync<ManpowerModel>(criteria);
    }

    public async Task<IEnumerable<ManpowerCategoryModel>> GetManpowerCategoryAsync(int year)
    {
        var criteria = new GetManpowerCategoryCriteria
        {
            CompanyId = currentCompanyId,
            Year = year
        };
        return await baseService.GetDataAsync<ManpowerCategoryModel>(criteria);
    }

    public byte[] ExportManpower(IEnumerable<ManpowerModel> manpowers, int year)
    {
        var templatePath = Path.Combine(env.ContentRootPath, "ExcelTemplates/Manpower.xlsx");
        using var templatePackage = new ExcelPackage(new FileInfo(templatePath));
        using var xlPackage = new ExcelPackage();
        var template = templatePackage.Workbook.Worksheets[0];
        var worksheet = xlPackage.Workbook.Worksheets.Add("Manpower");
        const byte totalColumns = 55;
        const byte totalHeaderRows = 2;
        const byte startBodyRow = 4;
        const byte startSummaryRow = 5;
        var rowIndex = 1;
        var colIndex = 1;

        // title
        template.Cells[rowIndex, colIndex, rowIndex, totalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, totalColumns]);
        worksheet.Cells[rowIndex, colIndex].Value = string.Format(worksheet.Cells[rowIndex, colIndex].Value.ToString() ?? string.Empty, year);
        rowIndex++;

        // header
        template.Cells[rowIndex, colIndex, rowIndex + 1, totalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex + 1, totalColumns]);

        // data
        rowIndex += totalHeaderRows;
        var rowNum = 1;
        var manpowerModels = manpowers.ToList();
        foreach (var manpower in manpowerModels)
        {
            colIndex = 1;
            template.Cells[startBodyRow, 1, startBodyRow, totalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, totalColumns]);

            WriteRow(worksheet, rowIndex, rowNum, year, manpower);

            rowIndex++;
            rowNum++;
        }

        // summary
        template.Cells[startSummaryRow, 1, startSummaryRow, totalColumns].Copy(worksheet.Cells[rowIndex, 1, rowIndex, totalColumns]);
        WriteSummaryRow(worksheet, rowIndex, year, manpowerModels);

        worksheet.Cells.AutoFitColumns();
        worksheet.View.FreezePanes(startBodyRow, 1);
        return xlPackage.GetAsByteArray();
    }

    public byte[] ExportManpowerCategory(IEnumerable<ManpowerCategoryModel> manpowers, int year)
    {
        var templatePath = Path.Combine(env.ContentRootPath, "ExcelTemplates/ManpowerCategory.xlsx");
        using var templatePackage = new ExcelPackage(new FileInfo(templatePath));
        using var xlPackage = new ExcelPackage();
        var template = templatePackage.Workbook.Worksheets[0];
        var worksheet = xlPackage.Workbook.Worksheets.Add("Manpower Category");
        const byte totalColumns = 16;
        const byte totalHeaderRows = 2;
        const byte startBodyRow = 4;
        const byte startSummaryRow = 5;
        var rowIndex = 1;
        var colIndex = 1;

        // title
        template.Cells[rowIndex, colIndex, rowIndex, totalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, totalColumns]);
        worksheet.Cells[rowIndex, colIndex].Value = string.Format(worksheet.Cells[rowIndex, colIndex].Value.ToString(), year);
        rowIndex++;

        // header
        template.Cells[rowIndex, colIndex, rowIndex + 1, totalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex + 1, totalColumns]);

        // data
        rowIndex += totalHeaderRows;
        var rowNum = 1;
        foreach (var manpower in manpowers)
        {
            colIndex = 1;
            template.Cells[startBodyRow, 1, startBodyRow, totalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, totalColumns]);

            worksheet.Cells[rowIndex, colIndex++].Value = rowNum++;
            worksheet.Cells[rowIndex, colIndex++].Value = manpower.DepartmentName;
            worksheet.Cells[rowIndex, colIndex++].Value = manpower.TeamName;
            worksheet.Cells[rowIndex, colIndex++].Value = manpower.TotalExpat;
            worksheet.Cells[rowIndex, colIndex++].Value = manpower.TotalLocal;
            worksheet.Cells[rowIndex, colIndex++].Value = manpower.Total;
            worksheet.Cells[rowIndex, colIndex++].Value = manpower.Management;
            worksheet.Cells[rowIndex, colIndex++].Value = manpower.AVGManagement;
            worksheet.Cells[rowIndex, colIndex++].Value = manpower.ExecutiveWithPosition;
            worksheet.Cells[rowIndex, colIndex++].Value = manpower.AVGExecutiveWithPosition;
            worksheet.Cells[rowIndex, colIndex++].Value = manpower.Executive;
            worksheet.Cells[rowIndex, colIndex++].Value = manpower.AVGExecutive;
            worksheet.Cells[rowIndex, colIndex++].Value = manpower.Staff;
            worksheet.Cells[rowIndex, colIndex++].Value = manpower.AVGStaff;
            worksheet.Cells[rowIndex, colIndex++].Value = manpower.Worker;
            worksheet.Cells[rowIndex, colIndex].Value = manpower.AVGWorker;

            rowIndex++;
        }

        // summary
        template.Cells[startSummaryRow, 1, startSummaryRow, totalColumns].Copy(worksheet.Cells[rowIndex, 1, rowIndex, totalColumns]);
        WriteSummaryRow(worksheet, rowIndex, manpowers);

        worksheet.Cells.AutoFitColumns();
        worksheet.View.FreezePanes(startBodyRow, 1);
        return xlPackage.GetAsByteArray();
    }

    private static dynamic GetCellValue(int value, int year, int month)
    {
        dynamic result = "-";
        var today = DateTimeOffset.Now;
        if (today.Year > year || (today.Year == year && today.Month >= month))
        {
            result = value;
        }

        return result;
    }

    private static void WriteRow(
        ExcelWorksheet worksheet,
        int rowIndex,
        int rowNum,
        int year,
        ManpowerModel manpower)
    {
        var colIndex = 1;
        var month = 1;
        worksheet.Cells[rowIndex, colIndex++].Value = rowNum;
        worksheet.Cells[rowIndex, colIndex++].Value = manpower.DepartmentName;
        worksheet.Cells[rowIndex, colIndex++].Value = manpower.TeamName;
        worksheet.Cells[rowIndex, colIndex++].Value = manpower.ProbationResigned;
        worksheet.Cells[rowIndex, colIndex++].Value = manpower.ConfirmedResigned;
        worksheet.Cells[rowIndex, colIndex++].Value = manpower.NewHire;
        worksheet.Cells[rowIndex, colIndex++].Value = manpower.ActiveUser;

        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpower.ProbationResignedJan, year, month);
        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpower.ConfirmedResignedJan, year, month);
        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpower.NewHireJan, year, month);
        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpower.ActiveUserJan, year, month++);

        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpower.ProbationResignedFeb, year, month);
        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpower.ConfirmedResignedFeb, year, month);
        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpower.NewHireFeb, year, month);
        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpower.ActiveUserFeb, year, month++);

        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpower.ProbationResignedMar, year, month);
        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpower.ConfirmedResignedMar, year, month);
        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpower.NewHireMar, year, month);
        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpower.ActiveUserMar, year, month++);

        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpower.ProbationResignedApr, year, month);
        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpower.ConfirmedResignedApr, year, month);
        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpower.NewHireApr, year, month);
        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpower.ActiveUserApr, year, month++);

        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpower.ProbationResignedMay, year, month);
        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpower.ConfirmedResignedMay, year, month);
        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpower.NewHireMay, year, month);
        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpower.ActiveUserMay, year, month++);

        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpower.ProbationResignedJun, year, month);
        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpower.ConfirmedResignedJun, year, month);
        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpower.NewHireJun, year, month);
        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpower.ActiveUserJun, year, month++);

        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpower.ProbationResignedJul, year, month);
        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpower.ConfirmedResignedJul, year, month);
        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpower.NewHireJul, year, month);
        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpower.ActiveUserJul, year, month++);

        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpower.ProbationResignedAug, year, month);
        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpower.ConfirmedResignedAug, year, month);
        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpower.NewHireAug, year, month);
        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpower.ActiveUserAug, year, month++);

        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpower.ProbationResignedSep, year, month);
        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpower.ConfirmedResignedSep, year, month);
        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpower.NewHireSep, year, month);
        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpower.ActiveUserSep, year, month++);

        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpower.ProbationResignedOct, year, month);
        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpower.ConfirmedResignedOct, year, month);
        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpower.NewHireOct, year, month);
        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpower.ActiveUserOct, year, month++);

        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpower.ProbationResignedNov, year, month);
        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpower.ConfirmedResignedNov, year, month);
        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpower.NewHireNov, year, month);
        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpower.ActiveUserNov, year, month++);

        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpower.ProbationResignedDec, year, month);
        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpower.ConfirmedResignedDec, year, month);
        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpower.NewHireDec, year, month);
        worksheet.Cells[rowIndex, colIndex].Value = GetCellValue(manpower.ActiveUserDec, year, month);
    }

    private static void WriteSummaryRow(ExcelWorksheet worksheet, int rowIndex, int year, IEnumerable<ManpowerModel> manpowers)
    {
        var month = 1;
        var colIndex = 4;
        worksheet.Cells[rowIndex, colIndex++].Value = manpowers.Sum(t => t.ProbationResigned);
        worksheet.Cells[rowIndex, colIndex++].Value = manpowers.Sum(t => t.ConfirmedResigned);
        worksheet.Cells[rowIndex, colIndex++].Value = manpowers.Sum(t => t.NewHire);
        worksheet.Cells[rowIndex, colIndex++].Value = manpowers.Sum(t => t.ActiveUser);

        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpowers.Sum(t => t.ProbationResignedJan), year, month);
        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpowers.Sum(t => t.ConfirmedResignedJan), year, month);
        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpowers.Sum(t => t.NewHireJan), year, month);
        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpowers.Sum(t => t.ActiveUserJan), year, month++);

        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpowers.Sum(t => t.ProbationResignedFeb), year, month);
        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpowers.Sum(t => t.ConfirmedResignedFeb), year, month);
        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpowers.Sum(t => t.NewHireFeb), year, month);
        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpowers.Sum(t => t.ActiveUserFeb), year, month++);

        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpowers.Sum(t => t.ProbationResignedMar), year, month);
        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpowers.Sum(t => t.ConfirmedResignedMar), year, month);
        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpowers.Sum(t => t.NewHireMar), year, month);
        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpowers.Sum(t => t.ActiveUserMar), year, month++);

        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpowers.Sum(t => t.ProbationResignedApr), year, month);
        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpowers.Sum(t => t.ConfirmedResignedApr), year, month);
        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpowers.Sum(t => t.NewHireApr), year, month);
        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpowers.Sum(t => t.ActiveUserApr), year, month++);

        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpowers.Sum(t => t.ProbationResignedMay), year, month);
        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpowers.Sum(t => t.ConfirmedResignedMay), year, month);
        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpowers.Sum(t => t.NewHireMay), year, month);
        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpowers.Sum(t => t.ActiveUserMay), year, month++);

        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpowers.Sum(t => t.ProbationResignedJun), year, month);
        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpowers.Sum(t => t.ConfirmedResignedJun), year, month);
        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpowers.Sum(t => t.NewHireJun), year, month);
        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpowers.Sum(t => t.ActiveUserJun), year, month++);

        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpowers.Sum(t => t.ProbationResignedJul), year, month);
        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpowers.Sum(t => t.ConfirmedResignedJul), year, month);
        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpowers.Sum(t => t.NewHireJul), year, month);
        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpowers.Sum(t => t.ActiveUserJul), year, month++);

        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpowers.Sum(t => t.ProbationResignedAug), year, month);
        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpowers.Sum(t => t.ConfirmedResignedAug), year, month);
        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpowers.Sum(t => t.NewHireAug), year, month);
        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpowers.Sum(t => t.ActiveUserAug), year, month++);

        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpowers.Sum(t => t.ProbationResignedSep), year, month);
        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpowers.Sum(t => t.ConfirmedResignedSep), year, month);
        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpowers.Sum(t => t.NewHireSep), year, month);
        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpowers.Sum(t => t.ActiveUserSep), year, month++);

        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpowers.Sum(t => t.ProbationResignedOct), year, month);
        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpowers.Sum(t => t.ConfirmedResignedOct), year, month);
        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpowers.Sum(t => t.NewHireOct), year, month);
        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpowers.Sum(t => t.ActiveUserOct), year, month++);

        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpowers.Sum(t => t.ProbationResignedNov), year, month);
        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpowers.Sum(t => t.ConfirmedResignedNov), year, month);
        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpowers.Sum(t => t.NewHireNov), year, month);
        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpowers.Sum(t => t.ActiveUserNov), year, month++);

        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpowers.Sum(t => t.ProbationResignedDec), year, month);
        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpowers.Sum(t => t.ConfirmedResignedDec), year, month);
        worksheet.Cells[rowIndex, colIndex++].Value = GetCellValue(manpowers.Sum(t => t.NewHireDec), year, month);
        worksheet.Cells[rowIndex, colIndex].Value = GetCellValue(manpowers.Sum(t => t.ActiveUserDec), year, month);
    }

    private static void WriteSummaryRow(ExcelWorksheet worksheet, int rowIndex, IEnumerable<ManpowerCategoryModel> manpowers)
    {
        var total = manpowers.Sum(t => t.Total);
        var management = manpowers.Sum(t => t.Management);
        var executiveWithPosition = manpowers.Sum(t => t.ExecutiveWithPosition);
        var executive = manpowers.Sum(t => t.Executive);
        var staff = manpowers.Sum(t => t.Staff);
        var worker = manpowers.Sum(t => t.Worker);

        var colIndex = 4;
        worksheet.Cells[rowIndex, colIndex++].Value = manpowers.Sum(x => x.TotalExpat);
        worksheet.Cells[rowIndex, colIndex++].Value = manpowers.Sum(x => x.TotalLocal);
        worksheet.Cells[rowIndex, colIndex++].Value = total;
        worksheet.Cells[rowIndex, colIndex++].Value = management;
        worksheet.Cells[rowIndex, colIndex++].Value = total != 0 ? management * 1.00 / total : 0;
        worksheet.Cells[rowIndex, colIndex++].Value = executiveWithPosition;
        worksheet.Cells[rowIndex, colIndex++].Value = total != 0 ? executiveWithPosition * 1.00 / total : 0;
        worksheet.Cells[rowIndex, colIndex++].Value = executive;
        worksheet.Cells[rowIndex, colIndex++].Value = total != 0 ? executive * 1.00 / total : 0;
        worksheet.Cells[rowIndex, colIndex++].Value = staff;
        worksheet.Cells[rowIndex, colIndex++].Value = total != 0 ? staff * 1.00 / total : 0;
        worksheet.Cells[rowIndex, colIndex++].Value = worker;
        worksheet.Cells[rowIndex, colIndex].Value = total != 0 ? worker * 1.00 / total : 0;
    }
}
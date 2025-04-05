namespace HRM.Services.Implement.Report;

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Core;
using Core.Constants;
using HRM.Core.Services;
using HRM.Criteria.Report.MealAllowance;
using HRM.Models.Report.MealAllowance;
using HRM.ViewModels.Report;
using Microsoft.AspNetCore.Hosting;
using OfficeOpenXml;

public interface IMealAllowanceSummaryService
{
    Task<IEnumerable<MealAllowanceSummary>> GetMealAllowanceSummaryAsync(GetMealAllowanceSummaryReportCriteria criteria);

    Task<IEnumerable<MealAllowanceDetail>> GetMealAllowanceDetailsAsync(GetMealAllowanceDetailReportCriteria criteria);

    byte[] Export(MealAllowanceReportViewModel param,
        IEnumerable<MealAllowanceSummary> summaries,
        IEnumerable<MealAllowanceDetail> details);
}

public class MealAllowanceSummaryService(
    IWebHostEnvironment env,
    IBaseService baseService) : IMealAllowanceSummaryService
{
    private const byte MinimumWidth = 10;
    private const byte MaximumWidth = 50;

    public async Task<IEnumerable<MealAllowanceSummary>> GetMealAllowanceSummaryAsync(GetMealAllowanceSummaryReportCriteria criteria)
    {
        return await baseService.GetDataAsync<MealAllowanceSummary>(criteria);
    }

    public async Task<IEnumerable<MealAllowanceDetail>> GetMealAllowanceDetailsAsync(GetMealAllowanceDetailReportCriteria criteria)
    {
        var details = await baseService.GetDataAsync<MealAllowanceDetail>(criteria);
        foreach (var detail in details)
        {
            detail.InFixed = detail.InFixed.ToClientTimeZone();
            detail.OutFixed = detail.OutFixed.ToClientTimeZone();
        }

        return details;
    }

    public byte[] Export(
        MealAllowanceReportViewModel param,
        IEnumerable<MealAllowanceSummary> summaries,
        IEnumerable<MealAllowanceDetail> details)
    {
        var templatePath = Path.Combine(env.ContentRootPath, "ExcelTemplates/MealAllowanceSummary.xlsx");
        using var templatePackage = new ExcelPackage(new FileInfo(templatePath));
        using var xlPackage = new ExcelPackage();
        var template = templatePackage.Workbook.Worksheets[0];
        var worksheet = xlPackage.Workbook.Worksheets.Add("Summary");
        WriteMealAllowanceSummary(template, worksheet, param, summaries);

        const byte DetailSheetIndex = 1;
        template = templatePackage.Workbook.Worksheets[DetailSheetIndex];
        worksheet = xlPackage.Workbook.Worksheets.Add("Details");
        WriteMealAllowanceDetails(template, worksheet, param, details);

        return xlPackage.GetAsByteArray();
    }

    private static void WriteMealAllowanceSummary(
        ExcelWorksheet template,
        ExcelWorksheet worksheet,
        MealAllowanceReportViewModel param,
        IEnumerable<MealAllowanceSummary> summaries)
    {
        const byte StartBodyRow = 3;
        const byte TotalColumns = 7;
        const byte StartSumRow = 4;
        var rowIndex = 1;
        var colIndex = 1;

        // title
        template.Cells[rowIndex, colIndex, rowIndex, TotalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns]);
        worksheet.Cells[rowIndex, colIndex].Value = string.Format(
            worksheet.Cells[rowIndex, colIndex].Value.ToString(),
            param.FromDate.ToDateMonthYearFormatedDate(),
            param.ToDate.ToDateMonthYearFormatedDate());
        rowIndex++;

        // header
        template.Cells[rowIndex, colIndex, rowIndex, TotalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns]);

        // data
        rowIndex++;
        var rowNum = 1;
        foreach (var summary in summaries)
        {
            colIndex = 1;
            template.Cells[StartBodyRow, 1, StartBodyRow, TotalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns]);

            worksheet.Cells[rowIndex, colIndex++].Value = rowNum++;
            worksheet.Cells[rowIndex, colIndex++].Value = summary.DepartmentName;
            worksheet.Cells[rowIndex, colIndex++].Value = summary.TeamName;
            worksheet.Cells[rowIndex, colIndex++].Value = summary.Code;
            worksheet.Cells[rowIndex, colIndex++].Value = summary.FullName;
            worksheet.Cells[rowIndex, colIndex++].Value = summary.NickName;
            worksheet.Cells[rowIndex, colIndex].Value = summary.TotalDays;

            rowIndex++;
        }

        // total
        template.Cells[StartSumRow, 1, StartSumRow, TotalColumns].Copy(worksheet.Cells[rowIndex, 1, rowIndex, TotalColumns]);
        var excelAddress = $"{worksheet.Cells[StartBodyRow, TotalColumns].Address}:{worksheet.Cells[rowIndex - 1, TotalColumns].Address}";
        worksheet.Cells[rowIndex, TotalColumns].Formula = string.Format(Constants.SummaryFormula, excelAddress);
        worksheet.Cells.AutoFitColumns(MinimumWidth, MaximumWidth);
    }

    private static void WriteMealAllowanceDetails(
        ExcelWorksheet template,
        ExcelWorksheet worksheet,
        MealAllowanceReportViewModel param,
        IEnumerable<MealAllowanceDetail> details)
    {
        const byte StartBodyRow = 3;
        const byte TotalColumns = 12;
        const byte StartSumRow = 4;
        var rowIndex = 1;
        var colIndex = 1;

        // title
        template.Cells[rowIndex, colIndex, rowIndex, TotalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns]);
        worksheet.Cells[rowIndex, colIndex].Value = string.Format(
            worksheet.Cells[rowIndex, colIndex].Value.ToString(),
            param.FromDate.ToDateMonthYearFormatedDate(),
            param.ToDate.ToDateMonthYearFormatedDate());
        rowIndex++;

        // header
        template.Cells[rowIndex, colIndex, rowIndex, TotalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns]);

        // data
        rowIndex++;
        var rowNum = 1;
        foreach (var detail in details)
        {
            colIndex = 1;
            template.Cells[StartBodyRow, 1, StartBodyRow, TotalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns]);
            worksheet.Cells[rowIndex, colIndex++].Value = rowNum++;
            worksheet.Cells[rowIndex, colIndex++].Value = detail.DepartmentName;
            worksheet.Cells[rowIndex, colIndex++].Value = detail.TeamName;
            worksheet.Cells[rowIndex, colIndex++].Value = detail.Code;
            worksheet.Cells[rowIndex, colIndex++].Value = detail.WorkDate.ToString("ddd, dd MMM, yyyy");
            worksheet.Cells[rowIndex, colIndex++].Value = detail.FullName;
            worksheet.Cells[rowIndex, colIndex++].Value = detail.NickName;
            worksheet.Cells[rowIndex, colIndex++].Value = detail.ShiftCode;
            worksheet.Cells[rowIndex, colIndex++].Value = detail.InFixed.ToString(@"dd MMM, yyyy - HH") + "h" + detail.InFixed.ToString(@"mm\'");
            worksheet.Cells[rowIndex, colIndex++].Value = detail.OutFixed.ToString(@"dd MMM, yyyy - HH") + "h" + detail.OutFixed.ToString(@"mm\'");
            worksheet.Cells[rowIndex, colIndex++].Value = detail.TotalWorkHours;
            worksheet.Cells[rowIndex, colIndex].Value = detail.TotalDays;
            rowIndex++;
        }

        // total
        template.Cells[StartSumRow, 1, StartSumRow, TotalColumns].Copy(worksheet.Cells[rowIndex, 1, rowIndex, TotalColumns]);
        var excelAddress = $"{worksheet.Cells[StartBodyRow, TotalColumns].Address}:{worksheet.Cells[rowIndex - 1, TotalColumns].Address}";
        worksheet.Cells[rowIndex, TotalColumns].Formula = string.Format(Constants.SummaryFormula, excelAddress);

        worksheet.Cells.AutoFitColumns(MinimumWidth, MaximumWidth);
    }
}
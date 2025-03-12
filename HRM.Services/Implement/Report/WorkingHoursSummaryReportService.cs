namespace HRM.Services;

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Core;
using Core.Enums;
using HRM.Core.Services;
using Criteria.Report;
using Models.Report;
using Models.Report.Overtime;
using Microsoft.AspNetCore.Hosting;
using OfficeOpenXml;

public interface IWorkingHoursSummaryReportService
{
    Task<IEnumerable<WorkingHoursSummary>> GetWorkingHoursSummaryAsync(GetWorkingHoursSummaryParam param);

    byte[] Export(GetWorkingHoursSummaryParam param, IEnumerable<WorkingHoursSummary> workingHours, IEnumerable<DailyOvertimeDetails> overtimes);
}

public class WorkingHoursSummaryReportService(
    IWebHostEnvironment env,
    IBaseService baseService) : IWorkingHoursSummaryReportService
{
    private const byte MinimumWidth = 5;
    private const byte MaximumWidth = 50;

    public async Task<IEnumerable<WorkingHoursSummary>> GetWorkingHoursSummaryAsync(GetWorkingHoursSummaryParam param)
    {
        var criteria = new GetWorkingHoursSummaryReportCriteria
        {
            CompanyId = param.CompanyId,
            ActorId = param.ActorId,
            FromDate = param.FromDate,
            ToDate = param.ToDate,
            IsLocal = param.IsLocal,
            TeamIds = param.TeamIds,
            Employee = param.Employee,
            EmployeeStatusId = param.EmployeeStatusId
        };
        return await baseService.GetDataAsync<WorkingHoursSummary>(criteria);
    }

    public byte[] Export(
        GetWorkingHoursSummaryParam param,
        IEnumerable<WorkingHoursSummary> workingHours,
        IEnumerable<DailyOvertimeDetails> overtimes)
    {
        var templatePath = Path.Combine(env.ContentRootPath, "ExcelTemplates/WorkingHoursSummary.xlsx");
        using var templatePackage = new ExcelPackage(new FileInfo(templatePath));
        using var xlPackage = new ExcelPackage();
        var template = templatePackage.Workbook.Worksheets[0];
        var worksheet = xlPackage.Workbook.Worksheets.Add("Total Working Hours");
        WriteWorkingHours(template, worksheet, param, workingHours);

        const byte OvertimeSheetIndex = 1;
        template = templatePackage.Workbook.Worksheets[OvertimeSheetIndex];
        worksheet = xlPackage.Workbook.Worksheets.Add("Overtime Details Report");
        WriteOvertimeDetails(template, worksheet, param, overtimes);

        return xlPackage.GetAsByteArray();
    }

    private static void WriteWorkingHours(
        ExcelWorksheet template,
        ExcelWorksheet worksheet,
        GetWorkingHoursSummaryParam param,
        IEnumerable<WorkingHoursSummary> workingHours)
    {
        const byte TotalColumns = 31;
        const byte TotalHeaderRows = 2;
        const byte StartBodyRow = 4;
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
        template.Cells[rowIndex, colIndex, rowIndex + 1, TotalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex + 1, TotalColumns]);
        rowIndex += TotalHeaderRows;

        // data
        var rowNum = 1;
        foreach (var item in workingHours)
        {
            colIndex = 1;
            template.Cells[StartBodyRow, 1, StartBodyRow, TotalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns]);

            worksheet.Cells[rowIndex, colIndex++].Value = rowNum++;
            worksheet.Cells[rowIndex, colIndex++].Value = item.DepartmentCode;
            worksheet.Cells[rowIndex, colIndex++].Value = item.Section;
            worksheet.Cells[rowIndex, colIndex++].Value = item.Team;
            worksheet.Cells[rowIndex, colIndex++].Value = item.Code;
            worksheet.Cells[rowIndex, colIndex++].Value = item.FullName;
            worksheet.Cells[rowIndex, colIndex++].Value = item.Nickname;
            worksheet.Cells[rowIndex, colIndex++].Value = item.WorkingDays;
            worksheet.Cells[rowIndex, colIndex++].Value = item.WorkingHours;
            worksheet.Cells[rowIndex, colIndex++].Value = item.TotalWorkingHours;
            worksheet.Cells[rowIndex, colIndex++].Value = item.ActualWorkingDays;

            worksheet.Cells[rowIndex, colIndex++].Value = item.PaymentWorkingDays;
            worksheet.Cells[rowIndex, colIndex++].Value = item.AnnualLeave;
            worksheet.Cells[rowIndex, colIndex++].Value = item.BusinessTrip;
            worksheet.Cells[rowIndex, colIndex++].Value = item.BereavementLeave;
            worksheet.Cells[rowIndex, colIndex++].Value = item.OffsetLeave;
            worksheet.Cells[rowIndex, colIndex++].Value = item.WeddingLeave;
            worksheet.Cells[rowIndex, colIndex++].Value = item.SuspendWork;
            worksheet.Cells[rowIndex, colIndex++].Value = item.HospitalizationLeave;
            worksheet.Cells[rowIndex, colIndex++].Value = item.CompanyEvent;
            worksheet.Cells[rowIndex, colIndex++].Value = item.TotalPayments;

            worksheet.Cells[rowIndex, colIndex++].Value = item.UnpaidLeave;
            worksheet.Cells[rowIndex, colIndex++].Value = item.UnpaidLeaveInsurance;
            worksheet.Cells[rowIndex, colIndex++].Value = item.PaternityLeave;
            worksheet.Cells[rowIndex, colIndex++].Value = item.MaternityLeave;
            worksheet.Cells[rowIndex, colIndex++].Value = item.TotalUnpaidLeaves;

            worksheet.Cells[rowIndex, colIndex++].Value = item.NormalOvertimeHours;
            worksheet.Cells[rowIndex, colIndex++].Value = item.OffOvertimeHours;
            worksheet.Cells[rowIndex, colIndex++].Value = item.PublicOvertimeHours;
            worksheet.Cells[rowIndex, colIndex++].Value = item.SpecialOvertimeHours;
            worksheet.Cells[rowIndex, colIndex].Value = item.TotalOvertimes;

            rowIndex++;
        }

        const byte FreezeColumnIndex = 8;
        worksheet.Cells.AutoFitColumns(MinimumWidth, MaximumWidth);
        worksheet.View.FreezePanes(StartBodyRow, FreezeColumnIndex);
    }

    private static void WriteOvertimeDetails(
        ExcelWorksheet template,
        ExcelWorksheet worksheet,
        GetWorkingHoursSummaryParam param,
        IEnumerable<DailyOvertimeDetails> dailyOvertimes)
    {
        const byte TotalColumns = 18;
        const byte StartBodyRow = 3;
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
        rowIndex++;

        // data
        var rowNum = 1;
        foreach (var overtime in dailyOvertimes)
        {
            var status = (CurrentRequestStatus)overtime.StatusId;
            colIndex = 1;
            template.Cells[StartBodyRow, 1, StartBodyRow, TotalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns]);

            worksheet.Cells[rowIndex, colIndex++].Value = rowNum++;
            worksheet.Cells[rowIndex, colIndex++].Value = overtime.WorkDate;
            worksheet.Cells[rowIndex, colIndex++].Value = overtime.DepartmentCode;
            worksheet.Cells[rowIndex, colIndex++].Value = overtime.Section;
            worksheet.Cells[rowIndex, colIndex++].Value = overtime.Team;
            worksheet.Cells[rowIndex, colIndex++].Value = overtime.Code;
            worksheet.Cells[rowIndex, colIndex++].Value = overtime.FullName;
            worksheet.Cells[rowIndex, colIndex++].Value = overtime.Nickname;
            worksheet.Cells[rowIndex, colIndex++].Value = overtime.ShiftCode;
            worksheet.Cells[rowIndex, colIndex++].Value = overtime.TimeIn.ToClientTimeZone().ToString("HH:mm");
            worksheet.Cells[rowIndex, colIndex++].Value = overtime.TimeOut.ToClientTimeZone().ToString("HH:mm");
            worksheet.Cells[rowIndex, colIndex++].Value = overtime.ActualWorkingHours;
            worksheet.Cells[rowIndex, colIndex++].Value = overtime.NormalHours;
            worksheet.Cells[rowIndex, colIndex++].Value = overtime.OffHours;
            worksheet.Cells[rowIndex, colIndex++].Value = overtime.PublicHours;
            worksheet.Cells[rowIndex, colIndex++].Value = overtime.SpecialHours;
            worksheet.Cells[rowIndex, colIndex++].Value = overtime.Remark;
            worksheet.Cells[rowIndex, colIndex].Value = status.ToString();

            rowIndex++;
        }

        worksheet.Cells.AutoFitColumns(MinimumWidth, MaximumWidth);
        worksheet.View.FreezePanes(StartBodyRow, 1);
    }
}
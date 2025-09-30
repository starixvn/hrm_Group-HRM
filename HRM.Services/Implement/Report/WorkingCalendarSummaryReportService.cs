using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HRM.Core;
using HRM.Criteria.Report;
using HRM.Models.Report;
using HRM.Repositories;
using HRM.Services.ExportExcel;
using Microsoft.AspNetCore.Hosting;
using OfficeOpenXml;

namespace HRM.Services.Implement.Report;

public interface IWorkingCalendarSummaryReportService
{
    Task<IEnumerable<WorkingCalendarSummary>> GetWorkingCalendarSummaryAsync(GetWorkingHoursSummaryParam param);

    byte[] Export(GetWorkingHoursSummaryParam param, IEnumerable<WorkingCalendarSummary> calendars);
}

public class WorkingCalendarSummaryReportService(
    IWebHostEnvironment env,
    IWorkingCalendarSummaryReportRepository repository,
    IExportExcelService exportExcelService) : IWorkingCalendarSummaryReportService
{
    private const byte MinimumWidth = 5;
    private const byte MaximumWidth = 50;

    public async Task<IEnumerable<WorkingCalendarSummary>> GetWorkingCalendarSummaryAsync(GetWorkingHoursSummaryParam param)
    {
        var criteria = new GetEmployeeWorkingCalendarByDateCriteria
        {
            CompanyId = param.CompanyId,
            ActorId = param.ActorId,
            FromDate = param.FromDate,
            ToDate = param.ToDate,
            IsLocal = param.IsLocal,
            TeamIds = param.TeamIds,
            Employee = param.Employee,
            EmployeeStatusIds = param.EmployeeStatusIds
        };
        return await repository.GetWorkingCalendarSummaryAsync(criteria);
    }

    public byte[] Export(
        GetWorkingHoursSummaryParam param,
        IEnumerable<WorkingCalendarSummary> calendars)
    {
        var templatePath = Path.Combine(env.ContentRootPath, "ExcelTemplates/WorkingCalendarSummary.xlsx");
        using var templatePackage = new ExcelPackage(new FileInfo(templatePath));
        using var xlPackage = new ExcelPackage();
        var template = templatePackage.Workbook.Worksheets[0];
        var worksheet = xlPackage.Workbook.Worksheets.Add("Working Calendar Summary");
        const byte startBodyRow = 3;
        const byte totalFixedColumns = 6;
        const byte startDateColumn = 7;
        var rowIndex = 1;
        var colIndex = 1;

        // title
        template.Cells[rowIndex, colIndex, rowIndex, totalFixedColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, totalFixedColumns]);
        worksheet.Cells[rowIndex, colIndex].Value = string.Format(
            worksheet.Cells[rowIndex, colIndex].Value.ToString() ?? string.Empty,
            param.FromDate.ToDateMonthYearFormatedDate(),
            param.ToDate.ToDateMonthYearFormatedDate());
        rowIndex++;

        // header
        template.Cells[rowIndex, colIndex, rowIndex, totalFixedColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, totalFixedColumns]);
        colIndex = startDateColumn;
        for (var day = param.FromDate.Date; day.Date <= param.ToDate.Date; day = day.AddDays(1))
        {
            template.Cells[rowIndex, startDateColumn].Copy(worksheet.Cells[rowIndex, colIndex]);
            worksheet.Cells[rowIndex, colIndex].Value = day.ToString("dd MMM, yyyy (ddd)");
            colIndex++;
        }

        // data
        rowIndex++;
        var rowNum = 1;
        var workingCalendarSummaries = calendars.ToList();
        var employees = workingCalendarSummaries.GroupBy(x => x.EmployeeId).Select(x => x.First()).OrderBy(x => x.Nickname);
        foreach (var employee in employees)
        {
            var items = workingCalendarSummaries.Where(x => x.EmployeeId == employee.EmployeeId).ToList();
            colIndex = 1;
            var templateRowIndex = exportExcelService.GetTemplateRowIndex(startBodyRow, employee.Enabled, employee.IsDeleted);
            template.Cells[templateRowIndex, 1, templateRowIndex, totalFixedColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, totalFixedColumns]);

            worksheet.Cells[rowIndex, colIndex++].Value = rowNum++;
            worksheet.Cells[rowIndex, colIndex++].Value = employee.DepartmentName;
            worksheet.Cells[rowIndex, colIndex++].Value = employee.TeamName;
            worksheet.Cells[rowIndex, colIndex++].Value = employee.Code;
            worksheet.Cells[rowIndex, colIndex++].Value = employee.FullName;
            worksheet.Cells[rowIndex, colIndex++].Value = employee.Nickname;

            for (var day = param.FromDate.Date; day.Date <= param.ToDate.Date; day = day.AddDays(1))
            {
                var item = items.Find(x => x.WorkDate.Date == day.Date);
                template.Cells[startBodyRow, startDateColumn].Copy(worksheet.Cells[rowIndex, colIndex]);
                worksheet.Cells[rowIndex, colIndex].Value = item != null ? GetShift(item) : string.Empty;
                colIndex++;
            }

            rowIndex++;
        }

        worksheet.Cells.AutoFitColumns(MinimumWidth, MaximumWidth);
        worksheet.View.FreezePanes(startBodyRow, startDateColumn);
        return xlPackage.GetAsByteArray();
    }

    private static string GetShift(WorkingCalendarSummary workingCalendar)
    {
        string shift;

        if (workingCalendar.IsLeaveOffDay)
        {
            shift = "Off";
        }
        else if (workingCalendar.IsLeaveDay)
        {
            shift = GetShiftOnLeaveDay(workingCalendar);
        }
        else if (workingCalendar.IsHoliday)
        {
            shift = GetShiftOnHoliday(workingCalendar);
        }
        else if (workingCalendar.IsOffDay)
        {
            shift = workingCalendar.Shift != null ? workingCalendar.Shift.Code : "Off";
        }
        else
        {
            shift = workingCalendar.Shift?.Code;
        }

        return shift;
    }

    private static string GetShiftOnHoliday(WorkingCalendarSummary workingCalendar)
    {
        string shift;
        if (workingCalendar.IsOffDay)
        {
            shift = "Off";
        }
        else if (workingCalendar.Shift != null)
        {
            shift = workingCalendar.Shift.Code;
        }
        else
        {
            shift = "PH";
        }

        return shift;
    }

    private static string GetShiftOnLeaveDay(WorkingCalendarSummary workingCalendar)
    {
        string shift;
        if (workingCalendar.IsHoliday)
        {
            shift = "PH";
        }
        else if (workingCalendar.IsOffDay)
        {
            shift = "Off";
        }
        else if (workingCalendar.IsLeaveHalfDay)
        {
            shift = $"{workingCalendar.LeaveType.Code} ½";
        }
        else
        {
            shift = workingCalendar.LeaveType.Code;

            if (workingCalendar.LeaveType2 != null && workingCalendar.LeaveType.Code != workingCalendar.LeaveType2.Code)
            {
                shift = $"{workingCalendar.LeaveType.Code} / {workingCalendar.LeaveType2.Code}";
            }
        }

        return shift;
    }
}
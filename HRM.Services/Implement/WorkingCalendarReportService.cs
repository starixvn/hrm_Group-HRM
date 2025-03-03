namespace HRM.Services;

using System.Collections.Generic;
using System.IO;
using HRM.Core;
using HRM.Criteria.Calendar;
using HRM.Models.Report;
using HRM.Models.ScheduleArrange;
using Microsoft.AspNetCore.Hosting;
using OfficeOpenXml;

public interface IWorkingCalendarReportService
{
    byte[] Export(GetWorkingCalendarWeeklyCriteria param, IEnumerable<WorkingCalendar> calendars);
}

public class WorkingCalendarReportService(IWebHostEnvironment env) : IWorkingCalendarReportService
{
    private const byte MaximumWidth = 50;
    private const byte MinimumWidth = 5;

    public byte[] Export(
        GetWorkingCalendarWeeklyCriteria param,
        IEnumerable<WorkingCalendar> calendars)
    {
        var templatePath = Path.Combine(env.ContentRootPath, "ExcelTemplates/WorkingCalendar.xlsx");
        using var templatePackage = new ExcelPackage(new FileInfo(templatePath));
        using var xlPackage = new ExcelPackage();
        var template = templatePackage.Workbook.Worksheets[0];
        var worksheet = xlPackage.Workbook.Worksheets.Add("Working Calendar");
        const byte startBodyRow = 3;
        const byte totalFixedColumns = 7;
        const byte startDateColumn = 8;
        var rowIndex = 1;
        var colIndex = 1;

        // title
        template.Cells[rowIndex, colIndex, rowIndex, totalFixedColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, totalFixedColumns]);
        worksheet.Cells[rowIndex, colIndex].Value = string.Format(
            worksheet.Cells[rowIndex, colIndex].Value.ToString(),
            param.Fromtime.ToDateMonthYearFormatedDate(),
            param.Totime.ToDateMonthYearFormatedDate());
        rowIndex++;

        // header
        template.Cells[rowIndex, colIndex, rowIndex, totalFixedColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, totalFixedColumns]);
        colIndex = startDateColumn;
        for (var day = param.Fromtime.Date; day.Date <= param.Totime.Date; day = day.AddDays(1))
        {
            template.Cells[rowIndex, startDateColumn].Copy(worksheet.Cells[rowIndex, colIndex]);
            worksheet.Cells[rowIndex, colIndex].Value = day.ToString("dd MMM, yyyy (ddd)");
            colIndex++;
        }

        // data
        rowIndex++;
        var rowNum = 1;
        foreach (var employee in calendars)
        {
            colIndex = 1;
            template.Cells[startBodyRow, 1, startBodyRow, totalFixedColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, totalFixedColumns]);

            worksheet.Cells[rowIndex, colIndex++].Value = rowNum++;
            worksheet.Cells[rowIndex, colIndex++].Value = employee.DepartmentCode;
            worksheet.Cells[rowIndex, colIndex++].Value = employee.SectionCode;
            worksheet.Cells[rowIndex, colIndex++].Value = employee.Team;
            worksheet.Cells[rowIndex, colIndex++].Value = employee.Code;
            worksheet.Cells[rowIndex, colIndex++].Value = employee.FullName;
            worksheet.Cells[rowIndex, colIndex++].Value = employee.Nickname;

            foreach (var daily in employee.Schedules)
            {
                template.Cells[startBodyRow, startDateColumn].Copy(worksheet.Cells[rowIndex, colIndex]);
                worksheet.Cells[rowIndex, colIndex].Value = GetShift(daily);
                colIndex++;
            }

            rowIndex++;
        }

        worksheet.Cells.AutoFitColumns(MinimumWidth, MaximumWidth);
        worksheet.View.FreezePanes(startBodyRow, startDateColumn);

        return xlPackage.GetAsByteArray();
    }

    private static string GetShift(DailySchedule schedule)
    {
        var shift = string.Empty;

        if (schedule.IsLeaveOffDay)
        {
            shift = "Off";
        }
        else if (schedule.IsLeaveDay)
        {
            shift = GetShiftOnLeaveDay(schedule);
        }
        else if (schedule.IsHoliday)
        {
            shift = schedule.Shift != null ? schedule.Shift.Code : "PH";
        }
        else if (schedule.IsOffDay)
        {
            shift = schedule.Shift != null ? schedule.Shift.Code : "Off";
        }
        else
        {
            shift = schedule.Shift != null ? schedule.Shift.Code : string.Empty;
        }
        return shift;
    }

    private static string GetShiftOnLeaveDay(DailySchedule schedule)
    {
        var shift = string.Empty;
        if (schedule.IsHoliday)
        {
            shift = "PH";
        }
        else if (schedule.IsOffDay)
        {
            shift = "Off";
        }
        else
        {
            if (schedule.IsLeaveHalfDay)
            {
                shift = $"{schedule.LeaveType.Code} ½";
            }
            else
            {
                shift = schedule.LeaveType.Code;

                if (schedule.LeaveType2 != null && schedule.LeaveType.Code != schedule.LeaveType2.Code)
                {
                    shift = $"{schedule.LeaveType.Code} / {schedule.LeaveType2.Code}";
                }
            }
        }

        return shift;
    }
}
using System.Threading.Tasks;
using HRM.ViewModels;

namespace HRM.Services
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using HRM.Core;
    using HRM.Criteria.Report;
    using HRM.Models.Report;
    using HRM.Repositories;
    using Microsoft.AspNetCore.Hosting;
    using OfficeOpenXml;

    public interface IWorkingCalendarSummaryReportService
    {
        Task<IEnumerable<EmployeeAttendanceModel>> GetWorkingCalendarSummary(GetWorkingHoursSummaryParam param);

        byte[] Export(GetWorkingHoursSummaryParam param, IEnumerable<WorkingCalendarSummary> calendars);
    }

    public class WorkingCalendarSummaryReportService(
        IWebHostEnvironment env,
        Repositories.Interfaces.IReportRepository repository) : IWorkingCalendarSummaryReportService
    {
        private const byte MinimumWidth = 5;
        private const byte MaximumWidth = 50;

        public async Task<IEnumerable<EmployeeAttendanceModel>> GetWorkingCalendarSummary(GetWorkingHoursSummaryParam param)
        {
            var criteria = new GetEmployeeWorkingCalendarByDateCriteria
            {
                ActorId = param.ActorId,
                FromDate = param.FromDate,
                ToDate = param.ToDate,
                IsLocal = param.IsLocal,
                TeamIds = param.TeamIds,
                Employee = param.Employee
            };
            return await repository.GetEmployeeAttendanceReportAsync(criteria);
        }

        public byte[] Export(
            GetWorkingHoursSummaryParam param,
            IEnumerable<WorkingCalendarSummary> calendars)
        {
            var templatePath = Path.Combine(env.ContentRootPath, "ExcelTemplates/WorkingCalendarSummary.xlsx");
            using (var templatePackage = new ExcelPackage(new FileInfo(templatePath)))
            {
                using (var xlPackage = new ExcelPackage())
                {
                    var template = templatePackage.Workbook.Worksheets[1];
                    var worksheet = xlPackage.Workbook.Worksheets.Add("Working Calendar Summary");
                    const byte StartBodyRow = 3;
                    const byte TotalFixedColumns = 7;
                    const byte StartDateColumn = 8;
                    var rowIndex = 1;
                    var colIndex = 1;

                    // title
                    template.Cells[rowIndex, colIndex, rowIndex, TotalFixedColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, TotalFixedColumns]);
                    worksheet.Cells[rowIndex, colIndex].Value = string.Format(
                        worksheet.Cells[rowIndex, colIndex].Value.ToString(),
                        param.FromDate.ToDateMonthYearFormatedDate(),
                        param.ToDate.ToDateMonthYearFormatedDate());
                    rowIndex++;

                    // header
                    template.Cells[rowIndex, colIndex, rowIndex, TotalFixedColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, TotalFixedColumns]);
                    colIndex = StartDateColumn;
                    for (var day = param.FromDate.Date; day.Date <= param.ToDate.Date; day = day.AddDays(1))
                    {
                        template.Cells[rowIndex, StartDateColumn].Copy(worksheet.Cells[rowIndex, colIndex]);
                        worksheet.Cells[rowIndex, colIndex].Value = day.ToString("dd MMM, yyyy (ddd)");
                        colIndex++;
                    }

                    // data
                    rowIndex++;
                    var rowNum = 1;
                    var employees = calendars.GroupBy(x => x.EmployeeId).Select(x => x.First()).OrderBy(x => x.Nickname);
                    foreach (var employee in employees)
                    {
                        var items = calendars.Where(x => x.EmployeeId == employee.EmployeeId);
                        colIndex = 1;
                        template.Cells[StartBodyRow, 1, StartBodyRow, TotalFixedColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, TotalFixedColumns]);

                        worksheet.Cells[rowIndex, colIndex++].Value = rowNum++;
                        worksheet.Cells[rowIndex, colIndex++].Value = employee.DepartmentCode;
                        worksheet.Cells[rowIndex, colIndex++].Value = employee.Section;
                        worksheet.Cells[rowIndex, colIndex++].Value = employee.Team;
                        worksheet.Cells[rowIndex, colIndex++].Value = employee.Code;
                        worksheet.Cells[rowIndex, colIndex++].Value = employee.FullName;
                        worksheet.Cells[rowIndex, colIndex++].Value = employee.Nickname;

                        for (var day = param.FromDate.Date; day.Date <= param.ToDate.Date; day = day.AddDays(1))
                        {
                            var item = items.FirstOrDefault(x => x.WorkDate.Date == day.Date);
                            template.Cells[StartBodyRow, StartDateColumn].Copy(worksheet.Cells[rowIndex, colIndex]);
                            worksheet.Cells[rowIndex, colIndex].Value = item != null ? GetShift(item) : string.Empty;
                            colIndex++;
                        }

                        rowIndex++;
                    }

                    worksheet.Cells.AutoFitColumns(MinimumWidth, MaximumWidth);
                    worksheet.View.FreezePanes(StartBodyRow, StartDateColumn);
                    return xlPackage.GetAsByteArray();
                }
            }
        }

        private static string GetShift(WorkingCalendarSummary workingCalendar)
        {
            var shift = string.Empty;

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
            var shift = string.Empty;
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
            var shift = string.Empty;
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
}
namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Criteria.Report;
    using Fanex.NexHR.Models.Report;
    using Fanex.NexHR.Repositories;
    using OfficeOpenXml;

    public interface IWorkingCalendarSummaryReportService
    {
        IEnumerable<WorkingCalendarSummary> GetWorkingCalendarSummary(GetWorkingHoursSummaryParam param);

        byte[] Export(GetWorkingHoursSummaryParam param, IEnumerable<WorkingCalendarSummary> calendars);
    }

    public class WorkingCalendarSummaryReportService : IWorkingCalendarSummaryReportService
    {
        private const byte MinimumWidth = 5;
        private const byte MaximumWidth = 50;
        private readonly IReportRepository repository;

        public WorkingCalendarSummaryReportService(IReportRepository repository)
        {
            this.repository = repository;
        }

        public IEnumerable<WorkingCalendarSummary> GetWorkingCalendarSummary(GetWorkingHoursSummaryParam param)
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
            return repository.GetWorkingCalendarSummary(criteria);
        }

        public byte[] Export(
            GetWorkingHoursSummaryParam param,
            IEnumerable<WorkingCalendarSummary> calendars)
        {
            var templatePath = HttpContext.Current.Server.MapPath(@"~/ExcelTemplates/WorkingCalendarSummary.xlsx");
            using (var templatePackage = new ExcelPackage(new FileInfo(templatePath)))
            {
                using (var xlPackage = new ExcelPackage())
                {
                    ExcelWorksheet template = templatePackage.Workbook.Worksheets[1];
                    ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets.Add("Working Calendar Summary");
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
                    IEnumerable<WorkingCalendarSummary> employees = calendars.GroupBy(x => x.EmployeeId).Select(x => x.First()).OrderBy(x => x.Nickname);
                    foreach (WorkingCalendarSummary employee in employees)
                    {
                        IEnumerable<WorkingCalendarSummary> items = calendars.Where(x => x.EmployeeId == employee.EmployeeId);
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
                            WorkingCalendarSummary item = items.FirstOrDefault(x => x.WorkDate.Date == day.Date);
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
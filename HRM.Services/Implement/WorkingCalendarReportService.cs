namespace HRM.Services
{
    using System.Collections.Generic;
    using System.IO;
    using System.Web;
    using HRM.Core;
    using HRM.Criteria.Calendar;
    using HRM.Models.Report;
    using HRM.Models.ScheduleArrange;
    using OfficeOpenXml;

    public interface IWorkingCalendarReportService
    {
        byte[] Export(GetWorkingCalendarWeeklyCriteria param, IEnumerable<WorkingCalendar> calendars);
    }

    public class WorkingCalendarReportService : IWorkingCalendarReportService
    {
        private const byte MaximumWidth = 50;
        private const byte MinimumWidth = 5;

        public byte[] Export(
        GetWorkingCalendarWeeklyCriteria param,
        IEnumerable<WorkingCalendar> calendars)
        {
            var templatePath = HttpContext.Current.Server.MapPath(@"~/ExcelTemplates/WorkingCalendar.xlsx");
            using (var templatePackage = new ExcelPackage(new FileInfo(templatePath)))
            {
                using (var xlPackage = new ExcelPackage())
                {
                    ExcelWorksheet template = templatePackage.Workbook.Worksheets[1];
                    ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets.Add("Working Calendar");
                    const byte StartBodyRow = 3;
                    const byte TotalFixedColumns = 7;
                    const byte StartDateColumn = 8;
                    var rowIndex = 1;
                    var colIndex = 1;

                    // title
                    template.Cells[rowIndex, colIndex, rowIndex, TotalFixedColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, TotalFixedColumns]);
                    worksheet.Cells[rowIndex, colIndex].Value = string.Format(
                        worksheet.Cells[rowIndex, colIndex].Value.ToString(),
                        param.Fromtime.ToDateMonthYearFormatedDate(),
                        param.Totime.ToDateMonthYearFormatedDate());
                    rowIndex++;

                    // header
                    template.Cells[rowIndex, colIndex, rowIndex, TotalFixedColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, TotalFixedColumns]);
                    colIndex = StartDateColumn;
                    for (var day = param.Fromtime.Date; day.Date <= param.Totime.Date; day = day.AddDays(1))
                    {
                        template.Cells[rowIndex, StartDateColumn].Copy(worksheet.Cells[rowIndex, colIndex]);
                        worksheet.Cells[rowIndex, colIndex].Value = day.ToString("dd MMM, yyyy (ddd)");
                        colIndex++;
                    }

                    // data
                    rowIndex++;
                    var rowNum = 1;
                    foreach (WorkingCalendar employee in calendars)
                    {
                        colIndex = 1;
                        template.Cells[StartBodyRow, 1, StartBodyRow, TotalFixedColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, TotalFixedColumns]);

                        worksheet.Cells[rowIndex, colIndex++].Value = rowNum++;
                        worksheet.Cells[rowIndex, colIndex++].Value = employee.DepartmentCode;
                        worksheet.Cells[rowIndex, colIndex++].Value = employee.SectionCode;
                        worksheet.Cells[rowIndex, colIndex++].Value = employee.Team;
                        worksheet.Cells[rowIndex, colIndex++].Value = employee.Code;
                        worksheet.Cells[rowIndex, colIndex++].Value = employee.FullName;
                        worksheet.Cells[rowIndex, colIndex++].Value = employee.Nickname;

                        foreach (DailySchedule daily in employee.Schedules)
                        {
                            template.Cells[StartBodyRow, StartDateColumn].Copy(worksheet.Cells[rowIndex, colIndex]);
                            worksheet.Cells[rowIndex, colIndex].Value = GetShift(daily);
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
                    shift = string.Format("{0} ½", schedule.LeaveType.Code);
                }
                else
                {
                    shift = schedule.LeaveType.Code;

                    if (schedule.LeaveType2 != null && schedule.LeaveType.Code != schedule.LeaveType2.Code)
                    {
                        shift = string.Format("{0} / {1}", schedule.LeaveType.Code, schedule.LeaveType2.Code);
                    }
                }
            }

            return shift;
        }
    }
}
namespace HRM.Services
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using HRM.Core.Constants;
    using HRM.Core.Enums;
    using HRM.Models.Report.MonthlyWorkingHours;
    using HRM.Resources;
    using OfficeOpenXml;
    using OfficeOpenXml.Style;

    public partial interface IEmployeeWorkingHoursService
    {
        byte[] ExportWeeklyWorkingHours(DateTime fromDate, DateTime toDate, IEnumerable<MonthlyWorking> monthlyWorkings);

        byte[] ExportDailyWorkingHours(DateTime date, IEnumerable<MonthlyWorking> monthlyWorkings);
    }

    public partial class EmployeeWorkingHoursService : IEmployeeWorkingHoursService
    {
        private const string ShortDateFormat = "dd MMM yyyy";
        private readonly Color RedColor = ColorTranslator.FromHtml(Constants.RedColor);
        private int TotalColumns { get; set; }

        public byte[] ExportWeeklyWorkingHours(DateTime fromDate, DateTime toDate, IEnumerable<MonthlyWorking> monthlyWorkings)
        {
            var templatePath = Path.Combine(env.ContentRootPath, "ExcelTemplates/WeeklyWorkingHours.xlsx");
            const ushort StartDateColumn = 9;
            TotalColumns = 15;

            using (var templatePackage = new ExcelPackage(new FileInfo(templatePath)))
            {
                using (var xlPackage = new ExcelPackage())
                {
                    var redColor = ColorTranslator.FromHtml(Constants.RedColor);
                    var templateWorksheet = templatePackage.Workbook.Worksheets[1];
                    var worksheet = xlPackage.Workbook.Worksheets.Add(Labels.AttendanceWeeklyDetail);
                    var rowIndex = 1;
                    var colIndex = 1;

                    // title
                    templateWorksheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns]);
                    worksheet.Cells[rowIndex, colIndex].Value = $"{Labels.AttendanceWeeklyDetail} {fromDate.ToString(ShortDateFormat)} - {toDate.ToString(ShortDateFormat)}";
                    rowIndex++;

                    // header
                    templateWorksheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns]);
                    colIndex = StartDateColumn;
                    for (var day = fromDate.Date; day.Date <= toDate.Date; day = day.AddDays(1))
                    {
                        worksheet.Cells[rowIndex, colIndex].Value = day.ToString("dd MMM, yyyy (ddd)");

                        if (day.DayOfWeek == DayOfWeek.Saturday || day.DayOfWeek == DayOfWeek.Sunday)
                        {
                            worksheet.Cells[rowIndex, colIndex].Style.Font.Color.SetColor(redColor);
                        }

                        colIndex++;
                    }

                    // data
                    rowIndex++;
                    foreach (MonthlyWorking monthlyWorking in monthlyWorkings)
                    {
                        colIndex = 1;
                        templateWorksheet.Cells[3, 1, 3, TotalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns]);

                        worksheet.Cells[rowIndex, colIndex++].Value = rowIndex - 2;
                        worksheet.Cells[rowIndex, colIndex++].Value = monthlyWorking.Department;
                        worksheet.Cells[rowIndex, colIndex++].Value = monthlyWorking.Section;
                        worksheet.Cells[rowIndex, colIndex++].Value = monthlyWorking.Team.Name;
                        worksheet.Cells[rowIndex, colIndex++].Value = monthlyWorking.Code;
                        worksheet.Cells[rowIndex, colIndex++].Value = monthlyWorking.FullName;
                        worksheet.Cells[rowIndex, colIndex++].Value = monthlyWorking.Nickname;
                        worksheet.Cells[rowIndex, colIndex++].Value = monthlyWorking.ActualWorkingDays;
                        foreach (DailyWorking dailyWorking in monthlyWorking.DailyWorkings)
                        {
                            WriteDailyCells(worksheet.Cells[rowIndex, colIndex++], dailyWorking);
                        }

                        rowIndex++;
                    }

                    worksheet.Cells.AutoFitColumns();
                    worksheet.View.FreezePanes(3, 8);

                    return xlPackage.GetAsByteArray();
                }
            }
        }

        public byte[] ExportDailyWorkingHours(DateTime date, IEnumerable<MonthlyWorking> monthlyWorkings)
        {
            var templatePath = Path.Combine(env.ContentRootPath, "/ExcelTemplates/DailyWorkingHours.xlsx");
            TotalColumns = 15;

            using (var templatePackage = new ExcelPackage(new FileInfo(templatePath)))
            {
                using (var xlPackage = new ExcelPackage())
                {
                    var redColor = ColorTranslator.FromHtml(Constants.RedColor);
                    ExcelWorksheet templateWorksheet = templatePackage.Workbook.Worksheets[1];
                    ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets.Add(Labels.AttendanceDailyDetail);
                    var rowIndex = 1;
                    var colIndex = 1;

                    // title
                    templateWorksheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns]);
                    worksheet.Cells[rowIndex, colIndex].Value = Labels.AttendanceDailyDetail;
                    rowIndex++;

                    // header
                    templateWorksheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns]);
                    colIndex = 8;
                    worksheet.Cells[rowIndex, colIndex].Value = date.ToString("dd MMM, yyyy (ddd)");
                    if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                    {
                        worksheet.Cells[rowIndex, colIndex].Style.Font.Color.SetColor(redColor);
                    }

                    // data
                    rowIndex++;
                    foreach (MonthlyWorking monthlyWorking in monthlyWorkings)
                    {
                        colIndex = 1;
                        templateWorksheet.Cells[3, 1, 3, TotalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns]);

                        worksheet.Cells[rowIndex, colIndex++].Value = rowIndex - 2;
                        worksheet.Cells[rowIndex, colIndex++].Value = monthlyWorking.Department;
                        worksheet.Cells[rowIndex, colIndex++].Value = monthlyWorking.Section;
                        worksheet.Cells[rowIndex, colIndex++].Value = monthlyWorking.Team.Name;
                        worksheet.Cells[rowIndex, colIndex++].Value = monthlyWorking.Code;
                        worksheet.Cells[rowIndex, colIndex++].Value = monthlyWorking.FullName;
                        worksheet.Cells[rowIndex, colIndex++].Value = monthlyWorking.Nickname;
                        WriteDailyCells(worksheet.Cells[rowIndex, colIndex], monthlyWorking.DailyWorkings.First());

                        rowIndex++;
                    }

                    worksheet.Cells.AutoFitColumns();
                    worksheet.View.FreezePanes(3, 1);

                    return xlPackage.GetAsByteArray();
                }
            }
        }

        private void WriteDailyCells(ExcelRange cell, DailyWorking dailyWorking)
        {
            var value = string.Empty;
            if (dailyWorking.IsLeaveDay)
            {
                WriteLeaveCells(cell, dailyWorking);
                return;
            }

            if (dailyWorking.TotalWorkingHours > 0)
            {
                value = dailyWorking.TotalWorkingHoursToDisplay;
            }
            else if (dailyWorking.IsMissingCheckIn)
            {
                value = "Check In";
            }
            else if (dailyWorking.IsMissingCheckOut)
            {
                value = "Check Out";
            }
            else if (dailyWorking.IsAbsent)
            {
                value = "Absent";
            }
            else if (dailyWorking.IsHoliday)
            {
                value = "PH";
            }
            else if (dailyWorking.IsOffDay)
            {
                value = "OFF";
            }
            else
            {
                value = string.Empty;
            }

            cell.Value = value;
        }

        private void WriteLeaveCells(ExcelRange cell, DailyWorking dailyWorking)
        {
            var backgroundColor = string.Empty;
            var value = string.Empty;

            if (dailyWorking.IsHoliday)
            {
                value = "PH";
            }
            else if (dailyWorking.IsOffDay)
            {
                value = "OFF";
            }
            else
            {
                if (dailyWorking.RequestStatusId == (byte)CurrentRequestStatus.Pending)
                {
                    cell.Style.Font.Color.SetColor(RedColor);
                }

                if (IsUnpaidLeave(dailyWorking))
                {
                    backgroundColor = Constants.OffDayBackgroundColor;
                }

                if (dailyWorking.IsLeaveHalfDay)
                {
                    value = GetLeaveHalfDay(dailyWorking);
                }
                else
                {
                    value = dailyWorking.LeaveType.Code;
                    if (dailyWorking.LeaveType2 != null && dailyWorking.LeaveType.Id != dailyWorking.LeaveType2.Id)
                    {
                        value = string.Format("{0}½/{1}½", dailyWorking.LeaveType.Code, dailyWorking.LeaveType2.Code);
                    }
                }
            }

            cell.Value = value;
            if (!string.IsNullOrEmpty(backgroundColor))
            {
                cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                cell.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(backgroundColor));
            }
        }

        private static string GetLeaveHalfDay(DailyWorking dailyWorking)
        {
            return dailyWorking.LeaveType.Id != (byte)Core.Enums.LeaveTypes.KeepInViewByHour
                        ? string.Format("{0}½", dailyWorking.LeaveType.Code)
                        : string.Empty;
        }

        public static bool IsUnpaidLeave(DailyWorking dailyWorking)
        {
            bool isUnpaidLeave = false;

            if (dailyWorking.LeaveType.Id == (byte)Core.Enums.LeaveTypes.UnpaidLeave
                || dailyWorking.LeaveType.Id == (byte)Core.Enums.LeaveTypes.UnpaidLeaveInsurance
                || dailyWorking.LeaveType.Id == (byte)Core.Enums.LeaveTypes.MaternityLeave
                || dailyWorking.LeaveType.Id == (byte)Core.Enums.LeaveTypes.PaternityLeave)
            {
                isUnpaidLeave = true;
            }

#pragma warning disable S1067 // Expressions should not be too complex
            if (dailyWorking.LeaveType2 != null
                && (dailyWorking.LeaveType2.Id == (byte)Core.Enums.LeaveTypes.UnpaidLeave
                    || dailyWorking.LeaveType2.Id == (byte)Core.Enums.LeaveTypes.UnpaidLeaveInsurance
                    || dailyWorking.LeaveType2.Id == (byte)Core.Enums.LeaveTypes.MaternityLeave
                    || dailyWorking.LeaveType2.Id == (byte)Core.Enums.LeaveTypes.PaternityLeave))
#pragma warning restore S1067 // Expressions should not be too complex
            {
                isUnpaidLeave = true;
            }

            return isUnpaidLeave;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using HRM.Core;
using HRM.Core.Constants;
using HRM.Models;
using HRM.Models.ScheduleArrange;
using Microsoft.AspNetCore.Hosting;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace HRM.Services.ExportExcel;

public interface IExportExcelScheduleService
{
    byte[] Export(WeeklyScheduleFilter filter, IEnumerable<WeeklySchedule> weeklySchedules);

    byte[] ExportDriverPickup(Actor actor, WeeklyScheduleFilter filter, IEnumerable<WeeklySchedule> weeklySchedules);
}

[SuppressMessage("Major Code Smell", "S109:Magic numbers should not be used", Justification = "Pending")]
public class ExportExcelScheduleService(IWebHostEnvironment hostingEnvironment) : IExportExcelScheduleService
{
    private const int DefaultRowHeight = 14;
    private const string DateFormat = "dd-MMM-yyyy";
    private const string DateTimeFormat = "MMM dd, yyyy HH:mm:ss";

    public byte[] Export(WeeklyScheduleFilter filter, IEnumerable<WeeklySchedule> weeklySchedules)
    {
        using var xlPackage = new ExcelPackage();
        ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets.Add("Sheet1");
        worksheet.DefaultRowHeight = DefaultRowHeight;
        var columns = new List<string>
        {
            "ID",
            "Nickname"
        };
        for (var day = filter.FromDate.Date; day.Date <= filter.ToDate.Date; day = day.AddDays(1))
        {
            columns.Add(day.ToString("dd-MMM-yyyy"));
        }

        int totalColumns = columns.Count;
        var rowIndex = 1;
        int colIndex;
        for (var i = 0; i < totalColumns; i++)
        {
            colIndex = i + 1;
            worksheet.Cells[rowIndex, colIndex].Value = columns[i];
            worksheet.Cells[rowIndex, colIndex].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            worksheet.Cells[rowIndex, colIndex].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[rowIndex, colIndex].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(142, 169, 224));
        }
        worksheet.Row(rowIndex).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        rowIndex++;

        ShiftBase shift;
        foreach (WeeklySchedule weeklySchedule in weeklySchedules)
        {
            colIndex = 1;

            worksheet.Cells[rowIndex, colIndex++].Value = weeklySchedule.Code;
            worksheet.Cells[rowIndex, colIndex++].Value = weeklySchedule.Nickname;

            shift = GetShift(weeklySchedule.MondaySchedule);
            worksheet.Cells[rowIndex, colIndex++].Value = shift.Code;

            shift = GetShift(weeklySchedule.TuesdaySchedule);
            worksheet.Cells[rowIndex, colIndex++].Value = shift.Code;

            shift = GetShift(weeklySchedule.WednesdaySchedule);
            worksheet.Cells[rowIndex, colIndex++].Value = shift.Code;

            shift = GetShift(weeklySchedule.ThursdaySchedule);
            worksheet.Cells[rowIndex, colIndex++].Value = shift.Code;

            shift = GetShift(weeklySchedule.FridaySchedule);
            worksheet.Cells[rowIndex, colIndex++].Value = shift.Code;

            shift = GetShift(weeklySchedule.SaturdaySchedule);
            worksheet.Cells[rowIndex, colIndex++].Value = shift.Code;

            shift = GetShift(weeklySchedule.SundaySchedule);
            worksheet.Cells[rowIndex, colIndex].Value = shift.Code;

            rowIndex++;
        }

        worksheet.View.FreezePanes(2, 1);

        return xlPackage.GetAsByteArray();
    }

    public byte[] ExportDriverPickup(Actor actor, WeeklyScheduleFilter filter, IEnumerable<WeeklySchedule> weeklySchedules)
    {
        var templatePath = Path.Combine(hostingEnvironment.ContentRootPath, @"ExcelTemplates/DriverPickup.xlsx");
        const int totalColumns = 10;
        using var templatePackage = new ExcelPackage(new FileInfo(templatePath));
        using var xlPackage = new ExcelPackage();
        var sheetName = $"{filter.FromDate.ToString(DateFormat)}-{filter.ToDate.ToString(DateFormat)}";
        ExcelWorksheet templateWorksheet = templatePackage.Workbook.Worksheets[0];
        ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets.Add(sheetName);
        var rowIndex = 1;
        var colIndex = 1;

        // header
        templateWorksheet.Cells[rowIndex, colIndex, rowIndex, totalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, totalColumns]);
        worksheet.Cells[rowIndex, colIndex].Value = $"Working Shift {filter.FromDate.ToString(DateFormat)} to {filter.ToDate.ToString(DateFormat)}";
        worksheet.Row(rowIndex).Height = 30;

        rowIndex++;
        templateWorksheet.Cells[rowIndex, colIndex, rowIndex, totalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, totalColumns]);
        worksheet.Cells[rowIndex, colIndex].Value =
            $"Print Date: {DateTimeOffset.Now.ToString(DateTimeFormat)} by {actor.Nickname} ({actor.FullName}), Total Records: {weeklySchedules.Count()}";

        // sum
        rowIndex++;
        WriteSumData(worksheet, templateWorksheet, filter, weeklySchedules, totalColumns, ref rowIndex);

        // data
        rowIndex++;
        templateWorksheet.Cells[5, 1, 5, totalColumns].Copy(worksheet.Cells[rowIndex, 1, rowIndex, totalColumns]);
        worksheet.Row(rowIndex).Height = 30;
        worksheet.Column(1).Width = 20;
        worksheet.Column(2).Width = 20;
        colIndex = 3;
        for (var day = filter.FromDate.Date; day.Date <= filter.ToDate.Date; day = day.AddDays(1))
        {
            worksheet.Cells[rowIndex, colIndex].Value = $"{ day.ToString("ddd").ToUpperInvariant()}\n{day.ToString("MMM dd").ToUpperInvariant()}";
            worksheet.Column(colIndex).Width = 15;
            colIndex++;
        }
        worksheet.Column(colIndex).Width = 40;

        rowIndex++;
        foreach (WeeklySchedule weeklySchedule in weeklySchedules)
        {
            colIndex = 1;
            templateWorksheet.Cells[6, colIndex, 6, totalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, totalColumns]);
            worksheet.Cells[rowIndex, colIndex++].Value = weeklySchedule.Team?.Code;
            worksheet.Cells[rowIndex, colIndex++].Value = weeklySchedule.Nickname;

            WriteDailyCells(worksheet.Cells[rowIndex, colIndex++], weeklySchedule.MondaySchedule);
            WriteDailyCells(worksheet.Cells[rowIndex, colIndex++], weeklySchedule.TuesdaySchedule);
            WriteDailyCells(worksheet.Cells[rowIndex, colIndex++], weeklySchedule.WednesdaySchedule);
            WriteDailyCells(worksheet.Cells[rowIndex, colIndex++], weeklySchedule.ThursdaySchedule);
            WriteDailyCells(worksheet.Cells[rowIndex, colIndex++], weeklySchedule.FridaySchedule);
            WriteDailyCells(worksheet.Cells[rowIndex, colIndex++], weeklySchedule.SaturdaySchedule);
            WriteDailyCells(worksheet.Cells[rowIndex, colIndex++], weeklySchedule.SundaySchedule);

            worksheet.Cells[rowIndex, colIndex].Value = weeklySchedule.PickupRemark;

            rowIndex++;
        }

        return xlPackage.GetAsByteArray();
    }

    private static void WriteSumData(
        ExcelWorksheet worksheet,
        ExcelWorksheet templateWorksheet,
        WeeklyScheduleFilter filter,
        IEnumerable<WeeklySchedule> weeklySchedules,
        int totalColumns,
        ref int rowIndex)
    {
        var colIndex = 1;
        templateWorksheet.Cells[rowIndex, colIndex, rowIndex, totalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, totalColumns]);
        IEnumerable<SumEmployeeWorkingOnShift> employeeWorkingOnShifts = SumEmployeeWorkingOnShift(weeklySchedules);
        if (employeeWorkingOnShifts.Any())
        {
            var shiftHeaderRow = rowIndex;
            rowIndex++;
            foreach (SumEmployeeWorkingOnShift item in employeeWorkingOnShifts)
            {
                templateWorksheet.Cells[4, 1, 4, totalColumns].Copy(worksheet.Cells[rowIndex, 1, rowIndex, totalColumns]);
                worksheet.Cells[rowIndex, 1, rowIndex, 2].Value = item.Code;
                colIndex = 3;
                for (var day = filter.FromDate.Date; day.Date <= filter.ToDate.Date; day = day.AddDays(1))
                {
                    worksheet.Cells[rowIndex, colIndex].Value = item.Weekdays[(int)day.DayOfWeek];
                    colIndex++;
                }

                rowIndex++;
            }

            for (var i = 3; i <= totalColumns - 1; i++)
            {
                string totalEmployeeAddress = ExcelCellBase.GetAddress(shiftHeaderRow + 1, i, rowIndex - 1, ToColumn: i);
                worksheet.Cells[shiftHeaderRow, i].Formula = $"SUM({totalEmployeeAddress})";
                worksheet.Cells[shiftHeaderRow, i].Calculate();
            }
        }
        else
        {
            rowIndex++;
        }
    }

    private static IList<SumEmployeeWorkingOnShift> SumEmployeeWorkingOnShift(IEnumerable<WeeklySchedule> weeklySchedules)
    {
        var employeeWorkingOnShifts = new List<SumEmployeeWorkingOnShift>();
        foreach (WeeklySchedule weeklySchedule in weeklySchedules)
        {
            SumEmployeeWorkingOnShift(employeeWorkingOnShifts, weeklySchedule.MondaySchedule);
            SumEmployeeWorkingOnShift(employeeWorkingOnShifts, weeklySchedule.TuesdaySchedule);
            SumEmployeeWorkingOnShift(employeeWorkingOnShifts, weeklySchedule.WednesdaySchedule);
            SumEmployeeWorkingOnShift(employeeWorkingOnShifts, weeklySchedule.ThursdaySchedule);
            SumEmployeeWorkingOnShift(employeeWorkingOnShifts, weeklySchedule.FridaySchedule);
            SumEmployeeWorkingOnShift(employeeWorkingOnShifts, weeklySchedule.SaturdaySchedule);
            SumEmployeeWorkingOnShift(employeeWorkingOnShifts, weeklySchedule.SundaySchedule);
        }

        return employeeWorkingOnShifts.OrderBy(x => x.IsInAfterDay).ThenBy(x => x.IsOutAfterDay)
            .ThenBy(x => x.CheckIn).ThenBy(x => x.CheckOut).ToList();
    }

    [SuppressMessage("Critical Code Smell", "S1067:Expressions should not be too complex", Justification = "Pending")]
    private static void SumEmployeeWorkingOnShift(
        IList<SumEmployeeWorkingOnShift> employeeWorkingOnShifts,
        DailySchedule dailySchedule)
    {
        ShiftBase shift = GetShift(dailySchedule);
        if (shift.IsShift)
        {
            var findExists = employeeWorkingOnShifts.FirstOrDefault(
                x => x.Code == shift.Code
                     && x.CheckIn == shift.CheckIn
                     && x.CheckOut == shift.CheckOut
                     && x.IsInAfterDay == shift.IsInAfterDay
                     && x.IsOutAfterDay == shift.IsOutAfterDay
            );
            if (findExists == null)
            {
                var item = new SumEmployeeWorkingOnShift
                {
                    Code = shift.Code,
                    CheckIn = shift.CheckIn,
                    CheckOut = shift.CheckOut,
                    IsInAfterDay = shift.IsInAfterDay,
                    IsOutAfterDay = shift.IsOutAfterDay
                };
                item.Weekdays[(int)dailySchedule.WorkDate.DayOfWeek] = 1;

                employeeWorkingOnShifts.Add(item);
            }
            else
            {
                findExists.Weekdays[(int)dailySchedule.WorkDate.DayOfWeek]++;
            }
        }
    }

    private static void WriteDailyCells(ExcelRange cell, DailySchedule dailySchedule)
    {
        ShiftBase shift = GetShift(dailySchedule);

        cell.Value = shift.Code;
        if (!string.IsNullOrEmpty(shift.BackgroundColor))
        {
            var backgroundColor = ColorTranslator.FromHtml(shift.BackgroundColor);
            cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
            cell.Style.Fill.BackgroundColor.SetColor(backgroundColor);
        }
    }

    private static ShiftBase GetShift(DailySchedule dailySchedule)
    {
        var shift = new ShiftBase();
        if (dailySchedule.IsLeaveOffDay)
        {
            shift.Code = "Off";
            shift.BackgroundColor = Constants.OffDayBackgroundColor;
        }
        else if (dailySchedule.IsLeaveDay)
        {
            shift = GetShiftOnLeaveDay(dailySchedule);
        }
        else if (dailySchedule.IsHoliday)
        {
            shift = GetShiftOnHoliday(dailySchedule);
        }
        else if (dailySchedule.IsOffDay)
        {
            shift = GetShiftOnOffDay(dailySchedule);
        }
        else
        {
            shift = dailySchedule.Shift ?? new ShiftBase();
            shift.IsShift = dailySchedule.Shift != null;
        }

        return shift;
    }

    private static ShiftBase GetShiftOnOffDay(DailySchedule dailySchedule)
    {
        var shift = new ShiftBase();
        if (dailySchedule.Shift != null)
        {
            shift = dailySchedule.Shift;
            shift.IsShift = true;
        }
        else
        {
            shift.Code = "Off";
        }

        shift.BackgroundColor = Constants.OffDayBackgroundColor;

        return shift;
    }

    private static ShiftBase GetShiftOnHoliday(DailySchedule dailySchedule)
    {
        var shift = new ShiftBase();
        if (dailySchedule.IsOffDay)
        {
            shift.Code = "Off";
        }
        else if (dailySchedule.Shift != null)
        {
            shift = dailySchedule.Shift;
            shift.IsShift = true;
        }
        else
        {
            shift.Code = "PH";
        }

        shift.BackgroundColor = Constants.HolidayBackgroundColor;

        return shift;
    }

    private static ShiftBase GetShiftOnLeaveDay(DailySchedule dailySchedule)
    {
        var shift = new ShiftBase();
        if (dailySchedule.IsHoliday)
        {
            shift.Code = "PH";
            shift.BackgroundColor = Constants.HolidayBackgroundColor;
        }
        else if (dailySchedule.IsOffDay)
        {
            shift.Code = "Off";
            shift.BackgroundColor = Constants.OffDayBackgroundColor;
        }
        else if (dailySchedule.LeaveDay == Constants.HalfDayOffValue)
        {
            shift = GetShiftOnLeaveHalfDay(dailySchedule);
        }
        else
        {
            shift.Code = dailySchedule.LeaveType.Code;
            shift.BackgroundColor = Utilities.GetBackgroundColorForLeave(dailySchedule.LeaveType.Id);
        }

        return shift;
    }

    private static ShiftBase GetShiftOnLeaveHalfDay(DailySchedule dailySchedule)
    {
        var shift = new ShiftBase();
        if (dailySchedule.Shift != null)
        {
            var checkInTime = GetCheckInTime(dailySchedule.WorkDate, dailySchedule.Shift);
            var checkOutTime = GetCheckOutTime(dailySchedule.WorkDate, dailySchedule.Shift);
            if (dailySchedule.LeaveType.FromTime.Value == checkInTime)
            {
                shift.CheckIn = GetHalfShiftCode(dailySchedule.LeaveType.ToTime.Value.ToShortTime());
                shift.CheckOut = GetHalfShiftCode(dailySchedule.Shift.CheckOut);
                shift.Code = $"{shift.CheckIn} >{shift.CheckOut}";
                shift.IsInAfterDay = dailySchedule.Shift.IsInAfterDay;
                shift.IsOutAfterDay = dailySchedule.Shift.IsOutAfterDay;
                shift.IsShift = true;
            }
            else if (dailySchedule.LeaveType.ToTime.Value == checkOutTime)
            {
                shift.CheckIn = GetHalfShiftCode(dailySchedule.Shift.CheckIn);
                shift.CheckOut = GetHalfShiftCode(dailySchedule.LeaveType.FromTime.Value.ToShortTime());
                shift.Code = $"{shift.CheckIn} >{shift.CheckOut}";
                shift.IsInAfterDay = dailySchedule.Shift.IsInAfterDay;
                shift.IsOutAfterDay = dailySchedule.Shift.IsOutAfterDay;
                shift.IsShift = true;
            }
            else
            {
                shift.Code = string.Empty;
            }
        }

        return shift;
    }

    private static DateTime GetCheckInTime(DateTime workDate, ShiftBase shift)
    {
        var checkInTime = ParseTime(shift.CheckIn);
        return workDate.Date.AddDays(shift.IsInAfterDay ? 1 : 0).AddHours(checkInTime.Hours);
    }

    private static DateTime GetCheckOutTime(DateTime workDate, ShiftBase shift)
    {
        var checkOutTime = ParseTime(shift.CheckOut);
        return workDate.Date.AddDays(shift.IsOutAfterDay ? 1 : 0).AddHours(checkOutTime.Hours);
    }

    private static string GetHalfShiftCode(string shortTime)
    {
        var time = ParseTime(shortTime);
        return time.Minutes > 0 ? time.ToString(@"hh\:mm") : time.ToString("hh");
    }

    private static TimeSpan ParseTime(string time) => TimeSpan.Parse(time, new CultureInfo("en-US"));
}
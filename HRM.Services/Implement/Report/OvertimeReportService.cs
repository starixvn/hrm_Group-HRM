using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRM.Core;
using HRM.Core.Constants;
using HRM.Core.Enums;
using HRM.Core.Services;
using HRM.Criteria.Report.Overtime;
using HRM.Models.Report.Overtime;
using HRM.Services.ExportExcel;
using HRM.Services.Interfaces.Report;
using Microsoft.AspNetCore.Hosting;
using OfficeOpenXml;

namespace HRM.Services.Implement.Report;

public class OvertimeReportService(
    IWebHostEnvironment env,
    IBaseService baseService,
    IExportExcelService exportExcelService) : IOvertimeReportService
{
    private const string DateFormat = "dd-MMM-yyyy";
    private const byte MinimumWidth = 5;
    private const byte MaximumWidth = 50;

    public async Task<IEnumerable<YearlyOvertime>> GetYearlyOvertimeAsync(GetYearlyOvertimeParam param)
    {
        var monthXml = new StringBuilder(Constants.BeginRootTag);
        for (var month = 1; month <= Constants.December; month++)
        {
            var fromDate = new DateTimeOffset(param.Year, month, 1, 0, 0, 0, TimeSpan.Zero).Date;
            var toDate = fromDate.LastDayOfMonth();

            if (param.Calendar == (byte)CalendarType.Fiscal)
            {
                (fromDate, toDate) = GetDateRange(fromDate);
            }

            monthXml.Append($"<Row Month='{month}' FromDate='{fromDate}' ToDate='{toDate}' />");
        }

        monthXml.Append(Constants.EndRootTag);

        var criteria = new GetYearlyOvertimeReportCriteria
        {
            CompanyId = param.CompanyId,
            ActorId = param.ActorId,
            TeamIds = param.TeamIds,
            Employee = param.Employee,
            EmployeeStatusIds = param.EmployeeStatusIds,
            IsLocal = param.IsLocal,
            MonthXml = monthXml.ToString()
        };
        return await baseService.GetDataAsync<YearlyOvertime>(criteria);
    }

    public async Task<IEnumerable<DailyOvertimeDetails>> GetMonthlyOvertimeDetailsAsync(GetMonthlyOvertimeDetailParam param)
    {
        var fromDate = new DateTimeOffset(param.Year, param.Month, 1, 0, 0, 0, TimeSpan.Zero).Date;
        var toDate = fromDate.LastDayOfMonth();

        if (param.Calendar == CalendarType.Fiscal)
        {
            (fromDate, toDate) = GetDateRange(fromDate);
        }

        var criteria = new GetMonthlyOvertimeDetailsReportCriteria
        {
            CompanyId = param.CompanyId,
            EmpId = param.EmployeeId,
            FromDate = fromDate,
            ToDate = toDate
        };
        return await baseService.GetDataAsync<DailyOvertimeDetails>(criteria);
    }

    public async Task<Tuple<IEnumerable<DateRange>, IEnumerable<DailyOvertimeDetails>>> GetMonthlyOvertimeAsync(GetMonthlyOvertimeParam param)
    {
        var criteria = new GetMonthlyOvertimeReportCriteria
        {
            CompanyId = param.CompanyId,
            ActorId = param.ActorId,
            FromDate = param.FromDate,
            ToDate = param.ToDate,
            Employee = param.Employee,
            EmployeeStatusIds = param.EmployeeStatusIds,
            TeamIds = param.TeamIds,
            IsLocal = param.IsLocal
        };
        var (dateRanges, dailyOvertimes) = await baseService.FetchMultipleAsync<DateRange, DailyOvertimeDetails>(criteria);
        dailyOvertimes = dailyOvertimes.OrderBy(x => x.WorkDate).ThenByDescending(x => x.TeamName);
        return new Tuple<IEnumerable<DateRange>, IEnumerable<DailyOvertimeDetails>>(dateRanges, dailyOvertimes);
    }

    public async Task<IEnumerable<DailyOvertimeDetails>> GetDailyOvertimeDetailsAsync(GetDailyOvertimeDetailsParam param)
    {
        var criteria = new GetDailyOvertimeReportCriteria
        {
            CompanyId = param.CompanyId,
            ActorId = param.ActorId,
            FromDate = param.FromDate,
            ToDate = param.ToDate,
            TeamIds = param.TeamIds,
            Employee = param.Employee,
            IsLocal = param.IsLocal,
            EmployeeStatusIds = param.EmployeeStatusIds
        };
        return await baseService.GetDataAsync<DailyOvertimeDetails>(criteria);
    }

    public byte[] ExportYearlyOvertime(IEnumerable<YearlyOvertime> overtimes, GetYearlyOvertimeParam param)
    {
        var templatePath = Path.Combine(env.ContentRootPath, "ExcelTemplates/YearlyOvertime.xlsx");
        const byte totalColumns = 18;
        const byte totalHeaderRows = 2;
        const byte startMonthColumn = 6;
        const byte startBodyRow = 4;
        const byte totalRowIndex = 7;
        using var templatePackage = new ExcelPackage(new FileInfo(templatePath));
        using var xlPackage = new ExcelPackage();
        var templateWorksheet = templatePackage.Workbook.Worksheets[0];
        var worksheet = xlPackage.Workbook.Worksheets.Add("Yearly Overtime Report");
        var rowIndex = 1;
        var colIndex = 1;

        // title
        templateWorksheet.Cells[rowIndex, colIndex, rowIndex, totalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, totalColumns]);
        rowIndex++;

        // header
        templateWorksheet.Cells[rowIndex, colIndex, rowIndex + 1, totalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex + 1, totalColumns]);
        var calendarType = param.Calendar == (byte)CalendarType.Fiscal ? "Fiscal Calendar" : "Normal Calendar";
        worksheet.Cells[rowIndex, startMonthColumn].Value = $"Overtime Summary In {param.Year} ({calendarType})";
        rowIndex += totalHeaderRows;

        // data
        var rowNum = 1;
        foreach (var overtime in overtimes)
        {
            var templateRowIndex = GetTemplateRowIndex(overtime);

            colIndex = 1;
            templateWorksheet.Cells[templateRowIndex, 1, templateRowIndex, totalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, totalColumns]);

            worksheet.Cells[rowIndex, colIndex++].Value = rowNum++;
            worksheet.Cells[rowIndex, colIndex++].Value = overtime.DepartmentName;
            worksheet.Cells[rowIndex, colIndex++].Value = overtime.TeamName;
            worksheet.Cells[rowIndex, colIndex++].Value = overtime.FullName;
            worksheet.Cells[rowIndex, colIndex++].Value = overtime.Nickname;
            worksheet.Cells[rowIndex, colIndex++].Value = overtime.Jan;
            worksheet.Cells[rowIndex, colIndex++].Value = overtime.Feb;
            worksheet.Cells[rowIndex, colIndex++].Value = overtime.Mar;
            worksheet.Cells[rowIndex, colIndex++].Value = overtime.Apr;
            worksheet.Cells[rowIndex, colIndex++].Value = overtime.May;
            worksheet.Cells[rowIndex, colIndex++].Value = overtime.Jun;
            worksheet.Cells[rowIndex, colIndex++].Value = overtime.Jul;
            worksheet.Cells[rowIndex, colIndex++].Value = overtime.Aug;
            worksheet.Cells[rowIndex, colIndex++].Value = overtime.Sep;
            worksheet.Cells[rowIndex, colIndex++].Value = overtime.Oct;
            worksheet.Cells[rowIndex, colIndex++].Value = overtime.Nov;
            worksheet.Cells[rowIndex, colIndex++].Value = overtime.Dec;
            worksheet.Cells[rowIndex, colIndex].Value = overtime.Total;

            rowIndex++;
        }

        // summary
        templateWorksheet.Cells[totalRowIndex, 1, totalRowIndex, totalColumns].Copy(worksheet.Cells[rowIndex, 1, rowIndex, totalColumns]);
        const byte totalMonths = 13; // 12 months + total column
        WriteSummaryFormulas(worksheet, rowIndex, startMonthColumn, startBodyRow, totalMonths);

        FinalizeWorksheet(worksheet, startBodyRow);

        return xlPackage.GetAsByteArray();
    }

    public byte[] ExportMonthlyOvertime(IEnumerable<DateRange> dateRanges, IEnumerable<DailyOvertimeDetails> dailyOvertimes)
    {
        var templatePath = Path.Combine(env.ContentRootPath, "ExcelTemplates/MonthlyOvertime.xlsx");
        using var templatePackage = new ExcelPackage(new FileInfo(templatePath));
        using var xlPackage = new ExcelPackage();
        var template = templatePackage.Workbook.Worksheets[0];
        var worksheet = xlPackage.Workbook.Worksheets.Add("Raw Data");
        WriteRawData(template, worksheet, dateRanges, dailyOvertimes);

        const byte OvertimeSheetIndex = 1;
        template = templatePackage.Workbook.Worksheets[OvertimeSheetIndex];
        worksheet = xlPackage.Workbook.Worksheets.Add("Overtime Record");
        WriteOvertimeRecord(template, worksheet, dateRanges, dailyOvertimes);

        const byte SummarySheetIndex = 2;
        template = templatePackage.Workbook.Worksheets[SummarySheetIndex];
        worksheet = xlPackage.Workbook.Worksheets.Add("Summary OT");
        WriteSummaryOvertime(template, worksheet, dateRanges, dailyOvertimes);

        return xlPackage.GetAsByteArray();
    }

    public byte[] ExportDailyOvertimeDetails(IEnumerable<DailyOvertimeDetails> overtimes, GetDailyOvertimeDetailsParam param)
    {
        var templatePath = Path.Combine(env.ContentRootPath, "ExcelTemplates/DailyOvertimeDetails.xlsx");
        const byte TotalColumns = 17;
        const byte StartBodyRow = 3;
        using var templatePackage = new ExcelPackage(new FileInfo(templatePath));
        using var xlPackage = new ExcelPackage();
        var templateWorksheet = templatePackage.Workbook.Worksheets[0];
        var worksheet = xlPackage.Workbook.Worksheets.Add("Overtime Details Report");
        var rowIndex = 1;
        var colIndex = 1;

        // title
        templateWorksheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns]);
        worksheet.Cells[rowIndex, colIndex].Value =
            $"Overtime Details Report from {param.FromDate.ToString(DateFormat)} to {param.ToDate.ToString(DateFormat)}";
        rowIndex++;

        // header
        templateWorksheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns]);
        rowIndex++;

        // data
        var rowNum = 1;
        foreach (var overtime in overtimes)
        {
            var status = (CurrentRequestStatus)overtime.StatusId;
            colIndex = 1;
            var templateRowIndex = exportExcelService.GetTemplateRowIndex(StartBodyRow, overtime.Enabled, overtime.IsDeleted);
            templateWorksheet.Cells[templateRowIndex, 1, templateRowIndex, TotalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns]);

            worksheet.Cells[rowIndex, colIndex++].Value = rowNum++;
            worksheet.Cells[rowIndex, colIndex++].Value = overtime.WorkDate;
            worksheet.Cells[rowIndex, colIndex++].Value = overtime.DepartmentName;
            worksheet.Cells[rowIndex, colIndex++].Value = overtime.TeamName;
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

        FinalizeWorksheet(worksheet, StartBodyRow);

        return xlPackage.GetAsByteArray();
    }

    private static void WriteRawData(
        ExcelWorksheet template,
        ExcelWorksheet worksheet,
        IEnumerable<DateRange> dateRanges,
        IEnumerable<DailyOvertimeDetails> dailyOvertimes)
    {
        const byte TotalColumns = 18;
        var templateIndex = new
        {
            bodyRow = 3,
            summaryRow = 6,
            startSummaryColumn = 13
        };
        var rowIndex = 1;
        var colIndex = 1;

        // title
        CopyTitleWithDateRange(template, worksheet, dateRanges, rowIndex, TotalColumns);
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
            var templateRowIndex = GetTemplateRowIndex(overtime.Enabled, overtime.IsDeleted);
            template.Cells[templateRowIndex, 1, templateRowIndex, TotalColumns]
                .Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns]);

            worksheet.Cells[rowIndex, colIndex++].Value = rowNum++;
            worksheet.Cells[rowIndex, colIndex++].Value = overtime.WorkDate;
            worksheet.Cells[rowIndex, colIndex++].Value = overtime.UnitCode;
            worksheet.Cells[rowIndex, colIndex++].Value = overtime.DepartmentName;
            worksheet.Cells[rowIndex, colIndex++].Value = overtime.TeamName;
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

        // summary
        if (dailyOvertimes.Any())
        {
            template.Cells[templateIndex.summaryRow, 1, templateIndex.summaryRow, TotalColumns].Copy(worksheet.Cells[rowIndex, 1, rowIndex, TotalColumns]);
            const byte TotalSummaryColumns = 4;
            WriteSummaryFormulas(worksheet, rowIndex, templateIndex.startSummaryColumn, templateIndex.bodyRow, TotalSummaryColumns);
        }

        FinalizeWorksheet(worksheet, templateIndex.bodyRow);
    }

    private static byte GetTemplateRowIndex(bool isEnable, bool isDeleted, byte baseRowIndex = 3)
    {
        const byte rowOffset = 0;
        const byte disabledOffset = 1;
        const byte deletedOffset = 2;

        if (isDeleted)
        {
            return (byte)(baseRowIndex + deletedOffset);
        }

        return isEnable ? (byte)(baseRowIndex + rowOffset) : (byte)(baseRowIndex + disabledOffset);
    }

    private static byte GetTemplateRowIndex(YearlyOvertime record)
    {
        return GetTemplateRowIndex(record.Enabled, record.IsDeleted, 4);
    }

    private static void WriteOvertimeRecord(
        ExcelWorksheet template,
        ExcelWorksheet worksheet,
        IEnumerable<DateRange> dateRanges,
        IEnumerable<DailyOvertimeDetails> dailyOvertimes)
    {
        const byte totalFixedColumns = 11;
        var templateIndex = new
        {
            headerRow = 2,
            bodyRow = 3,
            summaryRow = 6,
            startSummaryColumn = 8,
            startNormalDayColumn = 12,
            endNormalDayColumn = 14,
            holidayColumn = 15
        };
        var rowIndex = 3;
        int colIndex;

        // title & header
        WriteHeaderOvertimeRecord(template, worksheet, dateRanges, templateIndex);

        // data
        var rowNum = 1;
        var employees = dailyOvertimes.GroupBy(x => x.EmpId)
            .Select(x => x.First()).OrderByDescending(x => x.TeamName).ThenBy(x => x.Code);
        foreach (var employee in employees)
        {
            var overtimes = dailyOvertimes.Where(x => x.EmpId == employee.EmpId);
            colIndex = 1;
            var templateRowIndex = GetTemplateRowIndex(employee.Enabled, employee.IsDeleted);
            template.Cells[templateRowIndex, 1, templateRowIndex, totalFixedColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, totalFixedColumns]);

            worksheet.Cells[rowIndex, colIndex++].Value = rowNum++;
            worksheet.Cells[rowIndex, colIndex++].Value = employee.UnitCode;
            worksheet.Cells[rowIndex, colIndex++].Value = employee.DepartmentName;
            worksheet.Cells[rowIndex, colIndex++].Value = employee.TeamName;
            worksheet.Cells[rowIndex, colIndex++].Value = employee.Code;
            worksheet.Cells[rowIndex, colIndex++].Value = employee.FullName;
            worksheet.Cells[rowIndex, colIndex++].Value = employee.Nickname;
            worksheet.Cells[rowIndex, colIndex++].Value = overtimes.Sum(x => x.NormalHours);
            worksheet.Cells[rowIndex, colIndex++].Value = overtimes.Sum(x => x.OffHours);
            worksheet.Cells[rowIndex, colIndex++].Value = overtimes.Sum(x => x.PublicHours);
            worksheet.Cells[rowIndex, colIndex++].Value = overtimes.Sum(x => x.SpecialHours);

            foreach (var date in dateRanges)
            {
                var overtime = overtimes.FirstOrDefault(x => x.WorkDate.Date == date.Date.Date);
                if (date.IsHoliday)
                {
                    template.Cells[templateRowIndex, templateIndex.startNormalDayColumn].Copy(worksheet.Cells[rowIndex, colIndex]);
                    worksheet.Cells[rowIndex, colIndex++].Value = overtime?.PublicHours;
                }
                else
                {
                    var toColumn = colIndex + 3;
                    template.Cells[templateRowIndex, templateIndex.startNormalDayColumn, templateRowIndex, templateIndex.endNormalDayColumn]
                        .Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, toColumn]);
                    worksheet.Cells[rowIndex, colIndex++].Value = overtime?.NormalHours;
                    worksheet.Cells[rowIndex, colIndex++].Value = overtime?.OffHours;
                    worksheet.Cells[rowIndex, colIndex++].Value = overtime?.SpecialHours;
                }
            }

            rowIndex++;
        }

        // summary
        if (employees.Any())
        {
            const byte TotalSummaryColumns = 4;
            template.Cells[templateIndex.summaryRow, 1, templateIndex.summaryRow, totalFixedColumns]
                .Copy(worksheet.Cells[rowIndex, 1, rowIndex, totalFixedColumns]);
            WriteSummaryFormulas(worksheet, rowIndex, templateIndex.startSummaryColumn, templateIndex.bodyRow, TotalSummaryColumns);
        }

        FinalizeWorksheet(worksheet, templateIndex.bodyRow, templateIndex.startNormalDayColumn);
    }

    private static void WriteHeaderOvertimeRecord(
        ExcelWorksheet template,
        ExcelWorksheet worksheet,
        IEnumerable<DateRange> dateRanges,
        dynamic templateIndex)
    {
        const byte TotalFixedColumns = 11;
        const byte TotalNormalDayColumns = 3;
        var colIndex = 1;
        
        // Copy title with date range
        CopyTitleWithDateRange(template, worksheet, dateRanges, 1, TotalFixedColumns);
        template.Cells[2, colIndex, templateIndex.headerRow, TotalFixedColumns].Copy(worksheet.Cells[2, colIndex, templateIndex.headerRow, TotalFixedColumns]);
        
        colIndex = templateIndex.startNormalDayColumn;
        foreach (var date in dateRanges)
        {
            if (date.IsHoliday)
            {
                template.Cells[1, templateIndex.holidayColumn].Copy(worksheet.Cells[1, colIndex]);
                worksheet.Cells[1, colIndex].Value = date.Date.ToString("dd-MMM");
                template.Cells[templateIndex.headerRow, templateIndex.holidayColumn].Copy(worksheet.Cells[templateIndex.headerRow, colIndex]);
            }
            else
            {
                var toColumn = colIndex + TotalNormalDayColumns;
                template.Cells[1, templateIndex.startNormalDayColumn, 1, templateIndex.endNormalDayColumn]
                    .Copy(worksheet.Cells[1, colIndex, 1, toColumn]);
                worksheet.Cells[1, colIndex].Value = date.Date.ToString("dd-MMM");
                template.Cells[templateIndex.headerRow, templateIndex.startNormalDayColumn, templateIndex.headerRow, templateIndex.endNormalDayColumn]
                    .Copy(worksheet.Cells[templateIndex.headerRow, colIndex, templateIndex.headerRow, toColumn]);
            }

            colIndex += date.IsHoliday ? 1 : TotalNormalDayColumns;
        }
    }

    private static void WriteSummaryOvertime(
        ExcelWorksheet template,
        ExcelWorksheet worksheet,
        IEnumerable<DateRange> dateRanges,
        IEnumerable<DailyOvertimeDetails> dailyOvertimes)
    {
        const byte TotalColumns = 5;
        var templateIndex = new
        {
            bodyRow = 3,
            summaryRow = 4,
            startSummaryColumn = 2
        };
        var rowIndex = 1;
        var colIndex = 1;

        // title
        CopyTitleWithDateRange(template, worksheet, dateRanges, rowIndex, TotalColumns);
        rowIndex++;

        // header
        template.Cells[rowIndex, colIndex, rowIndex, TotalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns]);
        rowIndex++;

        // data
        IEnumerable<string> teams = dailyOvertimes.Select(x => x.TeamName).Distinct().OrderBy(x => x);
        foreach (var team in teams)
        {
            var overtimes = dailyOvertimes.Where(x => x.TeamName == team);
            colIndex = 1;
            template.Cells[templateIndex.bodyRow, 1, templateIndex.bodyRow, TotalColumns]
                .Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns]);
            worksheet.Cells[rowIndex, colIndex++].Value = team;
            worksheet.Cells[rowIndex, colIndex++].Value = overtimes.Sum(x => x.NormalHours);
            worksheet.Cells[rowIndex, colIndex++].Value = overtimes.Sum(x => x.OffHours);
            worksheet.Cells[rowIndex, colIndex++].Value = overtimes.Sum(x => x.PublicHours);
            worksheet.Cells[rowIndex, colIndex].Value = overtimes.Sum(x => x.SpecialHours);

            rowIndex++;
        }

        // summary
        if (teams.Any())
        {
            template.Cells[templateIndex.summaryRow, 1, templateIndex.summaryRow, TotalColumns].Copy(worksheet.Cells[rowIndex, 1, rowIndex, TotalColumns]);
            const byte TotalSummaryColumns = 4;
            WriteSummaryFormulas(worksheet, rowIndex, templateIndex.startSummaryColumn, templateIndex.bodyRow, TotalSummaryColumns);
        }

        FinalizeWorksheet(worksheet, templateIndex.bodyRow);
    }

    private static void CopyTitleWithDateRange(
        ExcelWorksheet template,
        ExcelWorksheet worksheet,
        IEnumerable<DateRange> dateRanges,
        int rowIndex,
        int totalColumns)
    {
        var fromDate = dateRanges.Min(x => x.Date);
        var toDate = dateRanges.Max(x => x.Date);
        template.Cells[rowIndex, 1, rowIndex, totalColumns].Copy(worksheet.Cells[rowIndex, 1, rowIndex, totalColumns]);
        worksheet.Cells[rowIndex, 1].Value = string.Format(
            worksheet.Cells[rowIndex, 1].Value.ToString(),
            fromDate.ToString(DateFormat),
            toDate.ToString(DateFormat));
    }

    private static void WriteSummaryFormulas(
        ExcelWorksheet worksheet,
        int rowIndex,
        int startColumn,
        int bodyRowIndex,
        int totalColumns)
    {
        for (var i = 0; i < totalColumns; i++)
        {
            var colIndex = startColumn + i;
            var excelAddress = $"{worksheet.Cells[bodyRowIndex, colIndex].Address}:{worksheet.Cells[rowIndex - 1, colIndex].Address}";
            worksheet.Cells[rowIndex, colIndex].Formula = string.Format(Constants.SummaryFormula, excelAddress);
        }
    }

    private static void FinalizeWorksheet(ExcelWorksheet worksheet, int freezeRow, int freezeColumn = 1)
    {
        worksheet.Cells.AutoFitColumns(MinimumWidth, MaximumWidth);
        worksheet.View.FreezePanes(freezeRow, freezeColumn);
    }

    private static ValueTuple<DateTime, DateTime> GetDateRange(DateTime month)
    {
        const byte FromDay = 21;
        const byte ToDay = 20;
        var newRuleApplyFromMonth = new DateTimeOffset(2020, 5, 1, 0, 0, 0, TimeSpan.Zero);
        var lastMonth = month.AddMonths(-1);
        var fromDate = new DateTimeOffset(lastMonth.Year, lastMonth.Month, 10, 0, 0, 0, TimeSpan.Zero);
        var toDate = new DateTimeOffset(month.Year, month.Month, 9, 0, 0, 0, TimeSpan.Zero);

        if (month == newRuleApplyFromMonth)
        {
            toDate = new DateTimeOffset(month.Year, month.Month, ToDay, 0, 0, 0, TimeSpan.Zero);
        }

        if (month > newRuleApplyFromMonth)
        {
            fromDate = new DateTimeOffset(lastMonth.Year, lastMonth.Month, FromDay, 0, 0, 0, TimeSpan.Zero);
            toDate = new DateTimeOffset(month.Year, month.Month, ToDay, 0, 0, 0, TimeSpan.Zero);
        }

        return (fromDate.Date, toDate.Date);
    }
}
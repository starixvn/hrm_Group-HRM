namespace HRM.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using HRM.Core;
    using HRM.Core.Constants;
    using HRM.Core.Enums;
    using HRM.Core.Services;
    using HRM.Criteria.Report;
    using HRM.Models;
    using HRM.Models.Report.MonthlyWorkingHours;
    using HRM.Models.Request;
    using HRM.Repositories;
    using Microsoft.AspNetCore.Hosting;
    using OfficeOpenXml;
    using HRM.Services.Implement;
    using HRM.Services.Implement.Request;

    public partial interface IEmployeeWorkingHoursService
    {
        Task<IEnumerable<MonthlyWorking>> GetMonthlyWorkingHoursAsync(GetMonthlyWorkingHoursParam param);

        IEnumerable<EmployeeRequest> GetEmployeeRequests(GetMonthlyWorkingHoursParam param);

        Task<byte[]> ExportMonthlyWorkingHours(GetMonthlyWorkingHoursParam param);
    }

    public partial class EmployeeWorkingHoursService(
        IWebHostEnvironment env,
        IEmployeeWorkingHoursRepository repository,
        IBaseService baseService,
        ICommonDataService commonDataService) : IEmployeeWorkingHoursService
    {
        private const ushort TotalFixedColumns = 21;
        private const ushort TotalUPLColumns = 4;
        private const ushort StartDateColumnIndex = 26;
        private const ushort StartUPLColumnIndex = 22;
        private const ushort EndUPLColumnIndex = 25;
        private const ushort StartHeaderRowIndex = 2;
        private const ushort EndHeaderRowIndex = 3;
        private const ushort StartBodyRowIndex = 4;

        private IEnumerable<LeaveType> LeaveTypes { get; set; }

        public async Task<IEnumerable<MonthlyWorking>> GetMonthlyWorkingHoursAsync(GetMonthlyWorkingHoursParam param)
        {
            var criteria = new GetMonthlyWorkingHoursCriteria
            {
                ActorId = param.ActorId,
                FromDate = param.FromDate,
                ToDate = param.ToDate,
                TeamIds = param.TeamIds,
                Employee = param.Employee,
                IsLocal = param.IsLocal,
                EmployeeStatusId = param.EmployeeStatusId
            };

            return await repository.GetMonthlyWorkingHoursAsync(criteria);
        }

        public IEnumerable<EmployeeRequest> GetEmployeeRequests(GetMonthlyWorkingHoursParam param)
        {
            var criteria = new GetEmployeeRequestCriteria
            {
                ActorId = param.ActorId,
                FromDate = param.FromDate,
                ToDate = param.ToDate,
                TeamIds = param.TeamIds,
                Employee = param.Employee,
                IsLocal = param.IsLocal,
                EmployeeStatusId = param.EmployeeStatusId
            };

            return baseService.GetData<EmployeeRequest>(criteria);
        }

        public async Task<byte[]> ExportMonthlyWorkingHours(GetMonthlyWorkingHoursParam param)
        {
            var templatePath = Path.Combine(env.ContentRootPath, "ExcelTemplates/MonthlyWorkingHours.xlsx");
            using (var templatePackage = new ExcelPackage(new FileInfo(templatePath)))
            {
                using (var xlPackage = new ExcelPackage())
                {
                    var monthlyWorkings = await GetMonthlyWorkingHoursAsync(param);
                    LeaveTypes = await commonDataService.GetLeaveTypeAsync();

                    var template = templatePackage.Workbook.Worksheets[0];
                    var worksheet = xlPackage.Workbook.Worksheets.Add("Details");
                    WriteDetailsSheet(template, worksheet, monthlyWorkings, param);

                    const byte SummarySheetIndex = 1;
                    template = templatePackage.Workbook.Worksheets[SummarySheetIndex];
                    worksheet = xlPackage.Workbook.Worksheets.Add("Summary");
                    WriteSummarySheet(template, worksheet, monthlyWorkings, param);

                    const byte RequestSheetIndex = 2;
                    template = templatePackage.Workbook.Worksheets[RequestSheetIndex];
                    worksheet = xlPackage.Workbook.Worksheets.Add("Employee's request");
                    WriteRequestSheet(template, worksheet, param);

                    return xlPackage.GetAsByteArray();
                }
            }
        }

        private void WriteDetailsSheet(
            ExcelWorksheet template,
            ExcelWorksheet worksheet,
            IEnumerable<MonthlyWorking> monthlyWorkings,
            GetMonthlyWorkingHoursParam param)
        {
            var months = GetMonthsBetween(param.FromDate, param.ToDate);
            var totalDays = (int)(param.ToDate - param.FromDate).TotalDays;
            TotalColumns = TotalFixedColumns + months.Count() * TotalUPLColumns + (totalDays > 0 ? totalDays + 1 : 0);

            // title
            template.Cells[1, 1, 1, TotalFixedColumns].Copy(worksheet.Cells[1, 1, 1, TotalFixedColumns]);
            worksheet.Cells[1, 1].Value = string.Format(
                worksheet.Cells[1, 1].Value.ToString(),
                param.FromDate.ToString(ShortDateFormat),
                param.ToDate.ToString(ShortDateFormat));

            // header
            WriteHeader(template, worksheet, param);

            // data
            var rowIndex = (int)StartBodyRowIndex;
            WriteBody(template, worksheet, monthlyWorkings, ref rowIndex, param);

            // summary
            if (monthlyWorkings.Any())
            {
                const byte SummaryRowIndex = 5;
                TotalColumns = TotalFixedColumns + months.Count() * TotalUPLColumns;
                template.Cells[SummaryRowIndex, 1, SummaryRowIndex, 8].Copy(worksheet.Cells[rowIndex, 1, rowIndex, 8]);
                for (var i = 9; i <= TotalColumns; i++)
                {
                    template.Cells[SummaryRowIndex, 9].Copy(worksheet.Cells[rowIndex, i]);
                    var excelAddress = $"{worksheet.Cells[StartBodyRowIndex, i].Address}:{worksheet.Cells[rowIndex - 1, i].Address}";
                    worksheet.Cells[rowIndex, i].Formula = string.Format(Constants.SummaryFormula, excelAddress);
                }
            }

            worksheet.Cells.AutoFitColumns();
            worksheet.View.FreezePanes(StartBodyRowIndex, 8);
        }

        private void WriteHeader(ExcelWorksheet template, ExcelWorksheet worksheet, GetMonthlyWorkingHoursParam param)
        {
            template.Cells[StartHeaderRowIndex, 1, EndHeaderRowIndex, TotalFixedColumns]
                .Copy(worksheet.Cells[StartHeaderRowIndex, 1, EndHeaderRowIndex, TotalFixedColumns]);

            var months = GetMonthsBetween(param.FromDate, param.ToDate);
            var colIndex = (int)StartUPLColumnIndex;
            foreach (var month in months)
            {
                template.Cells[StartHeaderRowIndex, StartUPLColumnIndex, EndHeaderRowIndex, EndUPLColumnIndex]
                    .Copy(worksheet.Cells[StartHeaderRowIndex, colIndex, EndHeaderRowIndex, colIndex + TotalUPLColumns - 1]);
                worksheet.Cells[StartHeaderRowIndex, colIndex].Value = $"Unpaid Leaves {month:MMM-yyyy}";

                colIndex += TotalUPLColumns;
            }

            for (var day = param.FromDate.Date; day.Date <= param.ToDate.Date; day = day.AddDays(1))
            {
                template.Cells[StartHeaderRowIndex, StartDateColumnIndex, EndHeaderRowIndex, StartDateColumnIndex]
                    .Copy(worksheet.Cells[StartHeaderRowIndex, colIndex, EndHeaderRowIndex, colIndex]);
                worksheet.Cells[StartHeaderRowIndex, colIndex].Value = day.ToString("dd MMM, yyyy (ddd)");

                if (day.DayOfWeek == DayOfWeek.Saturday || day.DayOfWeek == DayOfWeek.Sunday)
                {
                    worksheet.Cells[StartHeaderRowIndex, colIndex].Style.Font.Color.SetColor(RedColor);
                }

                colIndex++;
            }
        }

        private void WriteBody(
            ExcelWorksheet template,
            ExcelWorksheet worksheet,
            IEnumerable<MonthlyWorking> monthlyWorkings,
            ref int rowIndex,
            GetMonthlyWorkingHoursParam param)
        {
            var rowNum = 1;
            foreach (var monthlyWorking in monthlyWorkings)
            {
                var colIndex = 1;
                template.Cells[StartBodyRowIndex, 1, StartBodyRowIndex, TotalFixedColumns]
                    .Copy(worksheet.Cells[rowIndex, 1, rowIndex, TotalFixedColumns]);

                worksheet.Cells[rowIndex, colIndex++].Value = rowNum++;
                worksheet.Cells[rowIndex, colIndex++].Value = monthlyWorking.Department;
                worksheet.Cells[rowIndex, colIndex++].Value = monthlyWorking.Section;
                worksheet.Cells[rowIndex, colIndex++].Value = monthlyWorking.Team.Name;
                worksheet.Cells[rowIndex, colIndex++].Value = monthlyWorking.Code;
                worksheet.Cells[rowIndex, colIndex++].Value = monthlyWorking.FullName;
                worksheet.Cells[rowIndex, colIndex++].Value = monthlyWorking.Nickname;
                worksheet.Cells[rowIndex, colIndex++].Value = monthlyWorking.WorkingDays;
                worksheet.Cells[rowIndex, colIndex++].Value = monthlyWorking.ActualWorkingDays + monthlyWorking.DailyWorkings.Count(x => IsHoliday(x));

                var months = GetMonthsBetween(param.FromDate, param.ToDate);
                var dailyColumnIndex = TotalFixedColumns + months.Count() * TotalUPLColumns + 1;
                const string CountLeaveFormula = "COUNTIF({0},\"{1}\")+(COUNTIF({0},\"{1}½\")/2)+(COUNTIF({0},\"{1}½/*\")/2)+(COUNTIF({0},\"*/{1}½\")/2)";

                var excelAddress = $"{worksheet.Cells[rowIndex, dailyColumnIndex].Address}:{worksheet.Cells[rowIndex, TotalColumns].Address}";
                worksheet.Cells[rowIndex, colIndex++].Formula = string.Format("COUNTIF({0},\"PH\")", excelAddress);

                var leaveCode = LeaveTypes.First(x => x.Id == (short)Core.Enums.LeaveTypes.MaternityLeave).Code;
                excelAddress = $"{worksheet.Cells[rowIndex, dailyColumnIndex].Address}:{worksheet.Cells[rowIndex, TotalColumns].Address}";
                worksheet.Cells[rowIndex, colIndex++].Formula = string.Format(CountLeaveFormula, excelAddress, leaveCode);

                // payment leaves
                var annualLeaveColumnIndex = colIndex;
                leaveCode = LeaveTypes.First(x => x.Id == (short)Core.Enums.LeaveTypes.AnnualLeave).Code;
                excelAddress = $"{worksheet.Cells[rowIndex, dailyColumnIndex].Address}:{worksheet.Cells[rowIndex, TotalColumns].Address}";
                worksheet.Cells[rowIndex, colIndex++].Formula = string.Format(CountLeaveFormula, excelAddress, leaveCode);

                leaveCode = LeaveTypes.First(x => x.Id == (short)Core.Enums.LeaveTypes.BusinessTrip).Code;
                excelAddress = $"{worksheet.Cells[rowIndex, dailyColumnIndex].Address}:{worksheet.Cells[rowIndex, TotalColumns].Address}";
                worksheet.Cells[rowIndex, colIndex++].Formula = string.Format(CountLeaveFormula, excelAddress, leaveCode);

                leaveCode = LeaveTypes.First(x => x.Id == (short)Core.Enums.LeaveTypes.BereavementLeave).Code;
                excelAddress = $"{worksheet.Cells[rowIndex, dailyColumnIndex].Address}:{worksheet.Cells[rowIndex, TotalColumns].Address}";
                worksheet.Cells[rowIndex, colIndex++].Formula = string.Format(CountLeaveFormula, excelAddress, leaveCode);

                leaveCode = LeaveTypes.First(x => x.Id == (short)Core.Enums.LeaveTypes.OffsetLeave).Code;
                excelAddress = $"{worksheet.Cells[rowIndex, dailyColumnIndex].Address}:{worksheet.Cells[rowIndex, TotalColumns].Address}";
                worksheet.Cells[rowIndex, colIndex++].Formula = string.Format(CountLeaveFormula, excelAddress, leaveCode);

                leaveCode = LeaveTypes.First(x => x.Id == (short)Core.Enums.LeaveTypes.WeddingLeave).Code;
                excelAddress = $"{worksheet.Cells[rowIndex, dailyColumnIndex].Address}:{worksheet.Cells[rowIndex, TotalColumns].Address}";
                worksheet.Cells[rowIndex, colIndex++].Formula = string.Format(CountLeaveFormula, excelAddress, leaveCode);

                leaveCode = LeaveTypes.First(x => x.Id == (short)Core.Enums.LeaveTypes.SuspendWork).Code;
                excelAddress = $"{worksheet.Cells[rowIndex, dailyColumnIndex].Address}:{worksheet.Cells[rowIndex, TotalColumns].Address}";
                worksheet.Cells[rowIndex, colIndex++].Formula = string.Format(CountLeaveFormula, excelAddress, leaveCode);

                leaveCode = LeaveTypes.First(x => x.Id == (short)Core.Enums.LeaveTypes.HospitalizationLeave).Code;
                excelAddress = $"{worksheet.Cells[rowIndex, dailyColumnIndex].Address}:{worksheet.Cells[rowIndex, TotalColumns].Address}";
                worksheet.Cells[rowIndex, colIndex++].Formula = string.Format(CountLeaveFormula, excelAddress, leaveCode);

                leaveCode = LeaveTypes.First(x => x.Id == (short)Core.Enums.LeaveTypes.KeepInViewByDay).Code;
                excelAddress = $"{worksheet.Cells[rowIndex, dailyColumnIndex].Address}:{worksheet.Cells[rowIndex, TotalColumns].Address}";
                worksheet.Cells[rowIndex, colIndex++].Formula = string.Format(CountLeaveFormula, excelAddress, leaveCode);

                leaveCode = LeaveTypes.First(x => x.Id == (short)Core.Enums.LeaveTypes.CompanyEvent).Code;
                excelAddress = $"{worksheet.Cells[rowIndex, dailyColumnIndex].Address}:{worksheet.Cells[rowIndex, TotalColumns].Address}";
                worksheet.Cells[rowIndex, colIndex].Formula = string.Format(CountLeaveFormula, excelAddress, leaveCode);

                excelAddress = $"{worksheet.Cells[rowIndex, annualLeaveColumnIndex].Address}:{worksheet.Cells[rowIndex, colIndex++].Address}";
                worksheet.Cells[rowIndex, colIndex++].Formula = string.Format(Constants.SummaryFormula, excelAddress);

                // unpaid leaves
                var startColumnIndex = dailyColumnIndex;
                foreach (DateTime month in months)
                {
                    template.Cells[StartBodyRowIndex, StartUPLColumnIndex, StartBodyRowIndex, EndUPLColumnIndex]
                        .Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, colIndex + TotalUPLColumns - 1]);
                    var endColumnIndex = startColumnIndex +
                        monthlyWorking.DailyWorkings.Count(x => x.WorkDate.Year == month.Year && x.WorkDate.Month == month.Month) - 1;
                    excelAddress = $"{worksheet.Cells[rowIndex, startColumnIndex].Address}:{worksheet.Cells[rowIndex, endColumnIndex].Address}";

                    var unpaidLeaveColumnIndex = colIndex;
                    leaveCode = LeaveTypes.First(x => x.Id == (short)Core.Enums.LeaveTypes.UnpaidLeave).Code;
                    worksheet.Cells[rowIndex, colIndex++].Formula = string.Format(CountLeaveFormula, excelAddress, leaveCode);

                    leaveCode = LeaveTypes.First(x => x.Id == (short)Core.Enums.LeaveTypes.UnpaidLeaveInsurance).Code;
                    worksheet.Cells[rowIndex, colIndex++].Formula = string.Format(CountLeaveFormula, excelAddress, leaveCode);

                    leaveCode = LeaveTypes.First(x => x.Id == (short)Core.Enums.LeaveTypes.PaternityLeave).Code;
                    worksheet.Cells[rowIndex, colIndex].Formula = string.Format(CountLeaveFormula, excelAddress, leaveCode);

                    excelAddress = $"{worksheet.Cells[rowIndex, unpaidLeaveColumnIndex].Address}:{worksheet.Cells[rowIndex, colIndex++].Address}";
                    worksheet.Cells[rowIndex, colIndex++].Formula = string.Format(Constants.SummaryFormula, excelAddress);

                    startColumnIndex = endColumnIndex + 1;
                }

                foreach (var dailyWorking in monthlyWorking.DailyWorkings)
                {
                    template.Cells[StartBodyRowIndex, StartDateColumnIndex].Copy(worksheet.Cells[rowIndex, colIndex]);
                    WriteDailyCells(worksheet.Cells[rowIndex, colIndex++], dailyWorking);
                }

                rowIndex++;
            }
        }

        private void WriteSummarySheet(
            ExcelWorksheet template,
            ExcelWorksheet worksheet,
            IEnumerable<MonthlyWorking> monthlyWorkings,
            GetMonthlyWorkingHoursParam param)
        {
            const byte DefaultColumns = 15;
            var templateIndex = new
            {
                headerRow = 2,
                bodyRow = 3,
                summaryRow = 4,
                actualWorkingDaysColumn = 4,
                maternityLeaveColumn = 6,
                companyEventColumn = 15,
                unpaidLeaveColumn = 16
            };
            var months = GetMonthsBetween(param.FromDate, param.ToDate);
            TotalColumns = DefaultColumns + months.Count();
            var rowIndex = 1;

            // title
            template.Cells[1, 1, 1, DefaultColumns].Copy(worksheet.Cells[1, 1, 1, DefaultColumns]);
            worksheet.Cells[1, 1].Value = string.Format(
                worksheet.Cells[1, 1].Value.ToString(),
                param.FromDate.ToString(ShortDateFormat),
                param.ToDate.ToString(ShortDateFormat));

            // header
            WriteHeaderSummary(template, worksheet, months, templateIndex);

            // data
            WriteBodySummary(template, worksheet, monthlyWorkings, months, ref rowIndex, templateIndex);

            // summary
            if (monthlyWorkings.Any())
            {
                var colIndex = templateIndex.actualWorkingDaysColumn;
                template.Cells[templateIndex.summaryRow, 1, templateIndex.summaryRow, colIndex - 1]
                    .Copy(worksheet.Cells[rowIndex, 1, rowIndex, colIndex - 1]);
                for (var i = templateIndex.actualWorkingDaysColumn; i <= TotalColumns; i++)
                {
                    template.Cells[templateIndex.summaryRow, templateIndex.maternityLeaveColumn].Copy(worksheet.Cells[rowIndex, colIndex]);
                    var excelAddress = $"{worksheet.Cells[templateIndex.bodyRow, colIndex].Address}:{worksheet.Cells[rowIndex - 1, colIndex].Address}";
                    worksheet.Cells[rowIndex, colIndex++].Formula = string.Format(Constants.SummaryFormula, excelAddress);
                }
            }

            const byte MinimumWidth = 10;
            const byte MaximumWidth = 50;
            worksheet.Cells.AutoFitColumns(MinimumWidth, MaximumWidth);
            worksheet.View.FreezePanes(templateIndex.bodyRow, 1);
        }

        private static void WriteHeaderSummary(
            ExcelWorksheet template,
            ExcelWorksheet worksheet,
            IEnumerable<DateTime> months,
            dynamic templateIndex)
        {
            var rowIndex = templateIndex.headerRow;
            var colIndex = templateIndex.companyEventColumn;
            template.Cells[rowIndex, 1, rowIndex, colIndex].Copy(worksheet.Cells[rowIndex, 1, rowIndex, colIndex]);
            colIndex++;
            foreach (var month in months)
            {
                template.Cells[rowIndex, templateIndex.unpaidLeaveColumn].Copy(worksheet.Cells[rowIndex, colIndex]);
                worksheet.Cells[rowIndex, colIndex].Value = string.Format(worksheet.Cells[rowIndex, colIndex].Value.ToString(), month.ToString("MMM-yyyy"));
                colIndex++;
            }
        }

        private void WriteBodySummary(
            ExcelWorksheet template,
            ExcelWorksheet worksheet,
            IEnumerable<MonthlyWorking> monthlyWorkings,
            IEnumerable<DateTime> months,
            ref int rowIndex,
            dynamic templateIndex)
        {
            rowIndex = templateIndex.bodyRow;
            var teams = monthlyWorkings.GroupBy(x => new { x.Team.Id, x.Team.Name, x.Department, x.Section })
                .Select(x => new { x.Key.Department, x.Key.Section, Team = x.Key.Name, Members = x.ToList() })
                .OrderBy(x => x.Department).ThenBy(x => x.Section).ThenBy(x => x.Team).ToList();
            foreach (var team in teams)
            {
                var totalHolidays = team.Members.Sum(x => x.DailyWorkings.Count(y => IsHoliday(y)));
                var colIndex = 1;
                template.Cells[templateIndex.bodyRow, 1, templateIndex.bodyRow, templateIndex.companyEventColumn]
                    .Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, templateIndex.companyEventColumn]);

                worksheet.Cells[rowIndex, colIndex++].Value = team.Department;
                worksheet.Cells[rowIndex, colIndex++].Value = team.Section;
                worksheet.Cells[rowIndex, colIndex++].Value = team.Team;
                worksheet.Cells[rowIndex, colIndex++].Value = team.Members.Sum(x => x.ActualWorkingDays) + totalHolidays;
                worksheet.Cells[rowIndex, colIndex++].Value = totalHolidays;
                worksheet.Cells[rowIndex, colIndex++].Value =
                    team.Members.Sum(x => SummaryLeaveDay(x.DailyWorkings, Core.Enums.LeaveTypes.MaternityLeave));
                worksheet.Cells[rowIndex, colIndex++].Value =
                    team.Members.Sum(x => SummaryLeaveDay(x.DailyWorkings, Core.Enums.LeaveTypes.AnnualLeave));
                worksheet.Cells[rowIndex, colIndex++].Value =
                    team.Members.Sum(x => SummaryLeaveDay(x.DailyWorkings, Core.Enums.LeaveTypes.BusinessTrip));
                worksheet.Cells[rowIndex, colIndex++].Value =
                    team.Members.Sum(x => SummaryLeaveDay(x.DailyWorkings, Core.Enums.LeaveTypes.BereavementLeave));
                worksheet.Cells[rowIndex, colIndex++].Value =
                    team.Members.Sum(x => SummaryLeaveDay(x.DailyWorkings, Core.Enums.LeaveTypes.OffsetLeave));
                worksheet.Cells[rowIndex, colIndex++].Value = team.Members.Sum(x =>
                    SummaryLeaveDay(x.DailyWorkings, Core.Enums.LeaveTypes.WeddingLeave)
                    + SummaryLeaveDay(x.DailyWorkings, Core.Enums.LeaveTypes.WeddingOfEmployeeChildren));
                worksheet.Cells[rowIndex, colIndex++].Value =
                    team.Members.Sum(x => SummaryLeaveDay(x.DailyWorkings, Core.Enums.LeaveTypes.SuspendWork));
                worksheet.Cells[rowIndex, colIndex++].Value =
                    team.Members.Sum(x => SummaryLeaveDay(x.DailyWorkings, Core.Enums.LeaveTypes.HospitalizationLeave));
                worksheet.Cells[rowIndex, colIndex++].Value =
                    team.Members.Sum(x => SummaryLeaveDay(x.DailyWorkings, Core.Enums.LeaveTypes.KeepInViewByDay));
                worksheet.Cells[rowIndex, colIndex++].Value =
                    team.Members.Sum(x => SummaryLeaveDay(x.DailyWorkings, Core.Enums.LeaveTypes.CompanyEvent));

                foreach (var month in months)
                {
                    template.Cells[templateIndex.bodyRow, templateIndex.unpaidLeaveColumn].Copy(worksheet.Cells[rowIndex, colIndex]);
                    worksheet.Cells[rowIndex, colIndex++].Value =
                        team.Members.Sum(x => SummaryLeaveDay(
                            x.DailyWorkings.Where(daily => daily.WorkDate.Month == month.Month && daily.WorkDate.Year == month.Year),
                            Core.Enums.LeaveTypes.UnpaidLeave))
                        + team.Members.Sum(x => SummaryLeaveDay(
                            x.DailyWorkings.Where(daily => daily.WorkDate.Month == month.Month && daily.WorkDate.Year == month.Year),
                            Core.Enums.LeaveTypes.UnpaidLeaveInsurance))
                        + team.Members.Sum(x => SummaryLeaveDay(
                            x.DailyWorkings.Where(daily => daily.WorkDate.Month == month.Month && daily.WorkDate.Year == month.Year),
                            Core.Enums.LeaveTypes.PaternityLeave));
                }

                rowIndex++;
            }
        }

        private void WriteRequestSheet(
            ExcelWorksheet template,
            ExcelWorksheet worksheet,
            GetMonthlyWorkingHoursParam param)
        {
            var requests = GetEmployeeRequests(param);
            var templateIndex = new
            {
                headerRow = 2,
                bodyRow = 3
            };
            TotalColumns = 11;
            var rowIndex = 1;
            var colIndex = 1;

            // title
            template.Cells[rowIndex, colIndex, rowIndex, TotalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns]);
            rowIndex++;

            // header
            template.Cells[rowIndex, colIndex, rowIndex, TotalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns]);

            // data
            var rowNum = 1;
            rowIndex++;
            foreach (var request in requests)
            {
                colIndex = 1;
                var status = (CurrentRequestStatus)request.RequestStatusId;
                template.Cells[templateIndex.bodyRow, colIndex, templateIndex.bodyRow, TotalColumns]
                    .Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns]);

                worksheet.Cells[rowIndex, colIndex++].Value = rowNum++;
                worksheet.Cells[rowIndex, colIndex++].Value = request.Department;
                worksheet.Cells[rowIndex, colIndex++].Value = request.Section;
                worksheet.Cells[rowIndex, colIndex++].Value = request.Team;
                worksheet.Cells[rowIndex, colIndex++].Value = request.FullName;
                worksheet.Cells[rowIndex, colIndex++].Value = request.Nickname;
                worksheet.Cells[rowIndex, colIndex++].Value =
                    request.RequestTypeId == (short)RequestType.Leave ? request.LeaveType : request.RequestTypeId.RequestTypeToText();
                worksheet.Cells[rowIndex, colIndex++].Value = GetRequestDetails(request);
                worksheet.Cells[rowIndex, colIndex++].Value = request.Remark;
                worksheet.Cells[rowIndex, colIndex++].Value = request.CreatedDate;
                worksheet.Cells[rowIndex, colIndex].Value = status.ToString();

                rowIndex++;
            }

            const byte MinimumWidth = 10;
            const byte MaximumWidth = 50;
            worksheet.Cells.AutoFitColumns(MinimumWidth, MaximumWidth);
            worksheet.View.FreezePanes(templateIndex.bodyRow, 1);
        }

        private static IEnumerable<DateTime> GetMonthsBetween(DateTime fromDate, DateTime toDate)
        {
            DateTime iterator;
            DateTime limit;
            IList<DateTime> months = new List<DateTime>();

            if (toDate >= fromDate)
            {
                iterator = fromDate.FirstDayOfMonth();
                limit = toDate;

                while (iterator <= limit)
                {
                    months.Add(iterator.FirstDayOfMonth());

                    iterator = iterator.AddMonths(1);
                }
            }

            return months;
        }

        private static decimal SummaryLeaveDay(IEnumerable<DailyWorking> dailyWorkings, LeaveTypes leaveType)
        {
            decimal totals = 0;
            foreach (var dailyWorking in dailyWorkings)
            {
                if (dailyWorking.IsLeaveDay
                    && (dailyWorking.LeaveType.Id == (short)leaveType
                        || (dailyWorking.LeaveType2 != null && dailyWorking.LeaveType2.Id == (short)leaveType)))
                {
                    if (dailyWorking.IsLeaveHalfDay
                        || (dailyWorking.LeaveType2 != null && dailyWorking.LeaveType.Id != dailyWorking.LeaveType2.Id))
                    {
                        totals += Constants.HalfDayOffValue;
                    }
                    else
                    {
                        totals += 1;
                    }
                }
            }

            return totals;
        }

        private string GetRequestDetails(EmployeeRequest request)
        {
            var requestEntity = new RequestEntity
            {
                RequestType = new RequestTypeModel { Id = request.RequestTypeId },
                RequestedDate = request.RequestedDate,
                LeaveType = new LeaveType { Id = request.LeaveTypeId },
                TotalDays = request.TotalDays,
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                FromTime = request.FromTime,
                ToTime = request.ToTime,
                LeaveCountDetails = request.LeaveCountDetails,
                FromShift = new Shift { Name = request.FromShift },
                ToShift = new Shift { Name = request.ToShift },
                ActualLateInEarlyOut = request.ActualInOutMins,
                TotalMins = request.TotalMinutes,
                KeepInViewType = request.KeepInViewTypeId
            };
            return RequestService.ToRequestMessage(requestEntity, true);
        }

        private static bool IsHoliday(DailyWorking dailyWorking)
        {
            if (dailyWorking.IsLeaveDay && dailyWorking.IsHoliday)
            {
                return true;
            }

            return dailyWorking.TotalWorkingHours.Equals(0) && dailyWorking.IsHoliday;
        }
    }
}
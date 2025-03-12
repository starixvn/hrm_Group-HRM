namespace HRM.Services
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using Core;
    using Criteria.Discipline;
    using Core.Constants;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Models.Discipline;
    using Resources;

    [SuppressMessage("Major Code Smell", "S109:Magic numbers should not be used", Justification = "Pending")]
    public class ExportDisciplineDetailService : IExportDisciplineDetailService
    {
        private const string TemplatePath = @"ExcelTemplates\DisciplineDetailReport.xls";
        private const int HeaderRow = 2;
        private const int BodyRow = 3;
        private const int WeekendRow = 4;
        private const int TotalCol = 26;
        private readonly IWebHostEnvironment hostingEnvironment;

        public ExportDisciplineDetailService(IWebHostEnvironment hostingEnvironment)
        {
            this.hostingEnvironment = hostingEnvironment;
        }

        public byte[] Export(GetDisciplineDetailCriteria criteria, IList<DisciplineDetailModel> disciplines)
        {
            string templatePath = Path.Combine(hostingEnvironment.ContentRootPath, TemplatePath); 
            string reportName = Labels.DisciplineDetailReportName;
            string reportTitle;
            string startDate = criteria.FromDate.ToDateMonthYearFormatedDate();
            string endDate = criteria.ToDate.ToDateMonthYearFormatedDate();

            if (criteria.EmployeeId != null)
            {
                reportTitle = string.Format(Labels.DisciplineDetailReportTitle, criteria.EmployeeName, startDate, endDate);
            }
            else if (criteria.TeamId != null)
            {
                reportTitle = string.Format(Labels.DisciplineDetailReportTitle, criteria.TeamName, startDate, endDate);
            }
            else
            {
                reportTitle = string.Format(Labels.DisciplineDetailReportTitleNoPrefix, startDate, endDate);
            }

            var writer = new ExcelTemplateWriter(templatePath, reportName, Labels.DisciplineDetailReportSheet);

            // Fill report title.
            writer.WriteRow(0, new object[] { reportTitle });
            writer.CopyRows(1, 1);

            // Fill report header.
            writer.CopyRows(HeaderRow, 1);

            FillContentRow(disciplines, ref writer);
            writer.SyncColumnsWidth(TotalCol);
            writer.CreateFreezePane(6, 3);

            return writer.WriteContentToBytes();
        }

        private void FillContentRow(IList<DisciplineDetailModel> disciplines, ref ExcelTemplateWriter writer)
        {
            int rowIndex = 1;
            foreach (DisciplineDetailModel discipline in disciplines)
            {
                FillRow(discipline, ref writer, ref rowIndex);
            }
        }

        private void FillRow(DisciplineDetailModel discipline, ref ExcelTemplateWriter writer, ref int rowIndex)
        {
            object record = CreateRecord(discipline, ref rowIndex);

            var rowType = (discipline.IsWeekend) ? WeekendRow : BodyRow;

            writer.WriteRow(rowType, record);
        }

        private object CreateRecord(DisciplineDetailModel discipline, ref int rowIndex)
        {
            decimal systemSubtractScore = discipline.SystemSubtractScore > 0 ? -discipline.SystemSubtractScore : 0;
            decimal approverSubtractScore = discipline.ApproverSubtractScore > 0 ? -discipline.ApproverSubtractScore : 0;

            string checkIn = discipline.CheckIn.HasValue ? discipline.CheckIn.Value.ToString(Constants.TimeFormat) : string.Empty;
            string checkOut = discipline.CheckOut.HasValue ? discipline.CheckOut.Value.ToString(Constants.TimeFormat) : string.Empty;
            string totalHours = !string.IsNullOrEmpty(discipline.TotalHours) ? discipline.TotalHours : string.Empty;
            string missingCheckInOut = discipline.MissingCheckInOut.Value > 0 ? discipline.MissingCheckInOut.Value.ToString() : string.Empty;
            string workingUnderTime = discipline.WorkingUnderTime.Value > 0 ? "1" : string.Empty;
            string lateIn = ReturnValueOrEmtpyString(!string.IsNullOrEmpty(discipline.MinutesLateIn), discipline.MinutesLateIn);
            string earlyOut = ReturnValueOrEmtpyString(!string.IsNullOrEmpty(discipline.MinutesEarlyOut), discipline.MinutesEarlyOut);
            string absent = ReturnValueOrEmtpyString(discipline.Absent.Value > 0, discipline.Absent.Value.ToString());
            string unpaidLeave = ReturnValueOrEmtpyString(discipline.UnpaidLeave.Value > 0, discipline.UnpaidLeave.Value.ToString());
            string unpaidLeaveAbsent = ReturnValueOrEmtpyString(discipline.UnpaidLeaveAbsent.Value > 0, discipline.UnpaidLeaveAbsent.Value.ToString());
            string changeShift = ReturnValueOrEmtpyString(discipline.ChangeShift.Value > 0, discipline.ChangeShift.Value.ToString());
            string unplannedLeave = ReturnValueOrEmtpyString(discipline.UnplannedLeave.Value > 0, discipline.UnplannedLeave.Value.ToString());
            string weekendLeave = ReturnValueOrEmtpyString(discipline.WeekendLeave.Value > 0, discipline.WeekendLeave.Value.ToString());
            string sevenDaysLeave = ReturnValueOrEmtpyString(discipline.SevenDaysLeave.Value > 0, discipline.SevenDaysLeave.Value.ToString());
            string isOutOfControlReason = ReturnValueOrEmtpyString(discipline.IsOutOfControlReason, Labels.Yes);

            var record = new
            {
                discipline.Employee.Code,
                discipline.Employee.NickName,
                discipline.Employee.FullName,
                Category = discipline.Employee.IsLocal ? Labels.Local : Labels.Expat,
                discipline.Employee.TeamName,
                discipline.WorkDate,
                discipline.ShiftName,
                CheckIn = checkIn,
                CheckOut = checkOut,
                TotalHours = totalHours,
                MissingCheckInOut = missingCheckInOut,
                WorkingUnderTime = workingUnderTime,
                LateIn = lateIn,
                EarlyOut = earlyOut,
                Absent = absent,
                UnpaidLeave = unpaidLeave,
                UnpaidLeaveAbsent = unpaidLeaveAbsent,
                ChangeShift = changeShift,
                UnplannedLeave = unplannedLeave,
                WeekendLeave = weekendLeave,
                SevenDaysLeave = sevenDaysLeave,
                SystemSubtractScore = systemSubtractScore,
                ApproverSubtractScore = approverSubtractScore,
                discipline.ApproverComment,
                discipline.EmployeeComment,
                IsOutOfControlReason = isOutOfControlReason
            };
            rowIndex++;

            return record;
        }

        private static string ReturnValueOrEmtpyString(bool condition, string value)
        {
            return condition ? value : string.Empty;
        }
    }
}
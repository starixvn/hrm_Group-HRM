using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using HRM.Core;
using HRM.Core.Constants;
using HRM.Criteria.Discipline;
using HRM.Models.Discipline;
using HRM.Resources;
using Microsoft.AspNetCore.Hosting;

namespace HRM.Services.ExportExcel.Discipline;

[SuppressMessage("Major Code Smell", "S109:Magic numbers should not be used", Justification = "Pending")]
public class ExportDisciplineDetailService(IWebHostEnvironment hostingEnvironment) : IExportDisciplineDetailService
{
    private const string TemplatePath = @"ExcelTemplates\DisciplineDetailReport.xls";
    private const int HeaderRow = 2;
    private const int BodyRow = 3;
    private const int WeekendRow = 4;
    private const int TotalCol = 26;

    public byte[] Export(GetDisciplineDetailCriteria criteria, IList<DisciplineDetailModel> disciplines)
    {
        var templatePath = Path.Combine(hostingEnvironment.ContentRootPath, TemplatePath); 
        var reportName = Labels.DisciplineDetailReportName;
        string reportTitle;
        var startDate = criteria.FromDate.ToDateMonthYearFormatedDate();
        var endDate = criteria.ToDate.ToDateMonthYearFormatedDate();

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
        var rowIndex = 1;
        foreach (var discipline in disciplines)
        {
            FillRow(discipline, ref writer, ref rowIndex);
        }
    }

    private void FillRow(DisciplineDetailModel discipline, ref ExcelTemplateWriter writer, ref int rowIndex)
    {
        var record = CreateRecord(discipline, ref rowIndex);

        var rowType = (discipline.IsWeekend) ? WeekendRow : BodyRow;

        writer.WriteRow(rowType, record);
    }

    private object CreateRecord(DisciplineDetailModel discipline, ref int rowIndex)
    {
        var systemSubtractScore = discipline.SystemSubtractScore > 0 ? -discipline.SystemSubtractScore : 0;
        var approverSubtractScore = discipline.ApproverSubtractScore > 0 ? -discipline.ApproverSubtractScore : 0;

        var checkIn = discipline.CheckIn.HasValue ? discipline.CheckIn.Value.ToString(Constants.TimeFormat) : string.Empty;
        var checkOut = discipline.CheckOut.HasValue ? discipline.CheckOut.Value.ToString(Constants.TimeFormat) : string.Empty;
        var totalHours = !string.IsNullOrEmpty(discipline.TotalHours) ? discipline.TotalHours : string.Empty;
        var missingCheckInOut = discipline.MissingCheckInOut.Value > 0 ? discipline.MissingCheckInOut.Value.ToString() : string.Empty;
        var workingUnderTime = discipline.WorkingUnderTime.Value > 0 ? "1" : string.Empty;
        var lateIn = ReturnValueOrEmptyString(!string.IsNullOrEmpty(discipline.MinutesLateIn), discipline.MinutesLateIn);
        var earlyOut = ReturnValueOrEmptyString(!string.IsNullOrEmpty(discipline.MinutesEarlyOut), discipline.MinutesEarlyOut);
        var absent = ReturnValueOrEmptyString(discipline.Absent.Value > 0, discipline.Absent.Value.ToString());
        var unpaidLeave = ReturnValueOrEmptyString(discipline.UnpaidLeave.Value > 0, discipline.UnpaidLeave.Value.ToString());
        var unpaidLeaveAbsent = ReturnValueOrEmptyString(discipline.UnpaidLeaveAbsent.Value > 0, discipline.UnpaidLeaveAbsent.Value.ToString());
        var changeShift = ReturnValueOrEmptyString(discipline.ChangeShift.Value > 0, discipline.ChangeShift.Value.ToString());
        var unplannedLeave = ReturnValueOrEmptyString(discipline.UnplannedLeave.Value > 0, discipline.UnplannedLeave.Value.ToString());
        var weekendLeave = ReturnValueOrEmptyString(discipline.WeekendLeave.Value > 0, discipline.WeekendLeave.Value.ToString());
        var sevenDaysLeave = ReturnValueOrEmptyString(discipline.SevenDaysLeave.Value > 0, discipline.SevenDaysLeave.Value.ToString());
        var isOutOfControlReason = ReturnValueOrEmptyString(discipline.IsOutOfControlReason, Labels.Yes);

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

    private static string ReturnValueOrEmptyString(bool condition, string value)
    {
        return condition ? value : string.Empty;
    }
}
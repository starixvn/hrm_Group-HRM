namespace HRM.Services;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Core;
using Criteria.Discipline;
using Microsoft.AspNetCore.Hosting;
using Models.Discipline;
using Resources;

[SuppressMessage("Major Code Smell", "S109:Magic numbers should not be used", Justification = "Pending")]
public class ExportDisciplineSummaryService(IWebHostEnvironment hostingEnvironment)
    : IExportDisciplineSummaryService
{
    private const string TemplatePath = @"ExcelTemplates\DisciplineSummaryReport.xls";
    private const int HeaderRow = 2;
    private const int BodyRow = 3;
    private const int TotalCol = 20;


    public byte[] Export(GetDisciplineSummaryCriteria criteria, IList<DisciplineSummaryModel> disciplines)
    {
        string templatePath = Path.Combine(hostingEnvironment.ContentRootPath, TemplatePath); 
        string reportName = Labels.DisciplineSummaryReportName;
        string reportTitle;
        string startDate = criteria.FromDate.ToDateMonthYearFormatedDate();
        string endDate = criteria.ToDate.ToDateMonthYearFormatedDate();

        if (criteria.EmployeeId != null)
        {
            reportTitle = string.Format(Labels.DisciplineSummaryReportTitle, criteria.EmployeeName, startDate, endDate);
        }
        else if (criteria.TeamId != null)
        {
            reportTitle = string.Format(Labels.DisciplineSummaryReportTitle, criteria.TeamName, startDate, endDate);
        }
        else
        {
            reportTitle = string.Format(Labels.DisciplineSummaryReportTitleNoPrefix, startDate, endDate);
        }

        var writer = new ExcelTemplateWriter(templatePath, reportName, Labels.DisciplineSummaryReportSheet);

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

    private void FillContentRow(IList<DisciplineSummaryModel> disciplines, ref ExcelTemplateWriter writer)
    {
        int rowIndex = 1;
        foreach (DisciplineSummaryModel discipline in disciplines)
        {
            FillRow(discipline, ref writer, ref rowIndex);
        }
    }

    private void FillRow(DisciplineSummaryModel discipline, ref ExcelTemplateWriter writer, ref int rowIndex)
    {
        object record = CreateRecord(discipline, ref rowIndex);
        writer.WriteRow(BodyRow, record);
    }

    private object CreateRecord(DisciplineSummaryModel discipline, ref int rowIndex)
    {
        float otherDisciplines = discipline.OtherDisciplineScore > 0 ? -discipline.OtherDisciplineScore : 0;
        float systemSubtractScore = discipline.SystemSubtractScore > 0 ? -discipline.SystemSubtractScore : 0;
        float approverSubtractScore = discipline.ApproverSubtractScore > 0 ? -discipline.ApproverSubtractScore : 0;

        var record = new
        {
            Count = rowIndex,
            discipline.Employee.TeamName,
            discipline.Employee.Code,
            discipline.Employee.NickName,
            discipline.Employee.FullName,
            Category = discipline.Employee.IsLocal ? Labels.Local : Labels.Expat,
            discipline.MissingCheckInOut,
            discipline.WorkingUnderTime,
            discipline.LateIn,
            discipline.EarlyOut,
            discipline.Absent,
            discipline.UnpaidLeave,
            discipline.UnpaidLeaveAbsent,
            discipline.ChangeShift,
            discipline.UnplannedLeave,
            discipline.WeekendLeave,
            discipline.SevenDaysLeave,
            OtherDiscipline = (decimal)otherDisciplines,
            SystemSubtractScore = (decimal)systemSubtractScore,
            ApproverSubtractScore = (decimal)approverSubtractScore
        };
        rowIndex++;

        return record;
    }
}
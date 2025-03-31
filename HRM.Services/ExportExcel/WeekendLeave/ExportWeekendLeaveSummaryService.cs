using System.Collections.Generic;
using System.IO;
using HRM.Core;
using HRM.Criteria.Report;
using HRM.Models.Report;
using HRM.Resources;
using Microsoft.AspNetCore.Hosting;

namespace HRM.Services.ExportExcel.WeekendLeave;

public interface IExportWeekendLeaveSummaryService
{
    byte[] Export(GetWeekendLeaveSummaryCriteria criteria, IList<WeekendLeaveSummary> weekendLeaves);
}

public class ExportWeekendLeaveSummaryService(IWebHostEnvironment hostingEnvironment)
    : IExportWeekendLeaveSummaryService
{
    private const string WeekendLeaveSummaryTemplatePath = @"ExcelTemplates/WeekendLeaveSummaryTemplate.xls";
    private const int HeaderRow = 3;
    private const int BodyRow = 4;
    private const int TotalCol = 7;

    public byte[] Export(GetWeekendLeaveSummaryCriteria criteria, IList<WeekendLeaveSummary> weekendLeaves)
    {
        var templatePath = Path.Combine(hostingEnvironment.ContentRootPath, WeekendLeaveSummaryTemplatePath); 
        string reportTitle;
        var startDate = criteria.FromDate.ToDateMonthYearFormatedDate();
        var endDate = criteria.ToDate.ToDateMonthYearFormatedDate();

        if (criteria.EmployeeId != null)
        {
            reportTitle = string.Format(Labels.WeekendLeaveSummary, criteria.EmployeeName, startDate, endDate);
        }
        else if (criteria.TeamId != null)
        {
            reportTitle = string.Format(Labels.WeekendLeaveSummary, criteria.TeamName, startDate, endDate);
        }
        else
        {
            reportTitle = string.Format(Labels.WeekendLeaveSummary, startDate, endDate);
        }

        var writer = new ExcelTemplateWriter(templatePath, Labels.WeekendLeave);

        FillHeaderRows(ref writer, criteria, reportTitle);

        FillContentRow(weekendLeaves, ref writer);
        writer.SyncColumnsWidth(TotalCol);
        writer.CreateFreezePane(0, BodyRow);

        return writer.WriteContentToBytes();
    }

    private static void FillHeaderRows(ref ExcelTemplateWriter writer, GetWeekendLeaveSummaryCriteria criteria, string reportTitle)
    {
        var dateRange = new
        {
            string.Empty,
            FromDateLabel = Labels.FromDate,
            FromDate = criteria.FromDate.ToDateMonthYearFormatedDate(),
            ToDateLable = Labels.ToDate,
            ToDate = criteria.ToDate.ToDateMonthYearFormatedDate()
        };

        writer.WriteRow(0, [reportTitle]);
        writer.WriteRow(1, dateRange);
        writer.CopyRows(1, 1);

        writer.CopyRows(HeaderRow, 1);
    }

    private static void FillContentRow(IList<WeekendLeaveSummary> weekendLeaves, ref ExcelTemplateWriter writer)
    {
        var rowIndex = 1;

        foreach (var item in weekendLeaves)
        {
            FillRow(item, ref writer, ref rowIndex);
        }
    }

    private static void FillRow(WeekendLeaveSummary weekendLeave, ref ExcelTemplateWriter writer, ref int rowIndex)
    {
        var record = CreateRecord(weekendLeave, ref rowIndex);
        writer.WriteRow(BodyRow, record);
    }

    private static object CreateRecord(WeekendLeaveSummary weekendLeave, ref int rowIndex)
    {
        var totalWeekendLeaveDays = weekendLeave.TotalWeekendLeave > 0 ? weekendLeave.TotalWeekendLeave : 0;
        var record = new
        {
            Count = rowIndex,
            weekendLeave.Code,
            weekendLeave.FullName,
            Nick = weekendLeave.NickName,
            Team = weekendLeave.TeamName,
            Category = weekendLeave.IsLocal ? Labels.Local : Labels.Expat,
            WeekendLeave = totalWeekendLeaveDays
        };
        rowIndex++;

        return record;
    }
}
namespace HRM.Services.ExportExcel;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Core;
using Models;
using Microsoft.AspNetCore.Hosting;
using Implement;

public interface IExportExcelUnpaidLeaveSummaryReportService
{
    Task<byte[]> Export(IEnumerable<LeaveSummary> leaveSummaries, int year, string leaveType, string period, short companyId);
}

public class ExportExcelUnpaidLeaveSummaryReportService(
    ICommonDataService commonDataService,
    IWebHostEnvironment hostingEnvironment) : IExportExcelUnpaidLeaveSummaryReportService
{
    private const int RowStartHeader = 2;
    private const int RowStartBody = 4;
    private const string UnpaidLeaveSummaryTemplatePath = @"ExcelTemplates/UnpaidLeaveSummaryTemplate.xls";
    private const string UnpaidLeaveSummaryH1TemplatePath = @"ExcelTemplates/UnpaidLeaveSummaryH1Template.xls";
    private const string UnpaidLeaveSummaryH2TemplatePath = @"ExcelTemplates/UnpaidLeaveSummaryH2Template.xls";

    public async Task<byte[]> Export(IEnumerable<LeaveSummary> leaveSummaries, int year, string leaveType, string period, short companyId)
    {
        string path;
        int range;
        switch (period)
        {
            case "1":
                path = UnpaidLeaveSummaryH1TemplatePath;
                range = 0;
                break;
            case "2":
                path = UnpaidLeaveSummaryH2TemplatePath;
                range = 0;
                break;
            default:
                path = UnpaidLeaveSummaryTemplatePath;
                range = 6;
                break;
        }
        var templatePath = Path.Combine(hostingEnvironment.ContentRootPath, path);

        var leaveTypeIds = !string.IsNullOrWhiteSpace(leaveType) ? leaveType.Split(',').ToList() : [];
        var leaveTypes = await commonDataService.GetLeaveTypeAsync();
        var type = string.Join(", ", leaveTypes.Where(x => leaveTypeIds.Exists(y => x.Id.ToString() == y)).Select(x => x.Name));

        var writer = new ExcelTemplateWriter(templatePath, "Unpaid Leave Summary");
        // Fill report title.
        writer.CopyRows(0, 2);

        // Fill report header.
        writer.CopyRows(RowStartHeader, 2);
        writer.WriteCellValue(RowStartHeader, 6, $"Take {type} {year}");
        writer.WriteCellValue(RowStartHeader, 11 + range, $"Total Of {type} {year}");

        FillContentRowReport(leaveSummaries, ref writer, period);

        writer.SyncColumnsWidth(19);
        writer.CreateFreezePane(0, RowStartBody);

        return writer.WriteContentToBytes();
    }

    private static void FillContentRowReport(IEnumerable<LeaveSummary> leaveSummaries, ref ExcelTemplateWriter writer, string period)
    {
        var rowIndex = 0;
        foreach (var leaveSummary in leaveSummaries)
        {
            FillRowReport(leaveSummary, ref writer, ref rowIndex, period);
        }
    }

    private static void FillRowReport(LeaveSummary leaveSummary, ref ExcelTemplateWriter writer, ref int rowIndex, string period)
    {
        var record = CreateRecordReport(leaveSummary, rowIndex, period);
        ExportExcelService.WriteRowExcelReport(writer, rowIndex, RowStartBody, record);
        rowIndex++;
    }

    private static object CreateRecordReport(LeaveSummary leaveSummary, int rowIndex, string period)
    {
        return period switch
        {
            "1" => new
            {
                Count = rowIndex + 1,
                leaveSummary.Department,
                leaveSummary.Team,
                leaveSummary.FullName,
                leaveSummary.Nickname,
                Jan = leaveSummary.Jan * 1.00,
                Feb = leaveSummary.Feb * 1.00,
                Mar = leaveSummary.Mar * 1.00,
                Apr = leaveSummary.Apr * 1.00,
                May = leaveSummary.May * 1.00,
                Jun = leaveSummary.Jun * 1.00,
                LeaveBalance = (leaveSummary.Jan + leaveSummary.Feb + leaveSummary.Mar + leaveSummary.Apr + leaveSummary.May + leaveSummary.Jun) * 1.00
            },
            "2" => new
            {
                Count = rowIndex + 1,
                leaveSummary.Department,
                leaveSummary.Team,
                leaveSummary.FullName,
                leaveSummary.Nickname,
                Jul = leaveSummary.Jul * 1.00,
                Aug = leaveSummary.Aug * 1.00,
                Sep = leaveSummary.Sep * 1.00,
                Oct = leaveSummary.Oct * 1.00,
                Nov = leaveSummary.Nov * 1.00,
                Dec = leaveSummary.Dec * 1.00,
                LeaveBalance = (leaveSummary.Jul + leaveSummary.Aug + leaveSummary.Sep + leaveSummary.Oct + leaveSummary.Nov + leaveSummary.Dec) * 1.00
            },
            _ => new
            {
                Count = rowIndex + 1,
                leaveSummary.Department,
                leaveSummary.Team,
                leaveSummary.FullName,
                leaveSummary.Nickname,
                Jan = leaveSummary.Jan * 1.00,
                Feb = leaveSummary.Feb * 1.00,
                Mar = leaveSummary.Mar * 1.00,
                Apr = leaveSummary.Apr * 1.00,
                May = leaveSummary.May * 1.00,
                Jun = leaveSummary.Jun * 1.00,
                Jul = leaveSummary.Jul * 1.00,
                Aug = leaveSummary.Aug * 1.00,
                Sep = leaveSummary.Sep * 1.00,
                Oct = leaveSummary.Oct * 1.00,
                Nov = leaveSummary.Nov * 1.00,
                Dec = leaveSummary.Dec * 1.00,
                LeaveBalance = leaveSummary.LeaveBalance * 1.00
            }
        };
    }
}
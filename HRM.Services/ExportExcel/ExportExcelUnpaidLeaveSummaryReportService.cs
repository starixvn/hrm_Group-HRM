namespace HRM.Services
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Web;
    using HRM.Core;
    using HRM.Models;

    public interface IExportExcelUnpaidLeaveSummaryReportService
    {
        void Export(IList<LeaveSummary> leaveSummaries, int year, string leaveType, string period);
    }

    [SuppressMessage("Major Code Smell", "S109:Magic numbers should not be used", Justification = "Pending")]
    public class ExportExcelUnpaidLeaveSummaryReportService : IExportExcelUnpaidLeaveSummaryReportService
    {
        private readonly ICommonDataService commonDataService;

        private const int RowStartHeader = 2;
        private const int RowStartBody = 4;
        private const string UnpaidLeaveSummaryTemplatePath = @"~\ExcelTemplates\UnpaidLeaveSummaryTemplate.xls";
        private const string UnpaidLeaveSummaryH1TemplatePath = @"~\ExcelTemplates\UnpaidLeaveSummaryH1Template.xls";
        private const string UnpaidLeaveSummaryH2TemplatePath = @"~\ExcelTemplates\UnpaidLeaveSummaryH2Template.xls";

        public ExportExcelUnpaidLeaveSummaryReportService(ICommonDataService commonDataService)
        {
            this.commonDataService = commonDataService;
        }

        public void Export(IList<LeaveSummary> leaveSummaries, int year, string leaveType, string period)
        {
            string path = "";
            int range = 0;
            if (period == "1")
            {
                path = UnpaidLeaveSummaryH1TemplatePath;
                range = 0;
            }
            else if (period == "2")
            {
                path = UnpaidLeaveSummaryH2TemplatePath;
                range = 0;
            }
            else
            {
                path = UnpaidLeaveSummaryTemplatePath;
                range = 6;
            }
            string templatePath = HttpContext.Current.Server.MapPath(path);
            string fileName = "UnpaidLeaveSummary_" + DateTimeOffset.Now.ToString("yyyyMMdd");

            var leaveTypeIds = !string.IsNullOrWhiteSpace(leaveType) ? leaveType.Split(',').ToList() : new List<string>();
            IList<LeaveType> leaveTypes = commonDataService.GetLeaveType();
            string type = string.Join(", ", leaveTypes.Where(x => leaveTypeIds.Exists(y => x.Id.ToString() == y)).Select(x => x.Name));

            var writer = new ExcelTemplateWriter(templatePath, fileName, "Unpaid Leave Summary Report");
            /// Fill report title.
            writer.CopyRows(0, 2);

            /// Fill report header.
            writer.CopyRows(RowStartHeader, 2);
            writer.WriteCellValue(RowStartHeader, 6, $"Take {type} {year}");
            writer.WriteCellValue(RowStartHeader, 12 + range, $"Total Of {type} {year}");

            FillContentRowReport(leaveSummaries, ref writer, period);

            writer.SyncColumnsWidth(19);
            writer.CreateFreezePane(0, RowStartBody);

            writer.Render();
        }

        private void FillContentRowReport(IList<LeaveSummary> leaveSummaries, ref ExcelTemplateWriter writer, string period)
        {
            int rowIndex = 0;
            foreach (LeaveSummary leaveSummary in leaveSummaries)
            {
                FillRowReport(leaveSummary, ref writer, ref rowIndex, period);
            }
        }

        private void FillRowReport(LeaveSummary leaveSummary, ref ExcelTemplateWriter writer, ref int rowIndex, string period)
        {
            object record = CreateRecordReport(leaveSummary, rowIndex, period);
            ExportExcelService.WriteRowExcelReport(writer, rowIndex, RowStartBody, record);
            rowIndex++;
        }

        private object CreateRecordReport(LeaveSummary leaveSummary, int rowIndex, string period)
        {
            if (period == "1")
            {
                return new
                {
                    Count = rowIndex + 1,
                    leaveSummary.Department,
                    leaveSummary.Section,
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
                };
            }
            else if (period == "2")
            {
                return new
                {
                    Count = rowIndex + 1,
                    leaveSummary.Department,
                    leaveSummary.Section,
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
                };
            }
            else
            {
                return new
                {
                    Count = rowIndex + 1,
                    leaveSummary.Department,
                    leaveSummary.Section,
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
                };
            }
        }
    }
}
namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Web;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Criteria.Report;
    using Fanex.NexHR.Models.Report;
    using Fanex.NexHR.Resources;

    public interface IExportWeekendLeaveDetailService
    {
        void Export(GetWeekendLeaveDetailCriteria criteria, IList<WeekendLeaveDetail> weekendLeaveDetails);
    }

    public class ExportWeekendLeaveDetailService : IExportWeekendLeaveDetailService
    {
        private const string WeekendLeaveDetailTemplatePath = @"~\ExcelTemplates\WeekendLeaveDetailTemplate.xls";
        private const int HeaderRow = 3;
        private const int BodyRow = 4;
        private const int TotalCol = 9;

        public void Export(GetWeekendLeaveDetailCriteria criteria, IList<WeekendLeaveDetail> weekendLeaveDetails)
        {
            string templatePath = HttpContext.Current.Server.MapPath(WeekendLeaveDetailTemplatePath);
            string reportName = Labels.WeekendLeaveDetail;
            string reportTitle;
            string startDate = criteria.FromDate.ToDateMonthYearFormatedDate();
            string endDate = criteria.ToDate.ToDateMonthYearFormatedDate();

            if (criteria.EmployeeId != null)
            {
                reportTitle = string.Format(Labels.WeekendLeaveDetail, criteria.EmployeeName, startDate, endDate);
            }
            else if (criteria.TeamId != null)
            {
                reportTitle = string.Format(Labels.WeekendLeaveDetail, criteria.TeamName, startDate, endDate);
            }
            else
            {
                reportTitle = string.Format(Labels.WeekendLeaveDetail, startDate, endDate);
            }

            var writer = new ExcelTemplateWriter(templatePath, reportName, Labels.WeekendLeave);

            FillHeaderRows(ref writer, criteria, reportTitle);

            FillContentRow(weekendLeaveDetails, ref writer);
            writer.SyncColumnsWidth(TotalCol);
            writer.CreateFreezePane(0, BodyRow);

            writer.Render();
        }

        private void FillHeaderRows(ref ExcelTemplateWriter writer, GetWeekendLeaveDetailCriteria criteria, string reportTitle)
        {
            var dateRange = new
            {
                string.Empty,
                FromDateLabel = Labels.FromDate,
                FromDate = criteria.FromDate.ToDateMonthYearFormatedDate(),
                ToDateLable = Labels.ToDate,
                ToDate = criteria.ToDate.ToDateMonthYearFormatedDate()
            };

            writer.WriteRow(0, new object[] { reportTitle });
            writer.WriteRow(1, dateRange);
            writer.CopyRows(1, 1);

            writer.CopyRows(HeaderRow, 1);
        }

        private void FillContentRow(IList<WeekendLeaveDetail> weekendLeaveDetails, ref ExcelTemplateWriter writer)
        {
            int rowIndex = 1;

            foreach (WeekendLeaveDetail item in weekendLeaveDetails)
            {
                FillRow(item, ref writer, ref rowIndex);
            }
        }

        private void FillRow(WeekendLeaveDetail weekendLeave, ref ExcelTemplateWriter writer, ref int rowIndex)
        {
            object record = CreateRecord(weekendLeave, ref rowIndex);
            writer.WriteRow(BodyRow, record);
        }

        private object CreateRecord(WeekendLeaveDetail weekendLeave, ref int rowIndex)
        {
            double leaveDayQuantity = weekendLeave.LeaveDay > 0 ? weekendLeave.LeaveDay : 0;
            var record = new
            {
                Count = rowIndex,
                weekendLeave.Code,
                weekendLeave.FullName,
                Nick = weekendLeave.NickName,
                Team = weekendLeave.TeamName,
                Category = weekendLeave.IsLocal ? Labels.Local : Labels.Expat,
                weekendLeave.LeaveDate,
                weekendLeave.LeaveType,
                LeaveDay = leaveDayQuantity
            };
            rowIndex++;

            return record;
        }
    }
}
using System.Threading.Tasks;
using GO.UserControl.Client.Core.Session;
using HRM.Core;
using HRM.Core.Constants;
using HRM.Criteria.Time;
using HRM.Resources;
using HRM.Services.ExportExcel;
using HRM.Services.Interfaces;
using HRM.ViewModels.Report;
using Microsoft.AspNetCore.Mvc;

namespace HRM.Controllers.Export;

public class ExportTimeRecordController(
    IUserControlSession session,
    IReportService reportService,
    IExportExcelTimeRecordService exportTimeRecordService) : BaseController(session)
{
    public async Task<FileContentResult> Export(TimeRecordViewModel model)
    {
        var criteria = new GetTimeRecordCriteria
        {
            EmpId = model.EmployeeId == 0 ? null : model.EmployeeId,
            SupervisorId = session.UserId,
            TeamIds = model.TeamIds,
            FromDate = model.FromDate.ToServerTimeZone(),
            ToDate = model.ToDate.ToServerTimeZone().ToEndOfDay(),
            IsLocal = model.IsLocal
        };

        var timeRecords = await reportService.GetTimeRecords(criteria);
        var exportData = exportTimeRecordService.Export(timeRecords);

        var fileName = $"{Labels.TimeRecordReport}.xlsx";
        return File(exportData, MimeType.XLSX, fileName);
    }
}
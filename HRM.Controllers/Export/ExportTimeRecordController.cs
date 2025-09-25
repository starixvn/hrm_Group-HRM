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
    IExportExcelTimeRecordService exportTimeRecordService
) : BaseController(session)
{
    public async Task<FileContentResult> Export(TimeRecordViewModel model)
    {
        var criteria = new GetTimeRecordCriteria
        {
            EmpId = model.EmployeeId == 0 ? null : model.EmployeeId,
            SupervisorId = EmployeeSession.EmpId,
            TeamIds = model.TeamIds,
            FromDate = model.FromDate.ToServerTimeZone(),
            ToDate = model.ToDate.ToEndOfDay().ToServerTimeZone(),
            IsLocal = model.IsLocal,
            EmployeeStatusIds = model.EmployeeStatusIds
        };

        var records = await reportService.GetTimeRecordAsync(criteria);
        var exportData = exportTimeRecordService.Export(records);

        var fileName = $"{Labels.TimeRecordReport}.xlsx";
        return File(exportData, MimeType.XLSX, fileName);
    }
}

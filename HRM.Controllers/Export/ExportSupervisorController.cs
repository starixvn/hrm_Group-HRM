using System;
using System.Threading.Tasks;
using GO.UserControl.Client.Core.Session;
using HRM.Core.Constants;
using HRM.Criteria.Supervisor;
using HRM.Services.ExportExcel;
using HRM.Services.Interfaces;
using HRM.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HRM.Controllers.Export;

public class ExportSupervisorController(
    IUserControlSession userControlSession,
    IReportService reportService,
    IExportSupervisorService exportSupervisorService)
    : BaseExportController(userControlSession)
{
    public async Task<ActionResult> EmployeeSupervisor(EmployeeAndTeamSupervisorViewModel model)
    {
        var criteria = new GetSupervisorReportCriteria
        {
            EmployeeId = model.EmployeeId,
            TeamIds = model.TeamIds,
            IsLocal = model.IsLocal,
            ActionType = model.ActionType
        };
        var supervisors = await reportService.GetSupervisorReport(criteria);
        var bytes = exportSupervisorService.ExportEmployeeSupervisor(supervisors);

        return File(bytes, MimeType.XLSX, $"EmployeeSupervisor_{DateTimeOffset.Now:yyyyMMdd}.xlsx");
    }

    public async Task<FileContentResult> TeamSupervisor(EmployeeAndTeamSupervisorViewModel model)
    {
        var criteria = new GetTeamSupervisorCriteria
        {
            TeamIds = model.TeamIds,
        };
        var supervisors = await reportService.GetTeamSupervisor(criteria);
        var bytes = exportSupervisorService.ExportTeamSupervisor(supervisors);
        var fileName = $"TeamSupervisor_{DateTimeOffset.Now:yyyyMMdd}.xlsx";

        return File(bytes, MimeType.XLSX, fileName);
    }
}
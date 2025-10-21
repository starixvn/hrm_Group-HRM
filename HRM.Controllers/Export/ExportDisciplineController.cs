using System.Linq;
using System.Threading.Tasks;
using GO.UserControl.Client.Core.Services;
using GO.UserControl.Client.Core.Session;
using HRM.Core.Constants;
using HRM.Criteria.Discipline;
using HRM.Services.ExportExcel.Discipline;
using HRM.Services.Interfaces;
using HRM.ViewModels.Discipline;
using Microsoft.AspNetCore.Mvc;

namespace HRM.Controllers.Export;

public class ExportDisciplineController(
    IUserControlSession session,
    IRoleService roleService,
    IDisciplineService disciplineService,
    IExportDisciplineSummaryService exportDisciplineSummaryService,
    IExportDisciplineDetailService exportDisciplineDetailService) : BaseController(session)
{
    public async Task<IActionResult> ExportDisciplineSummaryReport(DisciplineSummaryViewModel model)
    {
        var criteria = new GetDisciplineSummaryCriteria
        {
            FromDate = model.FromDate,
            ToDate = model.ToDate,
            TeamIds = model.TeamId,
            IsLocal = model.IsLocal,
            EmployeeId = model.EmployeeId,
            ReasonId = model.ReasonId,
            ApproverUserId = EmployeeSession.EmpId,
            EmployeeName = model.Employee,
            TeamName = model.TeamName,
            CompanyId = CurrentCompanyId,
        };

        var disciplines = await disciplineService.GetDisciplineSummaryAsync(criteria);
        var data = exportDisciplineSummaryService.Export(criteria, disciplines);
        return File(data, MimeType.XLS, "DisciplineSummaryReport.xls");
    }

    public async Task<ActionResult> ExportDisciplineDetailReport(DisciplineSummaryViewModel model)
    {
        var criteria = new GetDisciplineDetailCriteria
        {
            FromDate = model.FromDate,
            ToDate = model.ToDate,
            TeamIds = model.TeamId,
            IsLocal = model.IsLocal,
            EmployeeId = model.EmployeeId,
            ReasonId = model.ReasonId,
            ApproverUserId = session.UserId,
            EmployeeName = model.Employee,
            TeamName = model.TeamName,
            CompanyId = CurrentCompanyId,
        };

        var disciplines = await disciplineService.GetDisciplineDetailAsync(criteria);
        var data = exportDisciplineDetailService.Export(criteria, disciplines);
        return File(data, MimeType.XLS, "DisciplineDetailReport.xls");
    }
}
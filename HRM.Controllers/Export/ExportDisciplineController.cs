using System.Linq;
using System.Threading.Tasks;
using GO.UserControl.Client.Core.Services;
using GO.UserControl.Client.Core.Session;
using HRM.Core.Constants;
using HRM.Criteria.Discipline;
using HRM.Resources;
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
        var roles = await roleService.GetByUserIdAsync(session.UserId);
        var criteria = new GetDisciplineSummaryCriteria
        {
            FromDate = model.FromDate,
            ToDate = model.ToDate,
            TeamIds = model.TeamId,
            IsLocal = model.IsLocal,
            EmployeeId = model.EmployeeId,
            ReasonId = model.ReasonId,
            ApproverId = session.UserId,
            EmployeeName = model.Employee,
            TeamName = model.TeamName,
            CompanyId = CurrentCompanyId,
            RoleIds = string.Join(",", roles.Select(x => x.RoleId))
        };

        var disciplines = await disciplineService.GetDisciplineSummaryAsync(criteria);
        var data = exportDisciplineSummaryService.Export(criteria, disciplines);
        var fileName = $"{Labels.DisciplineSummaryReportName}.xls";
        fileName = fileName.Replace(" ", "");
        return File(data, MimeType.XLS, fileName);
    }

    public async Task<ActionResult> ExportDisciplineDetailReport(DisciplineSummaryViewModel model)
    {
        var roles = await roleService.GetByUserIdAsync(session.UserId);
        var criteria = new GetDisciplineDetailCriteria
        {
            FromDate = model.FromDate,
            ToDate = model.ToDate,
            TeamIds = model.TeamId,
            IsLocal = model.IsLocal,
            EmployeeId = model.EmployeeId,
            ReasonId = model.ReasonId,
            ApproverId = session.UserId,
            EmployeeName = model.Employee,
            TeamName = model.TeamName,
            CompanyId = CurrentCompanyId,
            RoleIds = string.Join(",", roles.Select(x => x.RoleId))
        };

        var disciplines = await disciplineService.GetDisciplineDetailAsync(criteria);
        var data = exportDisciplineDetailService.Export(criteria, disciplines);
        var fileName = $"{Labels.DisciplineDetailReportName}.xls";

        fileName = fileName.Replace(" ", "");
        return File(data, MimeType.XLS, fileName);
    }
}
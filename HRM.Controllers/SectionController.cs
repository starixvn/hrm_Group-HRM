namespace HRM.Controllers;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Constants;
using Core.Filters;
using Criteria.Section;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Session;
using HRM.Core.Models;
using HRM.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.Section;
using Models.Unit;
using Services.Implement;
using Services.Interfaces;
using ViewModels;
using ViewModels.Section;

public class SectionController(
    IUserControlSession session,
    IUnitService unitService,
    IDepartmentService departmentService,
    ICommonDataService commonDataService,
    ISectionService sectionService,
    IBaseService baseService) : BaseController(session)
{
    [HrmPermission(Policy.Read, FunctionKey.GroupManagement)]
    public ActionResult Index()
    {
        var hasPermission = session.HasPermissionOnFunction(FunctionKey.GroupManagement, Policy.Host);

        return View(hasPermission);
    }

    [HttpPost]
    [HrmPermission(Policy.Read, FunctionKey.GroupManagement)]
    public async Task<IActionResult> GetSections([FromBody] GetSectionViewModel request)
    {
        var sections = await sectionService.GetSectionAsync(request.IsActive);
        return new JsonResult(sections);
    }

    [HrmPermission(Policy.Host, FunctionKey.GroupManagement)]
    public async Task<IActionResult> GetDepartments()
    {
        var departments = await departmentService.GetDepartmentAsync();
        return new JsonResult(departments);
    }

    [HrmPermission(Policy.Host, FunctionKey.GroupManagement)]
    public ActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.GroupManagement)]
    public async Task<IActionResult> Create([FromBody] AddSectionViewModel model)
    {
        var commandResult = new CommandResult();
        if (model == null || !ModelState.IsValid)
        {
            return new JsonResult(commandResult);
        }

        var criteria = new InsertSectionCriteria
        {
            Code = model.Code,
            Name = model.Name,
            IsActive = model.IsActive,
            DepartmentId = model.DepartmentId,
            ModifiedBy = session.UserId,
            CompanyId = CurrentCompanyId
        };
        commandResult = await baseService.ExecuteNonQueryAsync(criteria);

        return new JsonResult(commandResult);
    }

    [HrmPermission(Policy.Host, FunctionKey.GroupManagement)]
    public async Task<IActionResult> GetById(int id)
    {
        var section = await sectionService.GetByIdAsync(id);
        var departments = await departmentService.GetDepartmentAsync();
        var model = new EditSectionViewModel
        {
            SectionId = id,
            Code = section.Code,
            Name = section.Name,
            IsActive = section.IsActive,
            DepartmentId = section.Department?.Id ?? 0,
            Departments = departments
        };
        return new JsonResult(model);
    }

    [HrmPermission(Policy.Host, FunctionKey.GroupManagement)]
    public ActionResult Edit(int id)
    {
        return View(id);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.GroupManagement)]
    public async Task<IActionResult> Edit([FromBody] EditSectionViewModel model)
    {
        var commandResult = new CommandResult();
        if (model == null || !ModelState.IsValid)
        {
            return new JsonResult(commandResult);
        }

        var criteria = new UpdateSectionCriteria
        {
            SectionId = model.SectionId,
            Code = model.Code,
            Name = model.Name,
            IsActive = model.IsActive,
            DepartmentId = model.DepartmentId,
            ModifiedBy = session.UserId
        };
        commandResult = await baseService.ExecuteNonQueryAsync(criteria);

        return new JsonResult(commandResult);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.GroupManagement)]
    public async Task<IActionResult> Delete([FromRoute] DeleteSectionViewModel request)
    {
        var commandResult = await sectionService.DeleteAsync(session.UserId, request.Id);

        return new JsonResult(commandResult);
    }

    [HttpGet]
    public async Task<IActionResult> GetOrgChart()
    {
        var units = await unitService.GetUnitAsync();
        var department = await departmentService.GetDepartmentAsync();
        var teams = await commonDataService.GetAllTeamAsync();
        var sections = await sectionService.GetSectionAsync();

        var orgchart = GetOrgChart(units, department, teams, sections);

        return Json(orgchart);
    }

    private static IEnumerable<MultiSelectViewModel> GetOrgChart(
        IList<Unit> units,
        IList<Department> department,
        IEnumerable<Team> teams,
        IList<Section> sections)
    {
        var orgCharts = units.Select(u => new MultiSelectViewModel
        {
            Id = u.Id,
            Text = u.Name,
            Items = department.Where(d => d.Unit != null && d.Unit.Id == u.Id).Select(d => new MultiSelectViewModel
            {
                Id = d.Id,
                Text = d.Name,
                ParentId = u.Id,
                Items = sections.Where(s => s.Department != null && s.Department.Id == d.Id).Select(s => new MultiSelectViewModel
                {
                    Id = s.Id,
                    Text = s.Name,
                    ParentId = d.Id,
                    Items = teams.Where(t => t.Section != null && t.Section.Id == s.Id).Select(t => new MultiSelectViewModel
                    {
                        Id = t.Id,
                        Text = t.Name,
                        IsLeaf = true,
                        ParentId = s.Id
                    }).ToList()
                }).ToList()
            }).ToList()
        });

        return orgCharts;
    }
}
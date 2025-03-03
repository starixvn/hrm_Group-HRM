using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Session;
using HRM.Core.Constants;
using HRM.Core.Filters;
using HRM.Core.Services;
using HRM.Core;
using HRM.Core.Models;
using HRM.Criteria.Section;
using HRM.Models;
using HRM.Services;
using HRM.Services.Implement;
using HRM.Services.Interfaces;
using HRM.ViewModels;
using HRM.ViewModels.Section;
using Microsoft.AspNetCore.Mvc;

namespace HRM.Controllers;

public class SectionController(
    IUserControlSession userControlSession,
    IUnitService unitService,
    IDepartmentService departmentService,
    ICommonDataService commonDataService,
    ISectionService sectionService,
    IBaseService baseService) : BaseController(userControlSession)
{
    [HrmPermission(Policy.Read, FunctionKey.SectionManagement)]
    public ActionResult Index()
    {
        var hasPermission = userControlSession.HasPermissionOnFunction(FunctionKey.SectionManagement, Policy.Host);

        return View(hasPermission);
    }

    [HttpPost]
    [HrmPermission(Policy.Read, FunctionKey.SectionManagement)]
    public IActionResult GetSections([FromBody] GetSectionViewModel request)
    {
        var sections = sectionService.GetSection(request.IsActive);
        return new CustomJsonResult(sections);
    }

    [HrmPermission(Policy.Host, FunctionKey.SectionManagement)]
    public IActionResult GetDepartments()
    {
        var departments = departmentService.GetDepartment();
        return new CustomJsonResult(departments);
    }

    [HrmPermission(Policy.Host, FunctionKey.SectionManagement)]
    public ActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.SectionManagement)]
    public async Task<IActionResult> Create([FromBody] AddSectionViewModel model)
    {
        var commandResult = new CommandResult();
        if (model == null || !ModelState.IsValid) return new CustomJsonResult(commandResult);

        var criteria = new InsertSectionCriteria
        {
            Code = model.Code,
            Name = model.Name,
            IsActive = model.IsActive,
            DepartmentId = model.DepartmentId,
            ModifiedBy = EmployeeSession.EmpId
        };
        commandResult = await baseService.ExecuteNonQueryAsync(criteria);

        return new CustomJsonResult(commandResult);
    }

    [HrmPermission(Policy.Host, FunctionKey.SectionManagement)]
    public IActionResult GetById(int id)
    {
        var section = sectionService.GetById(id);
        var departments = departmentService.GetDepartment();
        var model = new EditSectionViewModel
        {
            SectionId = id,
            Code = section.Code,
            Name = section.Name,
            IsActive = section.IsActive,
            DepartmentId = section.Department?.Id ?? 0,
            Departments = departments
        };
        return new CustomJsonResult(model);
    }

    [HrmPermission(Policy.Host, FunctionKey.SectionManagement)]
    public ActionResult Edit(int id)
    {
        return View(id);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.SectionManagement)]
    public IActionResult Edit([FromBody] EditSectionViewModel model)
    {
        var commandResult = new CommandResult();
        if (model == null || !ModelState.IsValid) return new CustomJsonResult(commandResult);

        var criteria = new UpdateSectionCriteria
        {
            SectionId = model.SectionId,
            Code = model.Code,
            Name = model.Name,
            IsActive = model.IsActive,
            DepartmentId = model.DepartmentId,
            ModifiedBy = EmployeeSession.EmpId
        };
        commandResult = baseService.ExecuteNonQuery(criteria);

        return new CustomJsonResult(commandResult);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.SectionManagement)]
    public IActionResult Delete([FromRoute] DeleteSectionViewModel request)
    {
        var commandResult = sectionService.Delete(EmployeeSession.EmpId, request.Id);

        return new CustomJsonResult(commandResult);
    }

    [HttpGet]
    public async Task<IActionResult> GetOrgChart()
    {
        var units = unitService.GetUnit();
        var department = departmentService.GetDepartment();
        var teams = await commonDataService.GetAllTeamAsync();
        var sections = sectionService.GetSection();

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
                        ParentId = s.Id,
                    }).ToList()
                }).ToList()
            }).ToList()
        });

        return orgCharts;
    }
}
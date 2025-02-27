using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GO.UserControl.Client.Core.Session;
using HRM.Models;
using HRM.Services;
using HRM.Services.Interfaces;
using HRM.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HRM.Controllers;

public class SectionController(
    IUserControlSession userControlSession,
    IUnitService unitService,
    IDepartmentService departmentService,
    ICommonDataService commonDataService,
    ISectionService sectionService) : BaseController(userControlSession)
{
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
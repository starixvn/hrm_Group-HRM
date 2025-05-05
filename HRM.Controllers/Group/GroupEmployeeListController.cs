using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Session;
using HRM.Core.Constants;
using HRM.Core.Filters;
using HRM.Core.Services;
using HRM.Criteria.Group;
using HRM.Models;
using HRM.Models.Group;
using HRM.Models.Unit;
using HRM.Services.Implement;
using HRM.Services.Interfaces;
using HRM.ViewModels;
using HRM.ViewModels.EmployeeGroup;
using Microsoft.AspNetCore.Mvc;

namespace HRM.Controllers.Group;

public class GroupEmployeeListController(IUserControlSession session,
    ITeamService teamService,
    ICommonDataService commonDataService,
    IBaseService baseService,
    ICompanyService companyService,
    IUnitService unitService,
    IDepartmentService departmentService) : BaseController(session)
{
    [HrmPermission(Policy.Read, FunctionKey.GroupEmployeeList)]
    public async Task<IActionResult> Index(string key = null)
    {
        var teams = await teamService.GetAllAsync();
        var companies = await companyService.GetAllCompany();
        var model = new GroupEmployeeViewModel
        {
            IsEnable = true,
            CurrentPage = Constants.CurrentPage,
            ItemsPerPage = Constants.ItemsPerPage,
            TeamIds = string.Join(",", teams.Select(x => x.Id)),
            CompanyIds = string.Join(",", companies.Select(c => c.CompanyId))
        };

        return View(model);
    }

    [HttpPost]
    [HrmPermission(Policy.Read, FunctionKey.GroupEmployeeList)]
    public async Task<IActionResult> ListEmployee(GroupEmployeeViewModel model)
    {
        var employees = new List<GroupEmployeeModel>();
        if (model != null)
        {
            var criteria = new GetEmployeesInGroupScopeCriteria
            {
                Query = model.Keyword,
                TeamIds = model.TeamIds,
                EnableUser = model.IsEnable,
                IsDeleteUser = model.IsDelete,
                TitleId = model.JobTitleId,
                PageNumber = model.CurrentPage,
                PageSize = model.ItemsPerPage,
                CompanyIds = model.CompanyIds
            };
            employees = (await baseService.GetDataAsync<GroupEmployeeModel>(criteria)).ToList();

            var pagerOption = new PagerOptionModel
            {
                CurrentPage = model.CurrentPage,
                ItemsPerPage = model.ItemsPerPage,
                TotalItem = criteria.TotalRow ?? 0
            };
            ViewBag.PagerOption = pagerOption;
        }

        ViewBag.HasPermissionToEdit = session.HasPermissionOnFunction(FunctionKey.EmployeeManagement, Policy.Host);
        ViewBag.EnableGroupDate = await commonDataService.IsEnableGroupDateAsync();

        return PartialView("_ListEmployee", employees);
    }

    [HttpGet]
    public async Task<IActionResult> GetCompanyDropdownList()
    {
        var companies = await companyService.GetAllCompany();
        var dropdownList = companies.Select(c => new MultiSelectViewModel
        {
            Id = c.CompanyId,
            Text = c.Name,
            IsLeaf = true
        });

        return Json(dropdownList);                                                  
    }

    [HttpGet]
    public async Task<IActionResult> GetOrgChart()
    {
        var units = await unitService.GetUnitAsync();
        var departments = await departmentService.GetDepartmentAsync();
        var teams = await teamService.GetAllTeamByUnitScopeAsync();

        var orgchart = GetOrgChart(units, departments, teams);
        var teamIds = string.Join(",", teams?.Select(t => t.Id));

        var response = new GroupTeamMultiSelectViewModel
        {
            Data = orgchart, 
            TeamIds = teamIds
        };

        return Json(response);
    }

    private static IEnumerable<MultiSelectViewModel> GetOrgChart(
        IEnumerable<Unit> units,
        IEnumerable<Department> departments,
        IEnumerable<GroupTeamModel> teams)
    {
        departments = departments.Where(d => teams.Any(t => t.DepartmentId != null && t.DepartmentId == d.Id));
        units = units.Where(u => departments.Any(d => d.Unit != null && d.Unit.Id == u.Id)).ToList();

        var orgCharts = units.Select(u => new MultiSelectViewModel
        {
            Id = u.Id,
            Text = u.Code,
            Items = departments.Where(d => d.Unit != null && d.Unit.Id == u.Id).Select(d => new MultiSelectViewModel
            {
                Id = d.Id,
                Text = d.Name,
                ParentId = u.Id,
                Items = teams.Where(s => s.DepartmentId != null && s.DepartmentId == d.Id).Select(s => new MultiSelectViewModel
                {
                    Id = s.Id,
                    Text = s.TeamName,
                    IsLeaf = true,
                    ParentId = d.Id,
                }).ToList()
            }).ToList()
        });

        return orgCharts;
    }
}
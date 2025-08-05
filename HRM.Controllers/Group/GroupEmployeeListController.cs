using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Session;
using HRM.Core;
using HRM.Core.Constants;
using HRM.Core.Enums;
using HRM.Core.Filters;
using HRM.Core.Services;
using HRM.Criteria.Employee;
using HRM.Criteria.EmployeeWorkFromHome;
using HRM.Criteria.Group;
using HRM.Criteria.Supervisor;
using HRM.Models;
using HRM.Models.Group;
using HRM.Models.Supervisor;
using HRM.Models.Unit;
using HRM.Services.ExportExcel.Employee;
using HRM.Services.Implement;
using HRM.Services.Interfaces;
using HRM.ViewModels;
using HRM.ViewModels.EmployeeGroup;
using Microsoft.AspNetCore.Mvc;

namespace HRM.Controllers.Group;

public class GroupEmployeeListController(
    IUserControlSession session,
    ITeamService teamService,
    ICommonDataService commonDataService,
    ICompanyService companyService,
    IUnitService unitService,
    IDepartmentService departmentService,
    IExportExcelEmployeeInfoService excelService,
    IBaseService baseService,
    IEmployeeService employeeService,
    ISecondCompanyEmployeeService secondCompanyEmployeeService ) : BaseController(session)
{
    [HrmPermission(Policy.Read, FunctionKey.GroupEmployeeList)]
    public async Task<IActionResult> Index(string teamIds = null)
    {
        var teams = await teamService.GetAllAsync();
        var companies = await companyService.GetAllCompany();

        var selectedTeamIds = !string.IsNullOrEmpty(teamIds) ? teamIds : string.Join(",", teams.Select(x => x.Id));

        var model = new GroupEmployeeViewModel
        {
            IsEnable = true,
            CurrentPage = Constants.CurrentPage,
            ItemsPerPage = Constants.ItemsPerPage,
            TeamIds = selectedTeamIds,
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

        ViewBag.HasPermissionToEdit = session.HasPermissionOnFunction(FunctionKey.GroupEmployeeList, Policy.Host);
        ViewBag.EnableGroupDate = await commonDataService.IsEnableGroupDateAsync();

        return PartialView("_ListEmployee", employees);
    }

    [HrmPermission(Policy.Read, FunctionKey.GroupEmployeeList)]
    public async Task<IActionResult> ExportExcel(GroupEmployeeViewModel model, bool isEmployeeListExport)
    {
        var criteria = new GetEmployeesInGroupScopeCriteria
        {
            Query = model.Keyword,
            TeamIds = model.TeamIds,
            EnableUser = model.IsEnable,
            IsDeleteUser = model.IsDelete,
            TitleId = model.JobTitleId,
            CompanyIds = model.CompanyIds
        };

        var profiles = (await baseService.GetDataAsync<GroupEmployeeModel>(criteria)).ToList();

        if (!isEmployeeListExport)
        {
            var employeesWFH = await GetEmployeeWorkFromHome(model.TeamIds);
            var secondmentCompanyList = await secondCompanyEmployeeService.GetAllAsync();

            var wfhByEmpId = employeesWFH
                .GroupBy(e => e.EmpId)
                .ToDictionary(g => g.Key, g => g.First());


            var secondmentByEmpId = secondmentCompanyList
                .GroupBy(e => e.EmpId)
                .ToDictionary(g => g.Key, g => g.First().Location);

            foreach (var profile in profiles)
            {
                if (!int.TryParse(profile.EmpId, out var empId))
                {
                    continue;
                }

                if (wfhByEmpId.ContainsKey(empId))
                {
                    profile.Location = "WFH";
                }
                else if (secondmentByEmpId.TryGetValue(empId, out var secondmentLocation))
                {
                    profile.Location = secondmentLocation;
                }
                else
                {
                    // No matching location found; keep existing value
                }
            }
        }

        var bytes = isEmployeeListExport
            ? await excelService.ExportGroupEmployeesAsync(profiles)
            : await excelService.ExportProfitCenterReportAsync(profiles);

        var exportDate = DateTimeOffset.Now.DateTime.ToClientTimeZone().ToString("yyyyMMdd");
        var fileName = isEmployeeListExport
            ? $"GroupEmployeeList_{exportDate}.xlsx"
            : $"Profit Center Report - {exportDate}.xlsx";

        return File(bytes, MimeType.XLS, fileName);
    }

    public async Task<IEnumerable<EmployeeWorkFromHome>> GetEmployeeWorkFromHome(string TeamIds)
    {
        var criteria = new GetEmployeeWorkFromHomeCriteria
        {
            FromDate = DateTimeOffset.Now.DateTime.ToClientTimeZone(),
            ToDate = DateTimeOffset.Now.DateTime.ToClientTimeZone(),
            TeamIds = TeamIds
        };

        var employeesWFH = await baseService.GetDataAsync<EmployeeWorkFromHome>(criteria);

        return employeesWFH;
    }

    public async Task<IActionResult> GroupEmployeeList(GroupEmployeeViewModel model)
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
        }
        return Json(employees);
    }

    [HrmPermission(Policy.Host, FunctionKey.GroupEmployeeList)]
    public async Task<IActionResult> Edit(int id)
    {
        var employee = await employeeService.GetEmployeeByIdInGroupScopeAsync(id);
        if (employee != null)
        {
            var supervisors = (await GetSupervisors(id)).ToList();
            employee.Verifier = GetVerifiers(supervisors);
            employee.Approver = GetApproverList(supervisors);
            employee.Disciplinarian = GetDisciplineSupervisors(supervisors);
            employee.EmpShiftList = GetShiftMonitors(supervisors);
            return View(employee);
        }

        return RedirectToAction("PageNotFound", "Error");
    }

    [HttpGet]
    public IActionResult ShiftManager(int id)
    {
        ViewBag.EmployeeId = id;
        ViewBag.RoleId = (short)RoleSystem.ShiftManager;

        return View();
    }

    [HttpGet]
    public IActionResult ManageSupervisor(int id, short roleId)
    {
        ViewBag.EmployeeId = id;
        ViewBag.RoleId = roleId;
        return View();
    }

    [HrmPermission(Policy.Read, FunctionKey.GroupEmployeeList)]
    public IActionResult DisciplineManager(int id)
    {
        ViewBag.EmployeeId = id;
        ViewBag.RoleId = (short)RoleSystem.DisciplineManager;
        return View();
    }

    private async Task<IEnumerable<SupervisorNameModel>> GetSupervisors(int empId)
    {
        var criteria = new GetListSupervisorNameCriteria
        {
            EmpId = empId,
            Role = null,
            RequestType = null
        };
        return await employeeService.GetListSupervisorNameAsync(criteria);
    }

    private static string GetSupervisors(IEnumerable<SupervisorNameModel> supervisors, short roleId)
    {
        if (supervisors == null || !supervisors.Any())
        {
            return string.Empty;
        }

        var filteredSupervisors = supervisors
            .Where(x => x.ActionType == roleId)
            .Select(x => $"{x.NickName} ({x.CompanyName})")
            .ToList();

        var supervisorNames = string.Join(", ", filteredSupervisors);

        return supervisorNames;
    }

    public async Task<JsonResult> GetSupervisors(string roleIds, string query, string excludeIds)
    {
        var criteria = new GetEmployeeByRoleCriteria
        {
            ExcludeIds = excludeIds ?? string.Empty,
            Query = query.Trim(),
            RoleIds = roleIds
        };
        var employees = await employeeService.GetEmployeesByRoleAsync(criteria);

        return Json(employees);
    }

    private static string GetVerifiers(IEnumerable<SupervisorNameModel> supervisors)
    {
        return GetSupervisors(supervisors, (short)RoleSystem.Verifier);
    }

    private static string GetApproverList(IEnumerable<SupervisorNameModel> supervisors)
    {
        return GetSupervisors(supervisors, (short)RoleSystem.Approver);
    }

    private static string GetShiftMonitors(IEnumerable<SupervisorNameModel> supervisors)
    {
        return GetSupervisors(supervisors, (short)RoleSystem.ShiftManager);
    }

    private static string GetDisciplineSupervisors(IEnumerable<SupervisorNameModel> supervisors)
    {
        return GetSupervisors(supervisors, (short)RoleSystem.DisciplineManager);
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
    public async Task<IActionResult> GetOrgChart(bool isGroup = false)
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
        departments = departments.Where(d => teams.Any(t => t.DepartmentId == d.Id));
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
                Items = teams.Where(t => t.DepartmentId == d.Id).Select(s => new MultiSelectViewModel
                {
                    Id = s.Id,
                    Text = s.TeamName,
                    ParentId = d.Id,
                    IsLeaf = true
                }).ToList()
            }).ToList()
        });

        return orgCharts;
    }
}
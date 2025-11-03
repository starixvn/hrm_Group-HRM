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
using HRM.Criteria.GroupSpace.Employee;
using HRM.Criteria.Supervisor;
using HRM.Models;
using HRM.Models.Group;
using HRM.Models.Supervisor;
using HRM.Models.Unit;
using HRM.Services.ExportExcel.Employee;
using HRM.Services.Interfaces;
using HRM.ViewModels;
using HRM.ViewModels.EmployeeGroup;
using Microsoft.AspNetCore.Mvc;

namespace HRM.Controllers.GroupSpace;

#pragma warning disable S107 // Methods should not have too many parameters

public class GroupEmployeeListController(
    IUserControlSession session,
    ITeamService teamService,
    ICompanyService companyService,
    IUnitService unitService,
    IDepartmentService departmentService,
    IExportExcelEmployeeInfoService excelService,
    IBaseService baseService,
    IEmployeeService employeeService,
    ISecondCompanyEmployeeService secondCompanyEmployeeService,
    IMonthlyPayrollService payrollService) : BaseController(session)
#pragma warning restore S107 // Methods should not have too many parameters
{
    [HrmPermission(Policy.Read, FunctionKey.GroupEmployeeList)]
    public async Task<IActionResult> Index(string teamIds = null)
    {
        var teams = await teamService.GetAllAsync();
        var companies = await companyService.GetAllCompany();

        var selectedTeamIds = !string.IsNullOrEmpty(teamIds)
            ? teamIds
            : string.Join(",", teams.Select(x => x.Id));

        var model = new GroupEmployeeViewModel
        {
            IsEnable = true,
            CurrentPage = Constants.CurrentPage,
            ItemsPerPage = Constants.ItemsPerPage,
            TeamIds = selectedTeamIds,
            CompanyIds = string.Join(",", companies.Select(c => c.CompanyId)),
        };

        return View(model);
    }

    [HttpPost]
    [HrmPermission(Policy.Read, FunctionKey.GroupEmployeeList)]
    public async Task<IActionResult> ListEmployee(GroupEmployeeViewModel model)
    {
        ViewBag.HasPermissionToEdit = session.HasPermissionOnFunction(FunctionKey.GroupEmployeeList, Policy.Host);
        var employees = await GetGroupEmployeeList(model);

        return PartialView("_ListEmployee", employees);
    }

    private async Task<IEnumerable<GroupEmployeeModel>> GetGroupEmployeeList(GroupEmployeeViewModel model)
    {
        var (enableUser, isDeleteUser) = ParseEmployeeStatus(model.EmployeeStatus);
        var isShowSalary = session.HasPermissionOnFunction(FunctionKey.ShowSalaryCurrencyInfo, Policy.Read);

        var criteria = new GetEmployeesInGroupScopeCriteria
        {
            Query = model.Keyword,
            TeamIds = model.TeamIds,
            EnableUser = enableUser,
            IsDeleteUser = isDeleteUser,
            TitleId = model.JobTitleId,
            CompanyIds = model.CompanyIds,
            ActorId = EmployeeSession.EmpId,
            IsCtrID = ParseBool(model.CtrIDs),
            IsOutsource = ParseBool(model.Outsources)
        };
        var employees = await baseService.GetDataAsync<GroupEmployeeModel>(criteria);
        employees = FilterEmployeeByView(employees);
        if (isShowSalary)
        {
            await payrollService.CalculateMonthlySalary(employees);
        }

        ViewBag.ShowSalaryCurrency = isShowSalary;
        ViewBag.ShowResignedDate = enableUser is false || isDeleteUser is true ||
                                   (!enableUser.HasValue && !isDeleteUser.HasValue);

        return employees;
    }

    private IEnumerable<GroupEmployeeModel> FilterEmployeeByView(
        IEnumerable<GroupEmployeeModel> employees
    )
    {
        if (session.HasPermissionOnFunction(FunctionKey.GroupEmployeeList_ViewAll, Policy.Read))
        {
            return employees;
        }

        List<int> empIds = [];

        if (session.HasPermissionOnFunction(FunctionKey.GroupEmployeeList_BUView, Policy.Read))
        {
            empIds.AddRange(
                employees
                    .Where(e =>
                        e.UnitPICEmpId == EmployeeSession.EmpId
                        || (
                            !string.IsNullOrWhiteSpace(e.UnitSupporterEmpId)
                            && e.UnitSupporterEmpId.Split(
                                    ',',
                                    StringSplitOptions.RemoveEmptyEntries
                                        | StringSplitOptions.TrimEntries
                                )
                                .ToList()
                                .Exists(id => id == EmployeeSession.EmpId.ToString())
                        )
                    )
                    .Select(e => e.EmpId)
                    .ToList()
            );
        }

        if (session.HasPermissionOnFunction(FunctionKey.GroupEmployeeList_DeptView, Policy.Read))
        {
            empIds.AddRange(
                employees
                    .Where(e =>
                        e.DepartmentPICId == EmployeeSession.EmpId
                        || (
                            !string.IsNullOrWhiteSpace(e.DepartmentSupporterEmpId)
                            && e.DepartmentSupporterEmpId.Split(
                                    ',',
                                    StringSplitOptions.RemoveEmptyEntries
                                        | StringSplitOptions.TrimEntries
                                )
                                .ToList()
                                .Exists(id => id == EmployeeSession.EmpId.ToString())
                        )
                    )
                    .Select(e => e.EmpId)
                    .ToList()
            );
        }

        var distinctEmpIds = empIds.Distinct().ToList();

        return employees.Where(e => distinctEmpIds.Contains(e.EmpId));
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
                CompanyIds = model.CompanyIds,
            };
            employees = (await baseService.GetDataAsync<GroupEmployeeModel>(criteria)).ToList();
            employees = FilterEmployeeByView(employees).ToList();
        }

        return Json(employees);
    }

    private static (bool? enableUser, bool? isDeleteUser) ParseEmployeeStatus(
        string employeeStatuses
    )
    {
        return employeeStatuses switch
        {
            "1" => (true, null),
            "2" => (false, null),
            "3" => (null, true),
            _ => (null, null),
        };
    }

    private static bool? ParseBool(string value) =>
        value switch
        {
            "1" => true,
            "0" => false,
            _ => null,
        };

    [HrmPermission(Policy.Read, FunctionKey.GroupEmployeeList)]
    public async Task<IActionResult> ExportExcel(
        GroupEmployeeViewModel model,
        bool isEmployeeListExport)
    {
        var employees = await GetGroupEmployeeList(model);

        if (!isEmployeeListExport)
        {
            var employeesWFH = await GetEmployeeWorkFromHome(model.TeamIds);
            var secondments = await secondCompanyEmployeeService.GetAllAsync();

            var secondmentByEmpId = secondments
                .GroupBy(e => e.EmpId)
                .ToDictionary(g => g.Key, g => g.First().Location);

            foreach (var profile in employees)
            {
                string location;
                if (employeesWFH.Any(x => x.EmpId == profile.EmpId))
                {
                    location = "WFH";
                }
                else if (secondmentByEmpId.TryGetValue(profile.EmpId, out var secondmentLocation))
                {
                    location = secondmentLocation;
                }
                else
                {
                    location = profile.Location;
                }

                profile.Location = location;
            }
        }

        var bytes = isEmployeeListExport
            ? await excelService.ExportGroupEmployeesAsync(employees)
            : await excelService.ExportProfitCenterReportAsync(employees);

        var exportDate = DateTimeOffset.Now.DateTime.ToClientTimeZone().ToString("yyyyMMdd");
        var fileName = isEmployeeListExport
            ? $"GroupEmployeeList_{exportDate}.xlsx"
            : $"Profit Center Report - {exportDate}.xlsx";

        return File(bytes, MimeType.XLS, fileName);
    }

    private async Task<IEnumerable<EmployeeWorkFromHome>> GetEmployeeWorkFromHome(string teamIds)
    {
        var criteria = new GetEmployeeWorkFromHomeCriteria
        {
            FromDate = DateTimeOffset.Now.DateTime.ToClientTimeZone(),
            ToDate = DateTimeOffset.Now.DateTime.ToClientTimeZone(),
            TeamIds = teamIds,
        };

        return await baseService.GetDataAsync<EmployeeWorkFromHome>(criteria);
    }

    [HrmPermission(Policy.Host, FunctionKey.GroupEmployeeList)]
    public async Task<IActionResult> EditInfo(int id)
    {
        var employee = await employeeService.GetEmployeeByIdInGroupScopeAsync(id);
        if (employee != null)
        {
            var model = new UpdateEmployeeViewModel
            {
                EmpId = employee.Id,
                CtrId = employee.CtrId,
                Code = employee.Code,
                FullName = employee.FullName,
                NickName = employee.NickName,
                CompanyName = employee.CompanyName,
            };
            return View(model);
        }

        return RedirectToAction("PageNotFound", "Error");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [HrmPermission(Policy.Host, FunctionKey.GroupEmployeeList)]
    public async Task<IActionResult> EditInfo(UpdateEmployeeViewModel model)
    {
        if (ModelState.IsValid)
        {
            if (model.CtrId is < 1 or > int.MaxValue)
            {
                ModelState.AddModelError(
                    nameof(model.CtrId),
                    $"Ctr ID must be between 1 and {int.MaxValue}."
                );
            }

            var command = new UpdateEmployeeCommand
            {
                EmpId = model.EmpId,
                CtrId = model.CtrId,
                ModifiedBy = EmployeeSession.EmpId,
            };
            var result = await baseService.ExecuteNonQueryAsync(command);
            if (result.IsSuccessful)
            {
                ViewBag.Updated = true;
            }
            else
            {
                ViewBag.Updated = false;
                ModelState.AddModelError(string.Empty, result.Message);
            }
        }

        return View(model);
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
            RequestType = null,
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
            RoleIds = roleIds,
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
            IsLeaf = true,
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

        var response = new GroupTeamMultiSelectViewModel { Data = orgchart, TeamIds = teamIds };

        return Json(response);
    }

    private static IEnumerable<MultiSelectViewModel> GetOrgChart(
        IEnumerable<Unit> units,
        IEnumerable<Department> departments,
        IEnumerable<GroupTeamModel> teams
    )
    {
        departments = departments.Where(d => teams.Any(t => t.DepartmentId == d.Id));
        units = units
            .Where(u => departments.Any(d => d.Unit != null && d.Unit.Id == u.Id))
            .ToList();

        var orgCharts = units.Select(u => new MultiSelectViewModel
        {
            Id = u.Id,
            Text = u.Code,
            Items = departments
                .Where(d => d.Unit != null && d.Unit.Id == u.Id)
                .Select(d => new MultiSelectViewModel
                {
                    Id = d.Id,
                    Text = d.Name,
                    ParentId = u.Id,
                    Items = teams
                        .Where(t => t.DepartmentId == d.Id)
                        .Select(s => new MultiSelectViewModel
                        {
                            Id = s.Id,
                            Text = s.TeamName,
                            ParentId = d.Id,
                            IsLeaf = true,
                        })
                        .ToList(),
                })
                .ToList(),
        });

        return orgCharts;
    }
}

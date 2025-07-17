using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Session;
using HRM.Core.Constants;
using HRM.Core.Enums;
using HRM.Core.Filters;
using HRM.Core.Services;
using HRM.Criteria.Employee;
using HRM.Criteria.Group;
using HRM.Models;
using HRM.Models.Group;
using HRM.Models.Supervisor;
using HRM.Models.Unit;
using HRM.Resources;
using HRM.Services.Implement;
using HRM.Services.Interfaces;
using HRM.ViewModels;
using HRM.ViewModels.EmployeeGroup;
using HRM.ViewModels.Team;
using Microsoft.AspNetCore.Mvc;

namespace HRM.Controllers;

public class TeamController(
    IUserControlSession session,
    IUnitService unitService,
    IDepartmentService departmentService,
    ICommonDataService commonDataService,
    ITeamService teamService,
    IBaseService baseService,
    IEmployeeService employeeService) : BaseController(session)
{
    [HrmPermission(Policy.Read, FunctionKey.TeamManagement)]
    public async Task<ActionResult> Index(string keyword = null)
    {
        var teams = await teamService.GetAllAsync(keyword: keyword);
        ViewBag.HasPermission = session.HasPermissionOnFunction(FunctionKey.TeamManagement, Policy.Host);
        ViewBag.HasPermissionToViewEmployee = session.HasPermissionOnFunction(FunctionKey.EmployeeManagement, Policy.Read);
        ViewBag.CompanyId = CurrentCompanyId;
        return View(teams);
    }

    [HrmPermission(Policy.Host, FunctionKey.TeamManagement)]
    public async Task<ActionResult> Edit(int id)
    {
        var team = await teamService.GetById(id);
        if (team == null)
        {
            return RedirectToAction("PageNotFound", "Error");
        }

        var employees = await GetAllEmployees();
        var model = new TeamViewModel
        {
            Id = team.Id,
            TeamCode = team.Code,
            TeamName = team.Name,
            MailGroup = team.MailGroup,
            LocalBasicAnnualLeaveBalance = GetValue(team.LocalBasicAnnualLeaveBalance),
            LocalWorkingDays = GetValue(team.LocalWorkingDays),
            LocalWorkingHours = GetValue(team.LocalWorkingHours),
            LocalOffDaysbyWeek = GetValue(team.LocalOffDaysbyWeek),
            LocalWorkingHoursbyWeek = team.LocalWorkingHoursbyWeek,
            LocalYearsOfServiceForExtraLeave = team.LocalYearsOfServiceForExtraLeave,
            ExpatBasicAnnualLeaveBalance = GetValue(team.ExpatBasicAnnualLeaveBalance),
            ExpatWorkingDays = GetValue(team.ExpatWorkingDays),
            ExpatWorkingHours = GetValue(team.ExpatWorkingHours),
            ExpatOffDaysbyWeek = GetValue(team.ExpatOffDaysbyWeek),
            ExpatWorkingHoursbyWeek = team.ExpatWorkingHoursbyWeek,
            ExpatYearsOfServiceForExtraLeave = team.ExpatYearsOfServiceForExtraLeave,
            DepartmentId = team.Department?.Id ?? 0,
            Departments = await departmentService.GetDepartmentAsync(),
            DepartmentName = team.Department?.Name ?? "",
            Employees = employees,
            PICEmpId = team.PICEmpId,
            GroupTeamId = team.GroupTeamId,
            AccountableArea = team.AccountableArea,
            IsOutsource = team.IsOutsource
        };

        return View(model);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.TeamManagement)]
    public async Task<ActionResult> Edit(TeamViewModel model)
    {
        if (model != null)
        {
            model.Departments = await departmentService.GetDepartmentAsync();
            model.Employees = await GetAllEmployees();
            var team = await teamService.GetGroupTeamInUnitScopeById(model.Id);
            model.TeamCode = team.TeamCode;
            model.TeamName = team.TeamName;
            model.DepartmentName = team.DepartmentName;
            if (ModelState.IsValid)
            {
                var criteria = new EditTeamCriteria
                {
                    TeamId = team.Id,
                    Code = team.TeamCode,
                    Name = team.TeamName,
                    DepartmentId = team.DepartmentId,
                    PICEmpId = team.PICEmpId,
                    AccountableArea = team.AccountableArea,
                    GroupTeamId = team.GroupTeamId,
                    ModifiedBy = session.UserId,
                    MailGroup = model.MailGroup,
                    LocalBasicAnnualLeaveBalance = GetValue(model.LocalBasicAnnualLeaveBalance),
                    LocalWorkingDays = GetValue(model.LocalWorkingDays),
                    LocalWorkingHours = GetValue(model.LocalWorkingHours),
                    LocalOffDaysbyWeek = GetValue(model.LocalOffDaysbyWeek),
                    LocalWorkingHoursbyWeek = model.LocalWorkingHoursbyWeek,
                    LocalYearsOfServiceForExtraLeave = model.LocalYearsOfServiceForExtraLeave,
                    ExpatBasicAnnualLeaveBalance = GetValue(model.ExpatBasicAnnualLeaveBalance),
                    ExpatWorkingDays = GetValue(model.ExpatWorkingDays),
                    ExpatWorkingHours = GetValue(model.ExpatWorkingHours),
                    ExpatOffDaysbyWeek = GetValue(model.ExpatOffDaysbyWeek),
                    ExpatWorkingHoursbyWeek = model.ExpatWorkingHoursbyWeek,
                    ExpatYearsOfServiceForExtraLeave = model.ExpatYearsOfServiceForExtraLeave,
                    IsOutsource = model.IsOutsource
                };
                var result = await baseService.ExecuteNonQueryAsync(criteria);

                if (result.IsSuccessful)
                {
                    ViewBag.Updated = true;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, Messages.InvalidInputData);
            }
        }

        return View(model);
    }



    [HrmPermission(Policy.Read, FunctionKey.TeamManagement)]
    public async Task<IActionResult> TeamSupervisor(int id)
    {
        const bool isGroupSpace = false;
        var supervisors = (await teamService.GetSupervisorByTeamId(id, isGroupSpace)).ToList();
        ViewBag.TeamId = id;
        ViewBag.TeamName = supervisors.Count != 0 ? supervisors[0].Name : string.Empty;

        return View(supervisors);
    }

    [HrmPermission(Policy.Host, FunctionKey.TeamManagement)]
    public IActionResult ManageSupervisor(int id, short roleId)
    {
        ViewBag.TeamId = id;
        ViewBag.RoleId = roleId;
        ViewBag.CompanyId = CurrentCompanyId;

        return View();
    }

    [HrmPermission(Policy.Host, FunctionKey.TeamManagement)]
    public async Task<IActionResult> GetTeamSupervisorByRoleId(int teamId, short roleId)
    {
        IEnumerable<SupervisorModel> supervisors = await teamService.GetTeamSupervisorByRoleId(teamId, roleId);

        return Json(supervisors);
    }

    [HrmPermission(Policy.Host, FunctionKey.TeamManagement)]
    public IActionResult ShiftManager(int id)
    {
        ViewBag.TeamId = id;
        ViewBag.RoleId = (short)RoleSystem.ShiftManager;

        return View();
    }

    [HrmPermission(Policy.Host, FunctionKey.TeamManagement)]
    public async Task<IActionResult> GetShiftManagerByTeamId(int teamId)
    {
        IEnumerable<SupervisorModel> supervisors = await teamService.GetTeamSupervisorByRoleId(teamId, (short)RoleSystem.ShiftManager);
        var result = supervisors.Select(x => new Supervisor
        { OwnerId = x.OwnerId, Nickname = x.NickName, FullName = x.FullName });
        return Json(result);
    }

    [HrmPermission(Policy.Host, FunctionKey.TeamManagement)]
    public IActionResult DisciplineManager(int id)
    {
        ViewBag.TeamId = id;
        ViewBag.RoleId = (short)RoleSystem.DisciplineManager;
        return View();
    }

    [HrmPermission(Policy.Read, FunctionKey.TeamManagement)]
    public async Task<IActionResult> GetDisciplineManagerByTeamId(int teamId)
    {
        var supervisors = await teamService.GetDisciplineManagerByTeamId(teamId);
        return Json(supervisors);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.TeamManagement)]
    public async Task<IActionResult> EditTeamSupervisor(int teamId, short roleId, IEnumerable<EditTeamSupervisor> supervisors)
    {
        var commandResult = await teamService.EditTeamSupervisor(session.UserId, teamId, roleId, supervisors);

        return Json(commandResult);
    }

    [HttpGet]
    public async Task<IActionResult> DisciplineRule(int id)
    {
        var team = await teamService.GetById(id);
        if (team == null)
        {
            return RedirectToAction("PageNotFound", "Error");
        }

        var model = new DisciplineRuleViewModel
        {
            TeamId = team.Id,
            Team = team.Name,
            IsHost = session.HasPermissionOnFunction(FunctionKey.TeamManagement, Policy.Host),
            DisciplineRules = await teamService.GetDisciplineRuleByTeam(id)
        };
        return View(model);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.TeamManagement)]
    public async Task<IActionResult> EditDisciplineRule(int teamId, IEnumerable<DisciplineRule> disciplineRules)
    {
        var commandResult = await teamService.EditDisciplineRule(session.UserId, teamId, disciplineRules);

        return Json(commandResult);
    }

    private async Task<IList<Employee>> GetAllEmployees()
    {
        var criteria = new GetAllEmployeeCriteria
        {
            Query = null,
            TeamIds = null,
            EnableUser = true,
            IsDeleteUser = null,
            TitleId = null,
            IsLocalUser = null,
            CompanyId = CurrentCompanyId
        };
        IList<Employee> employees = (await employeeService.GetEmployeeAsync(criteria)).ToList();

        return employees;
    }

    [HttpGet]
    public async Task<IActionResult> GetOrgChart(bool isGroup = false)
    {
        var units = await unitService.GetUnitAsync();
        var departments = await departmentService.GetDepartmentAsync();
        var teams = await commonDataService.GetAllTeamAsync(isGroup: isGroup);

        var orgchart = GetOrgChart(units, departments, teams);

        return Json(orgchart);
    }

    private static IEnumerable<MultiSelectViewModel> GetOrgChart(
        IEnumerable<Unit> units,
        IEnumerable<Department> departments,
        IEnumerable<Team> teams)
    {
        departments = departments.Where(d => teams.Any(t => t.Department != null && t.Department.Id == d.Id));
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
                Items = teams.Where(s => s.Department != null && s.Department.Id == d.Id).Select(s => new MultiSelectViewModel
                {
                    Id = s.Id,
                    Text = s.Name,
                    IsLeaf = true,
                    ParentId = d.Id,
                }).ToList()
            }).ToList()
        });

        return orgCharts;
    }

    private static byte GetValue(byte? value) => value ?? 0;
}
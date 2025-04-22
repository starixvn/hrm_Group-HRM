using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Session;
using HRM.Core.Constants;
using HRM.Core.Filters;
using HRM.Core.Services;
using HRM.Criteria.Company;
using HRM.Criteria.Employee;
using HRM.Criteria.Group;
using HRM.Models;
using HRM.Resources;
using HRM.Services.Interfaces;
using HRM.ViewModels.EmployeeGroup;
using HRM.ViewModels.Team;
using Microsoft.AspNetCore.Mvc;

namespace HRM.Controllers.Group;

public class GroupTeamController(ITeamService teamService,
    IUserControlSession userControlSession,
    IDepartmentService departmentService,
    IBaseService baseService,
    IEmployeeService employeeService,
    ICompanyService companyService) : BaseController(userControlSession)
{

    [HrmPermission(Policy.Host, FunctionKey.TeamManagement)]
    public async Task<IActionResult> Index(string keyword = null)
    {
        var teams = await teamService.GetAllTeamByUnitScopeAsync(keyword);
        ViewBag.HasPermission = userControlSession.HasPermissionOnFunction(FunctionKey.TeamManagement, Policy.Host);
        ViewBag.HasPermissionToViewEmployee = userControlSession.HasPermissionOnFunction(FunctionKey.EmployeeManagement, Policy.Read);
        return View(teams);
    }

    [HrmPermission(Policy.Host, FunctionKey.TeamManagement)]
    public async Task<ActionResult> Create()
    {
        var groupTeamId = await teamService.GetNextGroupTeamIdAsync();
        var companies = await GetAllCompany();
        var employeePics = await GetAllEmployees();
        var model = new TeamViewModel
        {
            GroupTeamId = groupTeamId,
            Departments = await departmentService.GetDepartmentAsync(),
            EmployeePics = employeePics.ToList(),
            Companies = companies.ToList()
        };
        return View(model);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.TeamManagement)]
    public async Task<ActionResult> Create(TeamViewModel model)
    {
        model.Departments = await departmentService.GetDepartmentAsync();
        model.EmployeePics = (await GetAllEmployees())?.ToList();
        model.Companies = (await GetAllCompany())?.ToList();
        if (ModelState.IsValid)
        {
            var criteria = new CreateTeamCriteria
            {
                Code = model.TeamCode,
                Name = model.TeamName,
                DepartmentId = model.DepartmentId ?? 0,
                PICEmpId = model.PICEmpId,
                GroupTeamId = model.GroupTeamId,
                AccountableArea = model.AccountableArea,
                MailGroup = model.MailGroup,
                CreatedBy = session.UserId,
                LocalBasicAnnualLeaveBalance = GetValue(model.LocalBasicAnnualLeaveBalance),
                LocalWorkingDays = GetValue(model.LocalWorkingDays),
                LocalWorkingHours = GetValue(model.LocalWorkingHours),
                LocalOffDaysbyWeek = GetValue(model.LocalOffDaysbyWeek),
                LocalWorkingHoursbyWeek = model.LocalWorkingHoursbyWeek,
                ExpatBasicAnnualLeaveBalance = GetValue(model.ExpatBasicAnnualLeaveBalance),
                ExpatWorkingDays = GetValue(model.ExpatWorkingDays),
                ExpatWorkingHours = GetValue(model.ExpatWorkingHours),
                ExpatOffDaysbyWeek = GetValue(model.ExpatOffDaysbyWeek),
                ExpatWorkingHoursbyWeek = model.ExpatWorkingHoursbyWeek,
                CompanyId = (short)model.CompanyId
            };
            var result = await baseService.ExecuteNonQueryAsync(criteria);

            if (result.IsSuccessful)
            {
                return RedirectToAction("Index", "GroupTeam");
            }

            ModelState.AddModelError(string.Empty, result.Message);
        }
        else
        {
            ModelState.AddModelError(string.Empty, Messages.InvalidInputData);
        }

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> DisciplineRule(int id)
    {
        var team = await teamService.GetGroupTeamInUnitScopeById(id);
        if (team == null)
        {
            return RedirectToAction("PageNotFound", "Error");
        }

        var model = new DisciplineRuleViewModel
        {
            TeamId = team.Id,
            Team = team.TeamName,
            IsHost = session.HasPermissionOnFunction(FunctionKey.TeamManagement, Policy.Host),
            DisciplineRules = await teamService.GetDisciplineRuleByTeam(id)
        };
        return View("~/Views/Team/DisciplineRule.cshtml", model);
    }

    [HrmPermission(Policy.Host, FunctionKey.TeamManagement)]
    public async Task<ActionResult> Edit(int id)
    {
        var team = await teamService.GetGroupTeamInUnitScopeById(id);
        var companies = await GetAllCompany();
        if (team == null)
        {
            return RedirectToAction("PageNotFound", "Error");
        }
                                                                                                                                                                
        var employeePics = await GetAllEmployees();
        var model = new TeamViewModel
        {
            Id = team.Id,
            TeamCode = team.TeamCode,
            TeamName = team.TeamName,
            MailGroup = team.MailGroup,
            LocalBasicAnnualLeaveBalance = GetValue(team.LocalBasicAnnualLeaveBalance),
            LocalWorkingDays = GetValue(team.LocalWorkingDays),
            LocalWorkingHours = GetValue(team.LocalWorkingHours),
            LocalOffDaysbyWeek = GetValue(team.LocalOffDaysbyWeek),
            LocalWorkingHoursbyWeek = team.LocalWorkingHoursbyWeek,
            ExpatBasicAnnualLeaveBalance = GetValue(team.ExpatBasicAnnualLeaveBalance),
            ExpatWorkingDays = GetValue(team.ExpatWorkingDays),
            ExpatWorkingHours = GetValue(team.ExpatWorkingHours),
            ExpatOffDaysbyWeek = GetValue(team.ExpatOffDaysbyWeek),
            ExpatWorkingHoursbyWeek = team.ExpatWorkingHoursbyWeek,
            DepartmentId = team.DepartmentId,
            Departments = await departmentService.GetDepartmentAsync(),
            EmployeePics = employeePics.ToList(),
            PICEmpId = team.PICEmpId,
            GroupTeamId = team.GroupTeamId,
            AccountableArea = team.AccountableArea,
            Companies = companies.ToList(),
            CompanyId = team.CompanyId
        };

        return View(model);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.TeamManagement)]
    public async Task<ActionResult> Edit(TeamViewModel model)
    {
        if (model != null)
        {
            var team = await teamService.GetGroupTeamInUnitScopeById(model.Id);
            model.Departments = await departmentService.GetDepartmentAsync();
            model.EmployeePics = (await GetAllEmployees())?.ToList();
            model.Companies = (await GetAllCompany())?.ToList();
            model.GroupTeamId = team.GroupTeamId;
            if (ModelState.IsValid)
            {
                var criteria = new EditTeamCriteria
                {
                    TeamId = model.Id,
                    Code = model.TeamCode,
                    Name = model.TeamName,
                    DepartmentId = model.DepartmentId ?? 0,
                    MailGroup = model.MailGroup,
                    GroupTeamId = model.GroupTeamId,
                    PICEmpId = model.PICEmpId,
                    AccountableArea = model.AccountableArea,
                    ModifiedBy = session.UserId,
                    LocalBasicAnnualLeaveBalance = GetValue(model.LocalBasicAnnualLeaveBalance),
                    LocalWorkingDays = GetValue(model.LocalWorkingDays),
                    LocalWorkingHours = GetValue(model.LocalWorkingHours),
                    LocalOffDaysbyWeek = GetValue(model.LocalOffDaysbyWeek),
                    LocalWorkingHoursbyWeek = model.LocalWorkingHoursbyWeek,
                    ExpatBasicAnnualLeaveBalance = GetValue(model.ExpatBasicAnnualLeaveBalance),
                    ExpatWorkingDays = GetValue(model.ExpatWorkingDays),
                    ExpatWorkingHours = GetValue(model.ExpatWorkingHours),
                    ExpatOffDaysbyWeek = GetValue(model.ExpatOffDaysbyWeek),
                    ExpatWorkingHoursbyWeek = model.ExpatWorkingHoursbyWeek,
                    CompanyId = (short)model.CompanyId
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

    [HrmPermission(Policy.Host, FunctionKey.TeamManagement)]
    public async Task<IActionResult> TeamSupervisor(int id)
    {
        var supervisors = (await teamService.GetSupervisorByTeamId(id)).ToList();
        ViewBag.TeamId = id;
        ViewBag.TeamCode = supervisors.Count != 0 ? supervisors[0].Code : string.Empty;

        return View("~/Views/Team/TeamSupervisor.cshtml", supervisors);
    }

    private async Task<IEnumerable<Company>> GetAllCompany()
    {
        var companies = await companyService.GetAllCompany();

        return companies;
    }

    private async Task<IEnumerable<EmployeePIC>> GetAllEmployees()
    {
        var employees = await employeeService.GetEmployeeForPICAsync(new GetEmployeeForPICCriteria());

        return employees;
    }
    private static byte GetValue(byte? value) => value ?? 0;
}
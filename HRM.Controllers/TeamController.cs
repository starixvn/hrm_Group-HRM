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
using HRM.Resources;
using HRM.Services.Interfaces;
using HRM.ViewModels.EmployeeGroup;
using HRM.ViewModels.Team;
using Microsoft.AspNetCore.Mvc;

namespace HRM.Controllers;

[HrmPermission(Policy.Read, FunctionKey.TeamManagement)]
public class TeamController(
    IUserControlSession userControlSession,
    ITeamService teamService,
    ISectionService sectionService,
    IBaseService baseService,
    IEmployeeService employeeService) : BaseController(userControlSession)
{
    public async Task<ActionResult> Index(string keyword = null)
    {
        var teams = await teamService.GetAllAsync(keyword: keyword);
        ViewBag.HasPermission = userControlSession.HasPermissionOnFunction(FunctionKey.TeamManagement, Policy.Host);
        ViewBag.HasPermissionToViewEmployee = userControlSession.HasPermissionOnFunction(FunctionKey.EmployeeManagement, Policy.Read);
        return View(teams);
    }

    [HrmPermission(Policy.Host, FunctionKey.TeamManagement)]
    public async Task<ActionResult> Create()
    {
        var model = new TeamViewModel
        {
            Sections = await sectionService.GetSectionAsync(),
            Employees = await GetAllEmployees()
        };
        return View(model);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.TeamManagement)]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Critical Code Smell", "S1541:Methods and properties should not be too complex", Justification = "<Pending>")]
    public async Task<ActionResult> Create(TeamViewModel model)
    {
        model.Sections = await sectionService.GetSectionAsync();
        model.Employees = await GetAllEmployees();
        if (ModelState.IsValid)
        {
            var criteria = new CreateTeamCriteria
            {
                Code = model.TeamCode,
                Name = model.TeamName,
                SectionId = model.SectionId ?? 0,
                HeadId = model.HeadId ?? 0,
                ManagerIds = model.ManagerIds,
                LeaderIds = model.LeaderIds,
                SupervisorIds = model.SupervisorIds,
                MailGroup = model.MailGroup,
                CreatedBy = EmployeeSession.EmpId,
                LocalBasicAnnualLeaveBalance = model.LocalBasicAnnualLeaveBalance ?? 0,
                LocalWorkingDays = model.LocalWorkingDays ?? 0,
                LocalWorkingHours = model.LocalWorkingHours ?? 0,
                LocalOffDaysbyWeek = model.LocalOffDaysbyWeek ?? 0,
                LocalWorkingHoursbyWeek = model.LocalWorkingHoursbyWeek,
                ExpatBasicAnnualLeaveBalance = model.ExpatBasicAnnualLeaveBalance ?? 0,
                ExpatWorkingDays = model.ExpatWorkingDays ?? 0,
                ExpatWorkingHours = model.ExpatWorkingHours ?? 0,
                ExpatOffDaysbyWeek = model.ExpatOffDaysbyWeek ?? 0,
                ExpatWorkingHoursbyWeek = model.ExpatWorkingHoursbyWeek
            };
            var result = await baseService.ExecuteNonQueryAsync(criteria);

            if (result.IsSuccessful)
            {
                return RedirectToAction("Index", "Team");
            }

            ModelState.AddModelError(string.Empty, result.Message);
        }
        else
        {
            ModelState.AddModelError(string.Empty, Messages.InvalidInputData);
        }

        return View(model);
    }

    [HrmPermission(Policy.Host, FunctionKey.TeamManagement)]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Critical Code Smell", "S1541:Methods and properties should not be too complex", Justification = "<Pending>")]
    public async Task<ActionResult> Edit(int id)
    {
        var team = await teamService.GetById(id);
        if (team == null)
        {
            return RedirectToAction("PageNotFound", "Error");
        }

        var unitHead = team.Managers.FirstOrDefault(x => x.RoleId == (byte)TeamManagerRole.Head);
        var model = new TeamViewModel
        {
            TeamCode = team.Code,
            TeamName = team.Name,
            MailGroup = team.MailGroup,
            LocalBasicAnnualLeaveBalance = team.LocalBasicAnnualLeaveBalance ?? 0,
            LocalWorkingDays = team.LocalWorkingDays ?? 0,
            LocalWorkingHours = team.LocalWorkingHours ?? 0,
            LocalOffDaysbyWeek = team.LocalOffDaysbyWeek ?? 0,
            LocalWorkingHoursbyWeek = team.LocalWorkingHoursbyWeek,
            ExpatBasicAnnualLeaveBalance = team.ExpatBasicAnnualLeaveBalance ?? 0,
            ExpatWorkingDays = team.ExpatWorkingDays ?? 0,
            ExpatWorkingHours = team.ExpatWorkingHours ?? 0,
            ExpatOffDaysbyWeek = team.ExpatOffDaysbyWeek ?? 0,
            ExpatWorkingHoursbyWeek = team.ExpatWorkingHoursbyWeek,
            HeadId = unitHead?.EmpId,
            UnitHead = unitHead?.DisplayName,
            ManagerIds = string.Join(",", team.Managers.Where(x => x.RoleId == (byte)TeamManagerRole.Manager).Select(e => e.EmpId)),
            SupervisorIds = string.Join(",", team.Managers.Where(x => x.RoleId == (byte)TeamManagerRole.Supervisor).Select(e => e.EmpId)),
            LeaderIds = string.Join(",", team.Managers.Where(x => x.RoleId == (byte)TeamManagerRole.Leader).Select(e => e.EmpId)),
            SectionId = team.Section != null ? team.Section.Id : 0,
            Sections = await sectionService.GetSectionAsync(),
            Employees = await GetAllEmployees()
        };

        return View(model);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.TeamManagement)]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Critical Code Smell", "S1541:Methods and properties should not be too complex", Justification = "<Pending>")]
    public async Task<ActionResult> Edit(TeamViewModel model)
    {
        if (model != null)
        {
            model.Sections = await sectionService.GetSectionAsync();
            model.Employees = await GetAllEmployees();
            if (ModelState.IsValid)
            {
                var criteria = new EditTeamCriteria
                {
                    TeamId = model.Id,
                    Code = model.TeamCode,
                    Name = model.TeamName,
                    SectionId = model.SectionId ?? 0,
                    MailGroup = model.MailGroup,
                    HeadId = model.HeadId ?? 0,
                    ManagerIds = model.ManagerIds,
                    LeaderIds = model.LeaderIds,
                    SupervisorIds = model.SupervisorIds,
                    ModifiedBy = EmployeeSession.EmpId,
                    LocalBasicAnnualLeaveBalance = model.LocalBasicAnnualLeaveBalance ?? 0,
                    LocalWorkingDays = model.LocalWorkingDays ?? 0,
                    LocalWorkingHours = model.LocalWorkingHours ?? 0,
                    LocalOffDaysbyWeek = model.LocalOffDaysbyWeek ?? 0,
                    LocalWorkingHoursbyWeek = model.LocalWorkingHoursbyWeek,
                    ExpatBasicAnnualLeaveBalance = model.ExpatBasicAnnualLeaveBalance ?? 0,
                    ExpatWorkingDays = model.ExpatWorkingDays ?? 0,
                    ExpatWorkingHours = model.ExpatWorkingHours ?? 0,
                    ExpatOffDaysbyWeek = model.ExpatOffDaysbyWeek ?? 0,
                    ExpatWorkingHoursbyWeek = model.ExpatWorkingHoursbyWeek
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

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.TeamManagement)]
    public async Task<IActionResult> Delete(int id)
    {
        var commandResult = await teamService.Delete(EmployeeSession.EmpId, id);

        return Json(commandResult);
    }

    public async Task<IActionResult> TeamSupervisor(int id)
    {
        var supervisors = (await teamService.GetSupervisorByTeamId(id)).ToList();
        ViewBag.TeamId = id;
        ViewBag.TeamCode = supervisors.Count != 0 ? supervisors.First().Code : string.Empty;

        return View(supervisors);
    }

    [HrmPermission(Policy.Host, FunctionKey.TeamManagement)]
    public IActionResult ManageSupervisor(int id, short roleId)
    {
        ViewBag.TeamId = id;
        ViewBag.RoleId = roleId;

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

    [HttpGet]
    public async Task<IActionResult> GetDisciplineManagerByTeamId(int teamId)
    {
        var supervisors = await teamService.GetDisciplineManagerByTeamId(teamId);
        return Json(supervisors);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.TeamManagement)]
    public async Task<IActionResult> EditTeamSupervisor(int teamId, short roleId, IEnumerable<EditTeamSupervisor> supervisors)
    {
        var commandResult = await teamService.EditTeamSupervisor(EmployeeSession.EmpId, teamId, roleId, supervisors);

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
            IsHost = userControlSession.HasPermissionOnFunction(FunctionKey.TeamManagement, Policy.Host),
            DisciplineRules = await teamService.GetDisciplineRuleByTeam(id)
        };
        return View(model);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.TeamManagement)]
    public async Task<IActionResult> EditDisciplineRule(int teamId, IEnumerable<DisciplineRule> disciplineRules)
    {
        var commandResult = await teamService.EditDisciplineRule(EmployeeSession.EmpId, teamId, disciplineRules);

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
            IsLocalUser = null
        };
        IList<Employee> employees = (await employeeService.GetEmployeeAsync(criteria)).ToList();

        return employees;
    }
}
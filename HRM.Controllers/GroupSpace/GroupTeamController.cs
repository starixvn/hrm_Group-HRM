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
using HRM.Criteria.Group;
using HRM.Models;
using HRM.Models.Group;
using HRM.Models.Supervisor;
using HRM.Resources;
using HRM.Services.Interfaces;
using HRM.ViewModels.EmployeeGroup;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;

namespace HRM.Controllers.GroupSpace;

public class GroupTeamController(
    ITeamService teamService,
    IUserControlSession userControlSession,
    IDepartmentService departmentService,
    IBaseService baseService,
    IEmployeeService employeeService,
    ICompanyService companyService) : BaseController(userControlSession)
{
    [HrmPermission(Policy.Read, FunctionKey.GroupTeamManagement)]
    public IActionResult Index()
    {
        ViewBag.HasPermission = session.HasPermissionOnFunction(FunctionKey.GroupTeamManagement, Policy.Host);
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> GetTeamList(string teamIds = null)
    {
        var teams = await teamService.GetAllTeamByUnitScopeAsync(teamIds);
        var isActived = true;
        var departments = await departmentService.GetDepartmentAsync(isActived);
        var hasPermissionViewAllDepartment = session.HasPermissionOnFunction(FunctionKey.DepartmentManagement_SAView, Policy.Read);
        departments = departmentService.GetDepartmentEmployeeBelongsTo(departments, hasPermissionViewAllDepartment, EmployeeSession.EmpId);
        var departmentIds = departments.Select(d => d.Id).ToList();
        teams = teams.Where(x => departmentIds.Contains(x.DepartmentId))
            .ToList();

        ViewBag.HasPermission = session.HasPermissionOnFunction(FunctionKey.GroupTeamManagement, Policy.Host);
        ViewBag.HasPermissionToViewEmployee = session.HasPermissionOnFunction(FunctionKey.GroupEmployeeList, Policy.Read);

        return PartialView("_TeamList", teams);
    }

    [HrmPermission(Policy.Host, FunctionKey.GroupTeamManagement)]
    public async Task<ActionResult> Create()
    {
        var groupTeamId = await teamService.GetNextGroupTeamIdAsync();
        var companies = await GetAllCompany();
        var employeePics = await GetAllEmployees();
        var model = new GroupTeamViewModel
        {
            GroupTeamId = groupTeamId,
            Departments = await departmentService.GetDepartmentAsync(),
            EmployeePics = employeePics.ToList(),
            Companies = companies.ToList()
        };
        return View(model);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.GroupTeamManagement)]
    public async Task<ActionResult> Create(GroupTeamViewModel model)
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
                CreatedBy = EmployeeSession.EmpId,
                CompanyId = (short)model.CompanyId,
                IsOutsource = model.IsOutsource
            };
            var result = await baseService.ExecuteNonQueryAsync(criteria);

            if (result.IsSuccessful)
            {
                if (model.PICEmpId != null)
                {
                    var insertedTeamId = await teamService.GetGroupIdByGroupTeamId(model.GroupTeamId);
                    await InsertTeamSupervisorByTeamPIC(model.PICEmpId.Value, insertedTeamId);
                }

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

    private async Task InsertTeamSupervisorByTeamPIC(int picEmId, int groupId)
    {
        var supervisorFunctions = new List<EditTeamSupervisor>();

        for (ushort i = 1; i <= 8; i++)
        {
            supervisorFunctions.Add(new EditTeamSupervisor
            {
                OwnerId = picEmId,
                RequestType = i,
                DisplayOrder = 1
            });
        }

        await teamService.InsertTeamSupervisorByPIC(EmployeeSession.EmpId, groupId, supervisorFunctions);
    }

    [HrmPermission(Policy.Host, FunctionKey.GroupTeamManagement)]
    public async Task<ActionResult> Edit(int id)
    {
        var team = await teamService.GetGroupTeamInUnitScopeById(id);
        var companies = await GetAllCompany();
        if (team == null)
        {
            return RedirectToAction("PageNotFound", "Error");
        }

        var employeePics = await GetAllEmployees();
        var model = new GroupTeamViewModel
        {
            Id = team.Id,
            TeamCode = team.TeamCode,
            TeamName = team.TeamName,
            DepartmentId = team.DepartmentId,
            Departments = await departmentService.GetDepartmentAsync(),
            EmployeePics = employeePics.ToList(),
            PICEmpId = team.PICEmpId,
            GroupTeamId = team.GroupTeamId,
            AccountableArea = team.AccountableArea,
            Companies = companies.ToList(),
            CompanyId = team.CompanyId,
            IsOutsource = team.IsOutsource
        };

        return View(model);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.GroupTeamManagement)]
    public async Task<ActionResult> Edit(GroupTeamViewModel model)
    {
        if (model != null)
        {
            var team = await teamService.GetGroupTeamInUnitScopeById(model.Id);
            model.Departments = await departmentService.GetDepartmentAsync();
            model.EmployeePics = (await GetAllEmployees())?.ToList();
            model.Companies = (await GetAllCompany())?.ToList();
            model.CompanyId = team.CompanyId;
            if (ModelState.IsValid)
            {
                var criteria = new EditTeamCriteria
                {
                    TeamId = model.Id,
                    Code = model.TeamCode,
                    Name = model.TeamName,
                    DepartmentId = model.DepartmentId ?? 0,
                    PICEmpId = model.PICEmpId,
                    AccountableArea = model.AccountableArea,
                    ModifiedBy = EmployeeSession.EmpId,
                    GroupTeamId = model.GroupTeamId,
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

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.GroupTeamManagement)]
    public async Task<IActionResult> Delete(int id)
    {
        var commandResult = await teamService.Delete(EmployeeSession.EmpId, id);

        return Json(commandResult);
    }

    [HrmPermission(Policy.Read, FunctionKey.GroupTeamManagement)]
    public async Task<IActionResult> ExportGroupTeamList(string teamIds = null)
    {
        var teams = await teamService.GetAllTeamByUnitScopeAsync(teamIds);
        var isActived = true;
        var departments = await departmentService.GetDepartmentAsync(isActived);
        var hasPermissionViewAllDepartment = session.HasPermissionOnFunction(FunctionKey.DepartmentManagement_SAView, Policy.Read);
        departments = departmentService.GetDepartmentEmployeeBelongsTo(departments, hasPermissionViewAllDepartment, EmployeeSession.EmpId);
        var departmentIds = departments.Select(d => d.Id).ToList();
        teams = teams.Where(x => departmentIds.Contains(x.DepartmentId))
            .ToList();
        var bytes = await teamService.GenerateGroupTeamExcel(teams);
        var fileName = $"GroupTeam_{DateTimeOffset.Now.DateTime.ToClientTimeZone():yyyyMMdd}.xlsx";

        return File(bytes, MimeType.XLS, fileName);
    }

    [HrmPermission(Policy.Host, FunctionKey.GroupTeamManagement)]
    public async Task<IActionResult> TeamSupervisor(int id)
    {
        const bool isGroupSpace = true;
        var supervisors = (await teamService.GetSupervisorByTeamId(id, isGroupSpace)).ToList();
        ViewBag.TeamId = id;
        ViewBag.TeamName = supervisors.Count != 0 ? supervisors[0].Name : string.Empty;

        return View(supervisors);
    }

    [HrmPermission(Policy.Host, FunctionKey.GroupTeamManagement)]
    public IActionResult ShiftManager(int id)
    {
        ViewBag.TeamId = id;
        ViewBag.RoleId = (short)RoleSystem.ShiftManager;

        return View();
    }

    [HrmPermission(Policy.Host, FunctionKey.GroupTeamManagement)]
    public IActionResult DisciplineManager(int id)
    {
        ViewBag.TeamId = id;
        ViewBag.RoleId = (short)RoleSystem.DisciplineManager;
        return View();
    }

    [HrmPermission(Policy.Host, FunctionKey.GroupTeamManagement)]
    public IActionResult ManageSupervisor(int id, short roleId)
    {
        ViewBag.TeamId = id;
        ViewBag.RoleId = roleId;

        return View();
    }

    [HrmPermission(Policy.Host, FunctionKey.GroupTeamManagement)]
    public async Task<IActionResult> GetTeamSupervisorByRoleId(int teamId, short roleId)
    {
        IEnumerable<SupervisorModel> supervisors = await teamService.GetTeamSupervisorByRoleId(teamId, roleId);

        return Json(supervisors);
    }

    [HrmPermission(Policy.Read, FunctionKey.GroupTeamManagement)]
    public async Task<IActionResult> GetDisciplineManagerByTeamId(int teamId)
    {
        var supervisors = await teamService.GetDisciplineManagerByTeamId(teamId, true);
        return Json(supervisors);
    }

    [HrmPermission(Policy.Read, FunctionKey.GroupTeamManagement)]
    public async Task<IActionResult> GetShiftManagerByTeamId(int teamId)
    {
        IEnumerable<SupervisorModel> supervisors = await teamService.GetTeamSupervisorByRoleId(teamId, (short)RoleSystem.ShiftManager, true);
        var result = supervisors.Select(x => new Supervisor
        { OwnerId = x.OwnerId, Nickname = x.NickName, FullName = x.FullName, CanRemove = x.CanRemove });
        return Json(result);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.GroupTeamManagement)]
    public async Task<IActionResult> EditTeamSupervisor(int teamId, short roleId, IEnumerable<EditTeamSupervisor> supervisors)
    {
        var commandResult = await teamService.EditTeamSupervisor(EmployeeSession.EmpId, teamId, roleId, supervisors);

        return Json(commandResult);
    }

    [HrmPermission(Policy.Read, FunctionKey.GroupTeamManagement, FunctionKey.GroupEmployeeList)]
    public async Task<IActionResult> GetEmployees(GroupEmployeeViewModel model)
    {
        var criteria = new GetEmployeesInGroupScopeCriteria
        {
            Query = model.Keyword,
            IsDeleteUser = model.IsDelete,
            EnableUser = model.IsEnable
        };
        var employees = await baseService.GetDataAsync<GroupEmployeeModel>(criteria);

        return Ok(employees);
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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fanex.Data;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Models.Role;
using GO.UserControl.Client.Core.Models.User;
using GO.UserControl.Client.Core.Services;
using GO.UserControl.Client.Core.Session;
using HRM.Core;
using HRM.Core.Constants;
using HRM.Core.Enums;
using HRM.Core.Filters;
using HRM.Core.Services;
using HRM.Criteria.Employee;
using HRM.Criteria.Supervisor;
using HRM.Models;
using HRM.Models.Group;
using HRM.Models.Supervisor;
using HRM.Services.Helper;
using HRM.Services.Implement;
using HRM.Services.Implement.ActionLog;
using HRM.Services.Interfaces;
using HRM.ViewModels;
using HRM.ViewModels.Employee;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace HRM.Controllers;

#pragma warning disable S107 // Methods should not have too many parameters
public class EmployeeController(
    IUserControlSession session,
    IEmployeeService employeeService,
    ICommonDataService commonDataService,
    IHRMAuthService authService,
    IEmployeeLogService employeeLogService,
    ITeamService teamService,
    IJobLevelService jobLevelService,
    IManagementLevelService managementLevelService,
    ICryptoService cryptoService,
    IUserService userService,
    IRoleService roleService,
    IMinIOService minIOService,
    ICompanyService companyService,
    ISalaryStructureService salaryStructureService) : BaseController(session)
#pragma warning restore S107 // Methods should not have too many parameters
{
    [HrmPermission(Policy.Read, FunctionKey.ContactList)]
    public async Task<IActionResult> Index()
    {
        var teams = await teamService.GetAllAsync();
        var model = new UserManagementViewModel
        {
            IsEnable = true,
            CurrentPage = Constants.CurrentPage,
            ItemsPerPage = Constants.ItemsPerPage,
            TeamIds = string.Join(",", teams.Select(x => x.Id))
        };

        return View(model);
    }

    [HttpPost]
    [HrmPermission(Policy.Read, FunctionKey.ContactList)]
    public async Task<IActionResult> ContactList(UserManagementViewModel model)
    {
        var employees = new List<Employee>();
        if (model == null)
        {
            return PartialView("_ContactList", employees);
        }
        var criteria = new GetAllEmployeeCriteria
        {
            Query = model.Keyword,
            TeamIds = model.TeamIds,
            EnableUser = model.IsEnable,
            IsLocalUser = model.IsLocal,
            PageNumber = model.CurrentPage,
            PageSize = model.ItemsPerPage,
            CompanyId = CurrentCompanyId
        };

        employees = (await employeeService.GetEmployeeAsync(criteria)).ToList();

        var pagerOption = new PagerOptionModel
        {
            CurrentPage = model.CurrentPage,
            ItemsPerPage = model.ItemsPerPage,
            TotalItem = criteria.TotalRow ?? 0
        };
        ViewBag.PagerOption = pagerOption;
        ViewBag.IsShowJobtitle = (await roleService.GetByUserIdAsync(session.UserId))
            .Any(r => r.RoleId is (short)RoleSystem.HR or (short)RoleSystem.SystemAdmin);
        ViewBag.EnableGroupDate = await commonDataService.IsEnableGroupDateAsync();

        return PartialView("_ContactList", employees);
    }

    [HrmPermission(Policy.Read, FunctionKey.EmployeeManagement)]
    public async Task<IActionResult> UserManagement(int? jobTitleId = null, string teamIds = null)
    {
        var teams = await teamService.GetAllAsync();
        teamIds ??= string.Join(",", teams.Select(x => x.Id));
        var listJobTitle = await commonDataService.GetLookupDataByCodeAsync(Constants.JobTitle, session.Get<int>(SessionKey.CurrentCompanyId));
        var jobTitles = listJobTitle.Select(j => new SelectListItem
        {
            Text = j.Name,
            Value = j.Id.ToString(),
            Selected = j.Id.ToString() == (jobTitleId != null ? jobTitleId.ToString() : string.Empty)
        }).ToList();

        var model = new UserManagementViewModel
        {
            JobTitleId = jobTitleId,
            TeamIds = teamIds,
            IsEnable = true,
            IsLocal = GetCategory(),
            JobTitles = jobTitles,
            CurrentPage = Constants.CurrentPage,
            ItemsPerPage = Constants.ItemsPerPage,
            IsShowDropdownCategory = session.HasPermissionOnFunction(FunctionKey.ManageAllEmployee, Policy.Read)
        };

        return View(model);
    }

    [HttpPost]
    [HrmPermission(Policy.Read, FunctionKey.EmployeeManagement)]
    public async Task<IActionResult> ListEmployee(UserManagementViewModel model)
    {
        var employees = new List<Employee>();
        if (model != null)
        {
            var criteria = new GetAllEmployeeCriteria
            {
                Query = model.Keyword,
                TeamIds = model.TeamIds,
                EnableUser = model.IsEnable,
                IsDeleteUser = model.IsDelete,
                TitleId = model.JobTitleId,
                IsLocalUser = GetCategory(model.IsLocal),
                PageNumber = model.CurrentPage,
                PageSize = model.ItemsPerPage,
                CompanyId = CurrentCompanyId
            };
            employees = (await employeeService.GetEmployeeAsync(criteria)).ToList();

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

    public async Task<IActionResult> Detail(int id)
    {
        var model = await employeeService.GetEmployeeByIdAsync(id);
        var employeeShift = await employeeService.GetShiftByEmployeeId(id);
        var historyWork = await employeeLogService.GetTransferHistoryWork(id);
        model.HistoryWork = historyWork;

        ViewBag.Sun = employeeShift.SunShiftName;
        ViewBag.Mon = employeeShift.MonShiftName;
        ViewBag.Tue = employeeShift.TueShiftName;
        ViewBag.Wed = employeeShift.WedShiftName;
        ViewBag.Thu = employeeShift.ThuShiftName;
        ViewBag.Fri = employeeShift.FriShiftName;
        ViewBag.Sat = employeeShift.SatShiftName;
        ViewBag.HasPermission = session.HasPermissionOnFunction(FunctionKey.EmployeeManagement, Policy.Host);
        ViewBag.EnableGroupDate = await commonDataService.IsEnableGroupDateAsync();

        return View(model);
    }

    [HrmPermission(Policy.Host, FunctionKey.CreateAccount)]
    public IActionResult Create()
    {
        return View(new CreateEmployeeViewModel());
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.CreateAccount)]
    public async Task<IActionResult> Create(CreateEmployeeViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var commandResult = await employeeService.CheckExistAsync(model.Code, model.Nickname);
        if (commandResult.IsSuccessful)
        {
            return await CreateUserAsync(model);
        }

        ModelState.AddModelError(string.Empty, commandResult.Message);

        return View(model);
    }

    private async Task<IActionResult> CreateUserAsync(CreateEmployeeViewModel model)
    {
        var token = cryptoService.GenerateRijndaelKeyIV();
        var keyIv = token.Split(':');
        var rijndaelKey = keyIv[0];
        var rijndaelIv = keyIv[1];

        var param = new CreateUserParam
        {
            Username = model.Username,
            Code = model.Code,
            Nickname = model.Nickname,
            FullName = model.FullName,
            BCryptPassword = cryptoService.Encrypt(model.Code, rijndaelKey, rijndaelIv),
            Token = token
        };
        var commandResult = await userService.CreateAsync(param);

        if (commandResult.IsSuccessful)
        {
            return await CreateEmployeeAsync(model, commandResult.ObjectId);
        }

        ModelState.AddModelError(string.Empty, commandResult.Message);
        return View(model);
    }

    private async Task<IActionResult> CreateEmployeeAsync(CreateEmployeeViewModel model, int userId)
    {
        var param = new CreateEmployeeParam
        {
            CompanyId = CurrentCompanyId,
            UserId = userId,
            Code = model.Code,
            FullName = model.FullName,
            Nickname = model.Nickname,
            TeamId = model.TeamId,
            ActorId = EmployeeSession.EmpId
        };
        var commandResult = await employeeService.CreateAsync(param);
        if (commandResult.IsSuccessful)
        {
            return RedirectToAction("Edit", new { id = commandResult.OutputId });
        }

        ModelState.AddModelError(string.Empty, commandResult.Message);
        return View(model);
    }

    [HrmPermission(Policy.Host, FunctionKey.EmployeeManagement)]
    public async Task<IActionResult> Edit(int id)
    {
        var employee = await employeeService.GetEmployeeByIdAsync(id);
        if (employee != null)
        {
            if (session.HasPermissionOnFunction(FunctionKey.ManageLocalEmployee, Policy.Host) && !employee.IsLocal
                && session.HasPermissionOnFunction(FunctionKey.ManageExpatEmployee, Policy.Host) && employee.IsLocal)
            {
                return RedirectToAction("AccessDenied", "Error");
            }

            employee.Salary = !string.IsNullOrWhiteSpace(employee.Salary) ? Cryptography.DecryptPassword(employee.Salary) : string.Empty;
            employee.Coutries = await commonDataService.GetAllCountry();
            var supervisors = (await GetSupervisors(id)).ToList();
            employee.Verifier = GetVerifiers(supervisors);
            employee.Approver = GetApproverList(supervisors);
            employee.Disciplinarian = GetDisciplineSupervisors(supervisors);
            employee.EmpShiftList = GetShiftMonitors(supervisors);
            employee.ActingEmpList = GetActingSupervisors(supervisors);
            ViewBag.LengthOfServiceByGroupDate = employeeService.CalculateLengthOfService(employee.JoinedDate, employee.MilitaryFromDate, employee.MilitaryToDate);
            ViewBag.LengthOfServiceByCompanyDate = employeeService.CalculateLengthOfService(employee.CompanyDate, employee.MilitaryFromDate, employee.MilitaryToDate);
            ViewBag.EnableGroupDate = await commonDataService.IsEnableGroupDateAsync();
            ViewBag.Avatar = await GetAvatarUrl(employee.Code);
            await ComposeViewBagsForEditEmployee();
            return View(employee);
        }

        return RedirectToAction("PageNotFound", "Error");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [HrmPermission(Policy.Host, FunctionKey.EmployeeManagement)]
    public async Task<IActionResult> Edit(Employee model)
    {
        if (ModelState.IsValid)
        {
            var roles = BuildRole(model.Roles);
            var param = EmployeeHelper.MappingEditEmployeeParam(model, roles);
            var commandResult = await employeeService.EditAsync(EmployeeSession.EmpId, param, CurrentCompanyId);
            if (commandResult.IsSuccessful)
            {
                await UpdateUserInfo(model.UserId, param);
            }
            else
            {
                ViewBag.Updated = false;
                ModelState.AddModelError(string.Empty, commandResult.Message);
            }
        }

        await PrepareEmployeeModelForView(model);
        return View(model);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.EmployeeManagement)]
    public async Task<ActionResult> Delete(DeleteEmployeeParam param)
    {
        var commandResult = await employeeService.DeleteAsync(EmployeeSession.EmpId, param);
        if (!commandResult.IsSuccessful)
        {
            Json(commandResult);
        }

        var result = await userService.DeleteAsync(param.UserId);
        return Json(result);
    }

    [AllowAnonymous]
    public async Task<IActionResult> Avatar(string id)
    {
        var avatarUrl = await GetAvatarUrl(id);
        return Content(avatarUrl, "text/plain");
    }

    private async Task<string> GetAvatarUrl(string id)
    {
        var avatarName = string.IsNullOrEmpty(id) ? EmployeeSession.Code + ".jpg" : id + ".jpg";
        var avatarPath = $"{CurrentCompanyId}/{avatarName}";
        var avatarUrl = await minIOService.GetFilePath(Bucket.Avatar, avatarPath, "default-avatar.jpg");
        return avatarUrl;
    }

    private async Task PrepareEmployeeModelForView(Employee model)
    {
        model.Coutries = await commonDataService.GetAllCountry();
        var supervisors = (await GetSupervisors(model.Id)).ToList();
        model.Verifier = GetVerifiers(supervisors);
        model.Approver = GetApproverList(supervisors);
        model.Disciplinarian = GetDisciplineSupervisors(supervisors);
        model.EmpShiftList = GetShiftMonitors(supervisors);
        model.ActingEmpList = GetActingSupervisors(supervisors);
        ViewBag.EnableGroupDate = await commonDataService.IsEnableGroupDateAsync();
        ViewBag.Avatar = await GetAvatarUrl(model.Code);

        await ComposeViewBagsForEditEmployee();
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.EmployeeManagement)]
    public async Task<JsonResult> UploadAvatar(string code, IFormFile file)
    {
        if (file is not { Length: > 0 })
        {
            return Json(null);
        }
        var path = $"{CurrentCompanyId}/{code}.jpg";
        var uploadStatus = await minIOService.UploadFileAsync(file, Bucket.Avatar, path);
        if (!uploadStatus)
        {
            return Json(null);
        }

        var avatarUrl = await GetAvatarUrl(code);

        return Json(avatarUrl);
    }

    [HttpGet]
    public IActionResult ManageSupervisor(int id, short roleId)
    {
        ViewBag.EmployeeId = id;
        ViewBag.RoleId = roleId;
        ViewBag.CompanyId = CurrentCompanyId;
        return View();
    }

    public async Task<JsonResult> GetEmployeeSupervisorByRoleId(int employeeId, short roleId)
    {
        var supervisors = await employeeService.GetEmployeeSupervisorByRoleIdAsync(employeeId, roleId);
        return new JsonResult(supervisors);
    }

    [HttpPost]
    public async Task<JsonResult> EditEmployeeSupervisor(int employeeId, short roleId, IEnumerable<EditTeamSupervisor> supervisors)
    {
        var commandResult = await employeeService.EditEmployeeSupervisorAsync(EmployeeSession.EmpId, employeeId, roleId, supervisors);

        return new JsonResult(commandResult);
    }

    [HttpGet]
    public IActionResult ShiftManager(int id)
    {
        ViewBag.EmployeeId = id;
        ViewBag.RoleId = (short)RoleSystem.ShiftManager;
        ViewBag.CompanyId = CurrentCompanyId;

        return View();
    }

    public async Task<JsonResult> GetShiftManagerByEmployeeId(int employeeId)
    {
        var supervisors = await employeeService.GetEmployeeSupervisorByRoleIdAsync(employeeId, (short)RoleSystem.ShiftManager);
        return new JsonResult(supervisors.Select(x => new Supervisor
        { OwnerId = x.OwnerId, Nickname = x.NickName, FullName = x.FullName, CompanyName = x.CompanyName, CompanyId = x.CompanyId, CanRemove = x.CanRemove}));
    }

    [HttpPost]
    public IActionResult ToggleLeftNavigation(bool isShow)
    {
        session.Set(SessionKey.IsShowLeftNavigation, isShow);
        return Ok();
    }

    [HrmPermission(Policy.Read, FunctionKey.EmployeeManagement)]
    public IActionResult DisciplineManager(int id)
    {
        ViewBag.EmployeeId = id;
        ViewBag.RoleId = (short)RoleSystem.DisciplineManager;
        ViewBag.CompanyId = CurrentCompanyId;
        return View();
    }

    public async Task<JsonResult> GetDisciplineManagerByEmployeeId(int employeeId)
    {
        var supervisors = await employeeService.GetDisciplineSettingByEmployeeIdAsync(employeeId);
        return new JsonResult(supervisors);
    }

    public async Task<JsonResult> GetMemberByManager(string keyword, short? companyId = null)
    {
        var criteria = new GetMemberByManagerCriteria
        {
            SupervisorId = EmployeeSession.EmpId,
            Query = keyword,
            ExcludedManager = false,
            CompanyId = companyId ?? CurrentCompanyId
        };
        var employees = await employeeService.GetMemberByManagerAsync(criteria);
        const byte totalItems = 10;

        return Json(employees.Take(totalItems));
    }

    public async Task<JsonResult> GetEmployees(
        string query,
        string teamIds = "",
        bool? enable = null,
        bool? isDelete = null,
        int? titleId = null,
        bool? isLocal = null)
    {
        var criteria = new GetAllEmployeeCriteria
        {
            Query = query,
            TeamIds = teamIds,
            EnableUser = enable,
            IsDeleteUser = isDelete,
            TitleId = titleId,
            IsLocalUser = isLocal,
            CompanyId = CurrentCompanyId
        };
        var employees = await employeeService.GetEmployeeAsync(criteria);

        return Json(employees);
    }

    public async Task<JsonResult> GetSupervisors(string roleIds, string query, string excludeIds)
    {
        var criteria = new GetEmployeeByRoleCriteria
        {
            CompanyId = CurrentCompanyId,
            ExcludeIds = excludeIds ?? string.Empty,
            Query = query.Trim(),
            RoleIds = roleIds
        };
        var employees = await employeeService.GetEmployeesByRoleAsync(criteria);

        return Json(employees);
    }

    public async Task<JsonResult> GetMemberByGender(string keyword, int gender)
    {
        var employees = await employeeService.GetEmployeeByGender(keyword, gender, CurrentCompanyId);
        return new JsonResult(employees, new JsonSerializerSettings { ContractResolver = new DefaultContractResolver() });
    }

    public async Task<JsonResult> GenerateEmployeeCode(short? companyId)
    {
        var newCode = await employeeService.GenerateEmployeeCode(companyId);
        return Json(newCode);
    }

    [HrmPermission(Policy.Host, FunctionKey.EmployeeManagement)]
    public async Task<IActionResult> ActivityLog(int id)
    {
        var employee = await employeeService.GetEmployeeByIdAsync(id);
        var logs = await employeeLogService.GetLogDetailByIdAsync(id);
        ViewBag.Employee = $"{employee.NickName} ({employee.FullName})";
        ViewBag.EnableGroupDate = await commonDataService.IsEnableGroupDateAsync();

        return View("ActivityLog", logs);
    }

    private async Task UpdateUserInfo(int userId, EditEmployeeParam param)
    {
        var result = await employeeService.UpdateUserInfoAsync(userId, param);
        if (result.IsSuccessful)
        {
            await userService.AssignRoleAsync(new AssignRoleParam
            {
                SiteId = session.SiteId,
                UserId = userId,
                RoleIds = param.Roles
            });

            ViewBag.Updated = true;
            await UpdateUserSession(param.EmployeeId);
        }
        else
        {
            ViewBag.Updated = false;
            ModelState.AddModelError(string.Empty, result.Message);
        }
    }

    private async Task UpdateUserSession(int employeeId)
    {
        if (EmployeeSession.EmpId == employeeId)
        {
            session.Set(SessionKey.Employee, await authService.GetByUserIdAsync(session.UserId));
        }
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

    private string GetSupervisors(IEnumerable<SupervisorNameModel> supervisors, short roleId)
    {
        const byte maxDisplayItems = 3;

        if (supervisors == null || !supervisors.Any())
        {
            return string.Empty;
        }

        var filteredSupervisors = supervisors
            .Where(x => x.ActionType == roleId)
            .Select(x => $"{x.NickName} {(string.IsNullOrEmpty(x.CompanyName) || x.CompanyId == CurrentCompanyId ? "" : $"({x.CompanyName})")}")
            .Take(maxDisplayItems)
            .ToList();

        var supervisorNames = string.Join(", ", filteredSupervisors);

        if (supervisors.Count(x => x.ActionType == roleId) > maxDisplayItems)
        {
            supervisorNames += ",...";
        }

        return supervisorNames;
    }

    private string GetVerifiers(IEnumerable<SupervisorNameModel> supervisors)
    {
        return GetSupervisors(supervisors, (short)RoleSystem.Verifier);
    }

    private string GetApproverList(IEnumerable<SupervisorNameModel> supervisors)
    {
        return GetSupervisors(supervisors, (short)RoleSystem.Approver);
    }

    private string GetShiftMonitors(IEnumerable<SupervisorNameModel> supervisors)
    {
        return GetSupervisors(supervisors, (short)RoleSystem.ShiftManager);
    }

    private string GetActingSupervisors(IEnumerable<SupervisorNameModel> supervisors)
    {
        return GetSupervisors(supervisors, (short)RoleSystem.OnBehalfRequest);
    }

    private string GetDisciplineSupervisors(IEnumerable<SupervisorNameModel> supervisors)
    {
        return GetSupervisors(supervisors, (short)RoleSystem.DisciplineManager);
    }

    private async Task ComposeViewBagsForEditEmployee()
    {
        var jobLevels = await jobLevelService.GetJobLevel();
        var managementLevels = await managementLevelService.GetManagementLevel();
        var roles = await roleService.GetBySiteIdAsync(session.SiteId);
        var userRoles = await roleService.GetByUserIdAsync(session.UserId);
        var subCompanies = await companyService.GetSubCompanies(CurrentCompanyId);
        if (userRoles.All(r => r.RoleId != (short)RoleSystem.SystemAdmin))
        {
            var excludedRoles = new[] { (short)RoleSystem.SystemAdmin, (short)RoleSystem.PayrollAdmin };
            roles = roles
                .Where(r => !excludedRoles.Contains(r.RoleId))
                .ToList();
        }

        ViewBag.IsShowSubCompanyConfig = subCompanies != null && subCompanies.Any();
        ViewBag.HasReadPermissionOnSalaryStructure = session.HasPermissionOnFunction(FunctionKey.SalaryStructureConfiguration, Policy.Read);
        ViewBag.HasHostPermissionOnSalaryStructure = session.HasPermissionOnFunction(FunctionKey.SalaryStructureConfiguration, Policy.Host);

        ViewBag.JobTitles = await commonDataService.GetLookupDataByCodeAsync(Constants.JobTitle, session.Get<int>(SessionKey.CurrentCompanyId));
        ViewBag.JobLevel = jobLevels;
        ViewBag.Grades = managementLevels;
        ViewBag.Marriages = Utilities.Marriages;
        ViewBag.Roles = roles;
        ViewBag.SubCompanies = subCompanies;
        ViewBag.SalaryStructures = await salaryStructureService.GetSalaryStructureAsync(CurrentCompanyId);
    }

    private string BuildRole(IList<Role> roles)
    {
        var systemRoles = roleService.GetBySiteId(session.SiteId);
        var defaultRoles = systemRoles.Where(x => x.IsDefault).Select(x => x.RoleId);

        if (roles is { Count: > 0 })
        {
            var result = string.Join(",", roles.Select(i => i.RoleId));
            return roles.Any(x => defaultRoles.Any(y => x.RoleId == y)) ? result : string.Join(",", defaultRoles) + "," + result;
        }

        return string.Join(",", defaultRoles);
    }

    private bool? GetCategory(bool? isLocal = null)
    {
        var category = isLocal;
        if (session.HasPermissionOnFunction(FunctionKey.ManageAllEmployee, Policy.Read))
        {
            return category;
        }

        if (session.HasPermissionOnFunction(FunctionKey.ManageLocalEmployee, Policy.Read))
        {
            category = true;
        }
        else
        {
            if (session.HasPermissionOnFunction(FunctionKey.ManageExpatEmployee, Policy.Read))
            {
                category = false;
            }
        }

        return category;
    }
}
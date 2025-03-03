namespace HRM.Controllers;

using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Models.Role;
using GO.UserControl.Client.Core.Models.User;
using GO.UserControl.Client.Core.Services;
using GO.UserControl.Client.Core.Session;
using Core;
using Core.Constants;
using Core.Enums;
using Core.Filters;
using Criteria.Employee;
using Criteria.Supervisor;
using Models;
using HRM.Models.Group;
using Models.Supervisor;
using Services;
using Services.Implement;
using Services.Interfaces;
using ViewModels;
using ViewModels.Employee;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

public class EmployeeController(
    IUserControlSession userControlSession,
    IEmployeeService employeeService,
    ICryptoService cryptoService,
    IUserService userService,
    ICommonDataService commonDataService,
    IJobLevelService jobLevelService,
    IRoleService roleService,
    IWebHostEnvironment env,
    IHRMAuthService hrmService) : BaseController(userControlSession)
{

    [HrmPermission(Policy.Read, FunctionKey.ContactList)]
    public async Task<IActionResult> Index()
    {
        var teams = await commonDataService.GetAllTeamAsync();
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
        IEnumerable<Employee> employees = new List<Employee>();
        if (model == null) return PartialView("_ContactList", employees);

        var criteria = new GetAllEmployeeCriteria
        {
            Query = model.Keyword,
            TeamIds = model.TeamIds,
            EnableUser = model.IsEnable,
            IsLocalUser = model.IsLocal,
            PageNumber = model.CurrentPage,
            PageSize = model.ItemsPerPage
        };

        employees = await employeeService.GetEmployeeAsync(criteria);

        var pagerOption = new PagerOptionModel
        {
            CurrentPage = model.CurrentPage,
            ItemsPerPage = model.ItemsPerPage,
            TotalItem = (int)criteria.TotalRow
        };
        ViewBag.PagerOption = pagerOption;

        var roles = await roleService.GetByUserIdAsync(userControlSession.UserId);
        ViewBag.IsShowJobtitle = roles.Any(r => r.RoleId is (short)RoleSystem.HR or (short)RoleSystem.SystemAdmin);
            ViewBag.EnableGroupDate = await commonDataService.IsEnableGroupDateAsync();

        return PartialView("_ContactList", employees);
    }

    [HrmPermission(Policy.Host, FunctionKey.CreateAccount)]
    public ActionResult Create()
    {
        return View(new CreateEmployeeViewModel());
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.CreateAccount)]
    public async Task<ActionResult> Create(CreateEmployeeViewModel model)
    {
        var param = new CreateEmployeeParam()
        {
            Code = model.Code,
            DomainUsername = model.DomainUsername,
            FullName = model.FullName,
            Nickname = model.Nickname,
            TeamId = model.TeamId
        };

        var commandResult = await employeeService.CreateAsync(EmployeeSession.EmpId, param);
        if (!commandResult.IsSuccessful)
        {
            ModelState.AddModelError(string.Empty, commandResult.Message);
            return View(model);
        }

        var token = cryptoService.GenerateRijndaelKeyIV();
        var keyIv = token.Split(':');
        var rijndaelKey = keyIv[0];
        var rijndaelIv = keyIv[1];
        var userModel = new CreateUserParam
        {
            RefId = (int?)commandResult.OutputId,
            Code = param.Code,
            Nickname = param.Nickname,
            FullName = param.FullName,
            BCryptPassword = cryptoService.Encrypt(param.Code, rijndaelKey, rijndaelIv),
            Token = token
        };
        var result = await userService.CreateAsync(userModel);

        if (!result.IsSuccessful)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        var updatedUserModel = new UpdateUserIdCommand
        {
            EmpId = (int)commandResult.OutputId,
            UserId = result.ObjectId,
            ModifiedBy = EmployeeSession.EmpId
        };

        await employeeService.UpdateUserIdAsync(updatedUserModel);
        return RedirectToAction("Edit", new { id = commandResult.OutputId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [HrmPermission(Policy.Host, FunctionKey.EmployeeManagement)]
    public async Task<IActionResult> Edit(Employee model)
    {
        if (ModelState.IsValid)
        {
            var param = await MappingEditEmployeeParam(model);
            var commandResult = await employeeService.EditAsync(EmployeeSession.EmpId, param);
            if (commandResult.IsSuccessful)
            {
                await UpdateUserInfoAsync(model.UserId, param);
            }
            else
            {
                ViewBag.Updated = false;
                ModelState.AddModelError(string.Empty, commandResult.Message);
            }
        }

        model.Coutries = await commonDataService.GetAllCountryAsync();
        var supervisors = await GetSupervisorsAsync(model.Id);
        model.Verifier = GetVerifiers(supervisors);
        model.Approver = GetApprovers(supervisors);
        model.Disciplinarian = GetDisciplineSupervisors(supervisors);
        model.EmpShiftList = GetShiftMonitors(supervisors);
        model.ActingEmpList = GetActingSupervisors(supervisors);
        ViewBag.EnableGroupDate = await commonDataService.IsEnableGroupDateAsync();

        await ComposeViewBagsForEditEmployee();

        return View(model);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.EmployeeManagement)]
    public async Task<ActionResult> Delete(DeleteEmployeeParam param)
    {
        var commandResult = await employeeService.DeleteAsync(EmployeeSession.EmpId, param);
        if (!commandResult.IsSuccessful) return Json(commandResult);
        var result = await userService.DeleteAsync(param.UserId);
        return Json(result);
    }

    public JsonResult GenerateEmployeeCode()
    {
        var newCode = employeeService.GenerateEmployeeCode();
        return Json(newCode);
    }

    [HttpPost]
    public async Task<CustomJsonResult> EditEmployeeSupervisor(int employeeId, short roleId, IEnumerable<EditTeamSupervisor> supervisors)
    {
        var commandResult = await employeeService.EditEmployeeSupervisorAsync(EmployeeSession.EmpId, employeeId, roleId, supervisors);

        return new CustomJsonResult(commandResult);
    }

    [HrmPermission(Policy.Host, FunctionKey.EmployeeManagement)]
    public async Task<IActionResult> Edit(int id)
    {
        var employee = await employeeService.GetEmployeeByIdAsync(id);
        if (employee == null) return RedirectToAction("PageNotFound", "Error");

        if (userControlSession.HasPermissionOnFunction(FunctionKey.ManageLocalEmployee, Policy.Host)
            && !employee.IsLocal
            && userControlSession.HasPermissionOnFunction(FunctionKey.ManageExpatEmployee, Policy.Host)
            && employee.IsLocal)
        {
            return RedirectToAction("AccessDenied", "Error");
        }

        employee.Coutries = await commonDataService.GetAllCountryAsync();
        var supervisors = await GetSupervisorsAsync(id);
        employee.Verifier = GetVerifiers(supervisors);
        employee.Approver = GetApprovers(supervisors);
        employee.Disciplinarian = GetDisciplineSupervisors(supervisors);
        employee.EmpShiftList = GetShiftMonitors(supervisors);
        employee.ActingEmpList = GetActingSupervisors(supervisors);
        ViewBag.LengthOfServiceByGroupDate = employeeService.CaculateLengthOfService(employee.JoinedDate, employee.MilitaryFromDate, employee.MilitaryToDate);
        ViewBag.LengthOfServiceByCompanyDate = employeeService.CaculateLengthOfService(employee.CompanyDate, employee.MilitaryFromDate, employee.MilitaryToDate);
        ViewBag.EnableGroupDate = await commonDataService.IsEnableGroupDateAsync();

        await ComposeViewBagsForEditEmployee();
        return View(employee);
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

    public async Task<CustomJsonResult> GetEmployeeSupervisorByRoleId(int employeeId, short roleId)
    {
        IEnumerable<SupervisorModel> supervisors = await employeeService.GetEmployeeSupervisorByRoleIdAsync(employeeId, roleId);
        return new CustomJsonResult(supervisors);
    }

    [AllowAnonymous]
    public FileResult Avatar(string id)
    {
        var avatarName = string.IsNullOrEmpty(id) ? EmployeeSession.Code + ".jpg" : id + ".jpg";
        var filePath = Path.Combine(env.WebRootPath, "avatars", avatarName);

        if (System.IO.File.Exists(filePath)) return PhysicalFile(filePath, "image/jpeg");

        var defaultImgPath = Path.Combine(env.ContentRootPath, "avatars/avatar.jpg");
        return PhysicalFile(defaultImgPath, "image/jpeg");
    }

    [HttpGet]
    public ActionResult ManageSupervisor(int id, short roleId)
    {
        ViewBag.EmployeeId = id;
        ViewBag.RoleId = roleId;
        return View();
    }

    public async Task<IActionResult> GetEmployees(
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
            IsLocalUser = isLocal
        };
        var employees = await employeeService.GetEmployeeAsync(criteria);

        return Json(employees);
    }

    [HrmPermission(Policy.Read, FunctionKey.EmployeeManagement)]
    public ActionResult DisciplineManager(int id)
    {
        ViewBag.EmployeeId = id;
        ViewBag.RoleId = (short)RoleSystem.DisciplineManager;
        return View();
    }

    public async Task<CustomJsonResult> GetDisciplineManagerByEmployeeId(int employeeId)
    {
        var supervisors = await employeeService.GetDisciplineSettingByEmployeeIdAsync(employeeId);
        return new CustomJsonResult(supervisors);
    }

    [HttpGet]
    public ActionResult ShiftManager(int id)
    {
        ViewBag.EmployeeId = id;
        ViewBag.RoleId = (short)RoleSystem.ShiftManager;

        return View();
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.EmployeeManagement)]
    public JsonResult UploadAvatar(string code, IFormFile file)
    {
        if (file == null || file.Length <= 0) return Json(null);
        using var memoryStream = new MemoryStream();
        file.CopyTo(memoryStream);

        memoryStream.Position = 0;

        using var image = Image.FromStream(memoryStream);
        var avatarsFolderPath = Path.Combine(env.WebRootPath, "avatars");
        if (!Directory.Exists(avatarsFolderPath))
        {
            Directory.CreateDirectory(avatarsFolderPath);
        }

        var filePath = Path.Combine(avatarsFolderPath, $"{code}.jpg");

        image.Save(filePath);

        return Json(true);
    }

    public async Task<CustomJsonResult> GetShiftManagerByEmployeeId(int employeeId)
    {
        IEnumerable<SupervisorModel> supervisors = await employeeService
            .GetEmployeeSupervisorByRoleIdAsync(employeeId, (short)RoleSystem.ShiftManager);

        var data = supervisors
            .Select(x => new Supervisor
            {
                OwnerId = x.OwnerId,
                Nickname = x.NickName,
                FullName = x.FullName
            });
        return new CustomJsonResult(data);
    }

    public async Task<IActionResult> GetMemberByManager(string keyword)
    {
        var criteria = new GetMemberByManagerCriteria
        {
            SupervisorId = EmployeeSession.EmpId,
            Query = keyword,
            ExcludedManager = false
        };
        var employees = await employeeService.GetMemberByManagerAsync(criteria);
        const byte totalItems = 10;

        return Json(employees.Take(totalItems));
    }

    private async Task<IList<SupervisorNameModel>> GetSupervisorsAsync(int empId)
    {
        var criteria = new GetListSupervisorNameCriteria
        {
            EmpId = empId,
            Role = null,
            RequestType = null
        };
        var supervisors = await employeeService.GetListSupervisorNameAsync(criteria);
        return supervisors;
    }

    private async Task<EditEmployeeParam> MappingEditEmployeeParam(Employee model)
    {
        return new EditEmployeeParam
        {
            EmployeeId = model.Id,
            Code = model.Code.Trim(),
            DomainUsername = model.DomainUsername,
            FullName = model.FullName,
            Nickname = model.NickName,
            EmployeeCard = model.EmployeeCard,
            DateOfBirth = model.DateOfBirth,
            Email = model.Email,
            JobTitleId = model.JobTitleId,
            JobLevelId = model.JobLevelId,
            ManagementLevelId = model.ManagementLevelId,
            Enabled = model.Enabled,
            Phone = model.Phone,
            Gender = model.Gender,
            Roles = await BuildRole(model.Roles),
            JoinedDate = model.JoinedDate,
            CompanyDate = model.CompanyDate,
            ConfirmedDate = model.ConfirmedDate,
            ResignedDate = model.ResignedDate,
            ResignedReason = model.ResignedReason,
            Skype = model.Skype,
            LetsTalk = model.LetsTalk,
            Marriage = model.Marriage,
            PermanentAddress = model.Address,
            CurrentAddress = model.CurrentAddress,
            IdCardNumber = model.CardNo,
            PassportNo = model.PassportNo,
            PassportExp = model.PassportExp,
            TeamId = model.Team.Id,
            IsLocal = model.IsLocal,
            CountryCode = model.CountryCode,
            CanOT = model.CanOT,
            SpecialOT = model.SpecialOT,
            WorkingDays = model.WorkingDays,
            WorkingHours = model.WorkingHours,
            WorkingHoursbyWeek = model.WorkingHoursbyWeek,
            IsTopManager = model.IsTopManager,
            OffDaysbyWeek = model.OffDaysbyWeek,
            PlaceOfBirth = model.PlaceOfBirth,
            Ethnic = model.Ethnic,
            Native = model.Native,
            Religion = model.Religion,
            DateOfIdCard = model.DateOfIdCard,
            PlaceOfIdCard = model.PlaceOfIdCard,
            HomePhone = model.HomePhone,
            Qualification = model.Qualification,
            Major = model.Major,
            IsExpatUseHoliday = !model.IsLocal && model.IsExpatUseHoliday,
            PickupRemark = model.PickupRemark,
            PromotionDate = model.PromotionDate,
            TeamDate = model.TeamDate,
            MilitaryFromDate = model.MilitaryFromDate,
            MilitaryToDate = model.MilitaryToDate
        };
    }

    private async Task UpdateUserInfoAsync(int userId, EditEmployeeParam param)
    {
        var result = await userService.UpdateInfoAsync(new UpdateUserInfoParam
        {
            RefId = param.EmployeeId,
            UserId = userId,
            Code = param.Code,
            Nickname = param.Nickname,
            FullName = param.FullName,
            Email = param.Email,
            IsActive = param.Enabled
        });
        if (result.IsSuccessful)
        {
            result = await userService.AssignRoleAsync(new AssignRoleParam
            {
                UserId = userId,
                RoleIds = param.Roles
            });

            if (result.IsSuccessful)
            {
                ViewBag.Updated = true;
                await UpdateUserSessionAsync(param.EmployeeId);
            }
            else
            {
                ViewBag.Updated = false;
                ModelState.AddModelError(string.Empty, result.Message);
            }
        }
        else
        {
            ViewBag.Updated = false;
            ModelState.AddModelError(string.Empty, result.Message);
        }
    }

    private async Task UpdateUserSessionAsync(int employeeId)
    {
        if (EmployeeSession.EmpId == employeeId)
        {
            var user = await hrmService.GetByUserIdAsync(userControlSession.UserId);
            userControlSession.Set(SessionKey.Employee, user);
        }
    }

    private async Task<string> BuildRole(IList<Role> roles)
    {
        var systemRoles = await roleService.GetBySiteIdAsync(userControlSession.SiteId);
        var defaultRoles = systemRoles.Where(x => x.IsDefault).Select(x => x.RoleId);

        if (roles != null && roles.Count > 0)
        {
            var result = string.Join(",", roles.Select(i => i.RoleId));
            return roles.Any(x => defaultRoles.Any(y => x.RoleId == y)) ? result : string.Join(",", defaultRoles) + "," + result;
        }
        else
        {
            return string.Join(",", defaultRoles);
        }
    }

    private static string GetVerifiers(IList<SupervisorNameModel> supervisors)
    {
        return GetSupervisors(supervisors, (short)RoleSystem.Verifier);
    }

    private static string GetSupervisors(IList<SupervisorNameModel> supervisors, short roleId)
    {
        var supervisorsName = string.Empty;
        const byte maxDisplayItems = 3;

        if (supervisors == null) return supervisorsName;

        var supervisorsFilter = supervisors
            .Where(x => x.ActionType == roleId)
            .Take(maxDisplayItems)
            .ToList();

        supervisorsName = string
            .Join(", ", supervisorsFilter
            .Select(x => x.NickName));

        if (supervisorsFilter.Count > maxDisplayItems) supervisorsName += ",...";

        return supervisorsName;
    }

    private static string GetApprovers(IList<SupervisorNameModel> supervisors)
    {
        return GetSupervisors(supervisors, (short)RoleSystem.Approver);
    }

    private static string GetDisciplineSupervisors(IList<SupervisorNameModel> supervisors)
    {
        return GetSupervisors(supervisors, (short)RoleSystem.DisciplineManager);
    }

    private static string GetShiftMonitors(IList<SupervisorNameModel> supervisors)
    {
        return GetSupervisors(supervisors, (short)RoleSystem.ShiftManager);
    }

    private static string GetActingSupervisors(IList<SupervisorNameModel> supervisors)
    {
        return GetSupervisors(supervisors, (short)RoleSystem.OnBehalfRequest);
    }

    private async Task ComposeViewBagsForEditEmployee()
    {
        var jobLevels = await jobLevelService.GetJobLevel();
        ViewBag.JobTitles = await commonDataService.GetLookupDataByCodeAsync(Constants.JobTitle);
        ViewBag.JobLevel = jobLevels;
        ViewBag.Grades = jobLevels?.Where(jl => jl.Name.StartsWith("E")).ToList();
        ViewBag.Marriages = Utilities.Marriages;
        ViewBag.Roles = await roleService.GetBySiteIdAsync(userControlSession.SiteId);
    }
}
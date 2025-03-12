using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Models.Role;
using GO.UserControl.Client.Core.Models.User;
using GO.UserControl.Client.Core.Services;
using GO.UserControl.Client.Core.Session;
using HRM.Core;
using HRM.Core.Constants;
using HRM.Core.Enums;
using HRM.Core.Filters;
using HRM.Core.Models;
using HRM.Criteria.Employee;
using HRM.Criteria.Supervisor;
using HRM.Models;
using HRM.Models.Group;
using HRM.Models.Supervisor;
using HRM.Services.Implement;
using HRM.Services.Implement.ActionLog;
using HRM.Services.Interfaces;
using HRM.ViewModels;
using HRM.ViewModels.Employee;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SixLabors.ImageSharp;

namespace HRM.Controllers;

public class EmployeeController(
    IUserControlSession userControlSession,
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
    IWebHostEnvironment hostingEnvironment) : BaseController(userControlSession)
{
    private readonly IUserControlSession userControlSession = userControlSession;

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
        ViewBag.IsShowJobtitle = (await roleService.GetByUserIdAsync(userControlSession.UserId))
            .Any(r => r.RoleId is (short)RoleSystem.HR or (short)RoleSystem.SystemAdmin);
        ViewBag.EnableGroupDate = await commonDataService.IsEnableGroupDateAsync();

        return PartialView("_ContactList", employees);
    }

    [HrmPermission(Policy.Read, FunctionKey.EmployeeManagement)]
    public async Task<IActionResult> UserManagement(int? jobTitleId = null, string teamIds = null)
    {
        var teams = await teamService.GetAllAsync();
        teamIds ??= string.Join(",", teams.Select(x => x.Id));
        var listJobTitle = await commonDataService.GetLookupDataByCodeAsync(Constants.JobTitle, userControlSession.Get<int>(SessionKey.CurrentCompanyId));
        IList<SelectListItem> jobTitles = listJobTitle.Select(j => new SelectListItem
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
            IsShowDropdownCategory = userControlSession.HasPermissionOnFunction(FunctionKey.ManageAllEmployee, Policy.Read)
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

        ViewBag.HasPermissionToEdit = userControlSession.HasPermissionOnFunction(FunctionKey.EmployeeManagement, Policy.Host);
        ViewBag.EnableGroupDate = await commonDataService.IsEnableGroupDateAsync();

        return PartialView("_ListEmployee", employees);
    }

    public async Task<IActionResult> Detail(int id)
    {
        var model = await employeeService.GetEmployeeByIdAsync(id);
        var employeeShift = await employeeService.GetShiftByEmployeeId(id);

        ViewBag.Sun = employeeShift.SunShiftName;
        ViewBag.Mon = employeeShift.MonShiftName;
        ViewBag.Tue = employeeShift.TueShiftName;
        ViewBag.Wed = employeeShift.WedShiftName;
        ViewBag.Thu = employeeShift.ThuShiftName;
        ViewBag.Fri = employeeShift.FriShiftName;
        ViewBag.Sat = employeeShift.SatShiftName;
        ViewBag.HasPermission = userControlSession.HasPermissionOnFunction(FunctionKey.EmployeeManagement, Policy.Host);
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

        var param = new CreateEmployeeParam
        {
            Code = model.Code,
            Nickname = model.Nickname
        };

        var commandResult = await employeeService.CheckExistAsync(param);
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
            CompanyId = userControlSession.Get<short>(SessionKey.CurrentCompanyId),
            UserId = userId,
            Code = model.Code,
            FullName = model.FullName,
            Nickname = model.Nickname,
            TeamId = model.TeamId,
            DomainUsername = model.DomainUsername,
            ActorId = EmployeeSession.EmpId
        };
        var commandResult = await employeeService.CreateAsync(param);
        if (commandResult.IsSuccessful)
        {
            return RedirectToAction("Edit", new { id = userId });
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
            if ((userControlSession.HasPermissionOnFunction(FunctionKey.ManageLocalEmployee, Policy.Host) && !employee.IsLocal)
                && (userControlSession.HasPermissionOnFunction(FunctionKey.ManageExpatEmployee, Policy.Host) && employee.IsLocal))
            {
                return RedirectToAction("AccessDenied", "Error");
            }

            employee.Coutries = await commonDataService.GetAllCountry();
            var supervisors = (await GetSupervisors(id)).ToList();
            employee.Verifier = GetVerifiers(supervisors);
            employee.Approver = GetApproverList(supervisors);
            employee.Disciplinarian = GetDisciplineSupervisors(supervisors);
            employee.EmpShiftList = GetShiftMonitors(supervisors);
            employee.ActingEmpList = GetActingSupervisors(supervisors);
            ViewBag.LengthOfServiceByGroupDate = employeeService.CaculateLengthOfService(employee.JoinedDate, employee.MilitaryFromDate, employee.MilitaryToDate);
            ViewBag.LengthOfServiceByCompanyDate = employeeService.CaculateLengthOfService(employee.CompanyDate, employee.MilitaryFromDate, employee.MilitaryToDate);
            ViewBag.EnableGroupDate = await commonDataService.IsEnableGroupDateAsync();

            await ComposeViewBagsForEditEmployee();
            return View(employee);
        }
        else
        {
            return RedirectToAction("PageNotFound", "Error");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [HrmPermission(Policy.Host, FunctionKey.EmployeeManagement)]
    public async Task<IActionResult> Edit(Employee model)
    {
        if (ModelState.IsValid)
        {
            var param = MappingEditEmployeeParam(model);
            var commandResult = await employeeService.EditAsync(EmployeeSession.EmpId, param);
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

        model.Coutries = await commonDataService.GetAllCountry();
        var supervisors = (await GetSupervisors(model.Id)).ToList();
        model.Verifier = GetVerifiers(supervisors);
        model.Approver = GetApproverList(supervisors);
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
        if (!commandResult.IsSuccessful)
        {
            Json(commandResult);
        }

        var result = await userService.DeleteAsync(param.UserId);
        return Json(result);
    }

    [AllowAnonymous]
    public FileResult Avatar(string id)
    {
        var avatarName = string.IsNullOrEmpty(id) ? EmployeeSession.Code + ".jpg" : id + ".jpg";
        var avatarPath = Path.Combine(hostingEnvironment.ContentRootPath, "Avatars", avatarName);
        var defaultAvatarPath = Path.Combine(hostingEnvironment.ContentRootPath, "Avatars", "avatar.jpg");

        return System.IO.File.Exists(avatarPath)
            ? PhysicalFile(avatarPath, "image/jpeg")
            : PhysicalFile(defaultAvatarPath, "image/jpeg");
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.EmployeeManagement)]
    public async Task<JsonResult> UploadAvatar(string code, IFormFile file)
    {
        if (file is not { Length: > 0 })
        {
            return Json(null);
        }

        var avatarPath = Path.Combine(hostingEnvironment.ContentRootPath, "avatars", $"{code}.jpg");
        using var stream = new MemoryStream();
        await file.CopyToAsync(stream);
        stream.Seek(0, SeekOrigin.Begin);

        using var image = await Image.LoadAsync(stream);
        await image.SaveAsJpegAsync(avatarPath);

        return Json(null);
    }

    [HttpGet]
    public IActionResult ManageSupervisor(int id, short roleId)
    {
        ViewBag.EmployeeId = id;
        ViewBag.RoleId = roleId;
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

        return View();
    }

    public async Task<JsonResult> GetShiftManagerByEmployeeId(int employeeId)
    {
        var supervisors = await employeeService.GetEmployeeSupervisorByRoleIdAsync(employeeId, (short)RoleSystem.ShiftManager);
        return new JsonResult(supervisors.Select(x => new Supervisor
        { OwnerId = x.OwnerId, Nickname = x.NickName, FullName = x.FullName }));
    }

    [HttpPost]
    public IActionResult ToogleLeftNavigation(bool isShow)
    {
        userControlSession.Set<bool>(SessionKey.IsShowLeftNavigation, isShow);
        return Ok();
    }

    [HrmPermission(Policy.Read, FunctionKey.EmployeeManagement)]
    public IActionResult DisciplineManager(int id)
    {
        ViewBag.EmployeeId = id;
        ViewBag.RoleId = (short)RoleSystem.DisciplineManager;
        return View();
    }

    public async Task<JsonResult> GetDisciplineManagerByEmployeeId(int employeeId)
    {
        var supervisors = await employeeService.GetDisciplineSettingByEmployeeIdAsync(employeeId);
        return new JsonResult(supervisors);
    }

    public async Task<JsonResult> GetMemberByManager(string keyword)
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

        return new JsonResult(employees, new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver()
        });
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

    public async Task<JsonResult> GetMemberByGender(string keyword, int gender)
    {
        var employees = await employeeService.GetEmployeeByGender(keyword, gender);
        return new JsonResult(employees, new JsonSerializerSettings { ContractResolver = new DefaultContractResolver() });
    }

    public async Task<JsonResult> GenerateEmployeeCode()
    {
        var newCode = await employeeService.GenerateEmployeeCode();
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
                await UpdateUserSession(param.EmployeeId);
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

    private async Task UpdateUserSession(int employeeId)
    {
        if (EmployeeSession.EmpId == employeeId)
        {
            userControlSession.Set<EmployeeSession>(SessionKey.Employee, await authService.GetByUserIdAsync(userControlSession.UserId));
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

    private static string GetSupervisors(IEnumerable<SupervisorNameModel> supervisors, short roleId)
    {
        var supervisorsName = string.Empty;
        const byte maxDisplayItems = 3;

        if (supervisors == null)
        {
            return supervisorsName;
        }
        var supervisorsFilter = supervisors.Where(x => x.ActionType == roleId).Take(maxDisplayItems).ToList();
        supervisorsName = string.Join(", ", supervisorsFilter.Select(x => x.NickName));

        if (supervisorsFilter.Count > maxDisplayItems)
        {
            supervisorsName += ",...";
        }

        return supervisorsName;
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

    private static string GetActingSupervisors(IEnumerable<SupervisorNameModel> supervisors)
    {
        return GetSupervisors(supervisors, (short)RoleSystem.OnBehalfRequest);
    }

    private static string GetDisciplineSupervisors(IEnumerable<SupervisorNameModel> supervisors)
    {
        return GetSupervisors(supervisors, (short)RoleSystem.DisciplineManager);
    }

    private async Task ComposeViewBagsForEditEmployee()
    {
        var jobLevels = await jobLevelService.GetJobLevel();
        var managementLevels = await managementLevelService.GetManagementLevel();
        ViewBag.JobTitles = await commonDataService.GetLookupDataByCodeAsync(Constants.JobTitle, userControlSession.Get<int>(SessionKey.CurrentCompanyId));
        ViewBag.JobLevel = jobLevels;
        ViewBag.Grades = managementLevels;
        ViewBag.Marriages = Utilities.Marriages;
        ViewBag.Roles = await roleService.GetBySiteIdAsync(userControlSession.SiteId);
    }

    private EditEmployeeParam MappingEditEmployeeParam(Employee model)
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
            Roles = BuildRole(model.Roles),
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

    private string BuildRole(IList<Role> roles)
    {
        var systemRoles = roleService.GetBySiteId(userControlSession.SiteId);
        var defaultRoles = systemRoles.Where(x => x.IsDefault).Select(x => x.RoleId);

        if (roles is { Count: > 0 })
        {
            var result = string.Join(",", roles.Select(i => i.RoleId));
            return roles.Any(x => defaultRoles.Any(y => x.RoleId == y)) ? result : string.Join(",", defaultRoles) + "," + result;
        }
        else
        {
            return string.Join(",", defaultRoles);
        }
    }

    private bool? GetCategory(bool? isLocal = null)
    {
        var category = isLocal;
        if (userControlSession.HasPermissionOnFunction(FunctionKey.ManageAllEmployee, Policy.Read))
        {
            return category;
        }

        if (userControlSession.HasPermissionOnFunction(FunctionKey.ManageLocalEmployee, Policy.Read))
        {
            category = true;
        }
        else
        {
            if (userControlSession.HasPermissionOnFunction(FunctionKey.ManageExpatEmployee, Policy.Read))
            {
                category = false;
            }
        }

        return category;
    }
}
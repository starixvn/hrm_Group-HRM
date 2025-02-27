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
using HRM.Models.Supervisor;
using HRM.Services;
using HRM.Services.Interfaces;
using HRM.ViewModels.Employee;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace HRM.Controllers;

public class EmployeeController(IUserControlSession userControlSession,
    IEmployeeService employeeService,
    ICryptoService cryptoService,
    IUserService userService,
    ICommonDataService commonDataService,
    IJobLevelService jobLevelService,
    IRoleService roleService,
    IWebHostEnvironment env,
    IHRMAuthService hrmService) : BaseController(userControlSession)
{
    [HrmPermission(Policy.Host, FunctionKey.CreateAccount)]
    public ActionResult Create()
    {
        return View(new CreateEmployeeViewModel());
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.CreateAccount)]
    public ActionResult Create(CreateEmployeeViewModel model)
    {
        var param = new CreateEmployeeParam()
        {
            Code = model.Code,
            DomainUsername = model.DomainUsername,
            FullName = model.FullName,
            Nickname = model.Nickname,
            TeamId = model.TeamId
        };

        var commandResult = employeeService.Create(EmployeeSession.EmpId, param);
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
        var result = userService.Create(userModel);

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

        employeeService.UpdateUserId(updatedUserModel);
        return RedirectToAction("Edit", new { id = commandResult.OutputId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [HrmPermission(Policy.Host, FunctionKey.EmployeeManagement)]
    public ActionResult Edit(Employee model)
    {
        if (ModelState.IsValid)
        {
            EditEmployeeParam param = MappingEditEmployeeParam(model);
            var commandResult = employeeService.Edit(EmployeeSession.EmpId, param);
            if (commandResult.IsSuccessful)
            {
                UpdateUserInfo(model.UserId, param);
            }
            else
            {
                ViewBag.Updated = false;
                ModelState.AddModelError(string.Empty, commandResult.Message);
            }
        }

        model.Coutries = commonDataService.GetAllCountry();
        IList<SupervisorNameModel> supervisors = GetSupervisors(model.Id);
        model.Verifier = GetVerifiers(supervisors);
        model.Approver = GetApprovers(supervisors);
        model.Disciplinarian = GetDisciplineSupervisors(supervisors);
        model.EmpShiftList = GetShiftMonitors(supervisors);
        model.ActingEmpList = GetActingSupervisors(supervisors);
        ViewBag.EnableGroupDate = commonDataService.IsEnableGroupDate();

        ComposeViewBagsForEditEmployee();

        return View(model);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.EmployeeManagement)]
    public ActionResult Delete(DeleteEmployeeParam param)
    {
        CommandResult commandResult = employeeService.Delete(EmployeeSession.EmpId, param);
        if (commandResult.IsSuccessful)
        {
            var result = userService.Delete(param.UserId);
            return Json(result);
        }

        return Json(commandResult);
    }

    public JsonResult GenerateEmployeeCode()
    {
        var newCode = employeeService.GenerateEmployeeCode();
        return Json(newCode);
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

        employee.Coutries = commonDataService.GetAllCountry();
        var supervisors = GetSupervisors(id);
        employee.Verifier = GetVerifiers(supervisors);
        employee.Approver = GetApprovers(supervisors);
        employee.Disciplinarian = GetDisciplineSupervisors(supervisors);
        employee.EmpShiftList = GetShiftMonitors(supervisors);
        employee.ActingEmpList = GetActingSupervisors(supervisors);
        ViewBag.LengthOfServiceByGroupDate = employeeService.CaculateLengthOfService(employee.JoinedDate, employee.MilitaryFromDate, employee.MilitaryToDate);
        ViewBag.LengthOfServiceByCompanyDate = employeeService.CaculateLengthOfService(employee.CompanyDate, employee.MilitaryFromDate, employee.MilitaryToDate);
        ViewBag.EnableGroupDate = commonDataService.IsEnableGroupDate();

        ComposeViewBagsForEditEmployee();
        return View(employee);
    }

    public JsonResult GetSupervisors(string roleIds, string query, string excludeIds)
    {
        var criteria = new GetEmployeeByRoleCriteria
        {
            ExcludeIds = excludeIds ?? string.Empty,
            Query = query.Trim(),
            RoleIds = roleIds
        };
        var employees = employeeService.GetEmployeesByRole(criteria);

        return Json(employees);
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

    private IList<SupervisorNameModel> GetSupervisors(int empId)
    {
        var criteria = new GetListSupervisorNameCriteria
        {
            EmpId = empId,
            Role = null,
            RequestType = null
        };
        var supervisors = employeeService.GetListSupervisorName(criteria);
        return supervisors;
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
            ExpertGradeId = model.ExpertGradeId,
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

    private void UpdateUserInfo(int userId, EditEmployeeParam param)
    {
        var result = userService.UpdateInfo(new UpdateUserInfoParam
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
            result = userService.AssignRole(new AssignRoleParam
            {
                UserId = userId,
                RoleIds = param.Roles
            });

            if (result.IsSuccessful)
            {
                ViewBag.Updated = true;
                UpdateUserSession(param.EmployeeId);
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

    private void UpdateUserSession(int employeeId)
    {
        if (EmployeeSession.EmpId == employeeId)
        {
            userControlSession.Set<EmployeeSession>(SessionKey.Employee, hrmService.GetByUserId(userControlSession.UserId));
        }
    }

    private string BuildRole(IList<Role> roles)
    {
        var systemRoles = roleService.GetBySiteId(userControlSession.SiteId);
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

    private void ComposeViewBagsForEditEmployee()
    {
        var jobLevels = jobLevelService.GetJobLevel();
        ViewBag.JobTitles = commonDataService.GetLookupDataByCode(Constants.JobTitle);
        ViewBag.JobLevel = jobLevels;
        ViewBag.Grades = jobLevels?.Where(jl => jl.Name.StartsWith("E")).ToList();
        ViewBag.Marriages = Utilities.Marriages;
        ViewBag.Roles = roleService.GetBySiteId(userControlSession.SiteId);
    }
}
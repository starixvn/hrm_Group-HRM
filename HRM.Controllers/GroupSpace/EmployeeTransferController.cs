using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Models.User;
using GO.UserControl.Client.Core.Services;
using GO.UserControl.Client.Core.Session;
using HRM.Core;
using HRM.Core.Constants;
using HRM.Core.Enums;
using HRM.Core.Filters;
using HRM.Core.Models;
using HRM.Core.Services;
using HRM.Criteria.Employee;
using HRM.Criteria.Group;
using HRM.Models;
using HRM.Models.Group;
using HRM.Services.ExportExcel.Employee;
using HRM.Services.Helper;
using HRM.Services.Implement;
using HRM.Services.Interfaces;
using HRM.ViewModels.EmployeeGroup;
using Microsoft.AspNetCore.Mvc;

namespace HRM.Controllers.GroupSpace;

public class EmployeeTransferController(
    IUserControlSession session,
    IEmployeeService employeeService,
    IUserService userService,
    ICommonDataService commonDataService,
    ICompanyService companyService,
    IJobLevelService jobLevelService,
    IManagementLevelService managementLevelService,
    IBaseService baseService,
    IExportExcelEmployeeInfoService excelService) : BaseController(session)
{
    [HrmPermission(Policy.Read, FunctionKey.EmployeeTransfer_All, FunctionKey.EmployeeTransfer_BU)]
    public IActionResult Index()
    {
        var hasPermission = session.HasPermissionOnFunction(FunctionKey.EmployeeTransfer, Policy.Host);
        return View(hasPermission);
    }

    [HttpGet]
    [HrmPermission(Policy.Read, FunctionKey.EmployeeTransfer_All, FunctionKey.EmployeeTransfer_BU)]
    public async Task<IActionResult> GetEmployeeTransferList()
    {
        var criteria = new GetEmployeeTransferCriteria
        {
            CurrentEmpId = session.Get<EmployeeSession>(SessionKey.Employee).EmpId,
            IsViewAll = session.HasPermissionOnFunction(FunctionKey.EmployeeTransfer_All, Policy.Read)
        };
        var employees = await employeeService.GetEmployeeTransferListAsync(criteria);
        return Json(employees);
    }

    [HrmPermission(Policy.Host, FunctionKey.EmployeeTransfer)]
    public IActionResult Transfer()
    {
        return View();
    }

    [HttpGet]
    [HrmPermission(Policy.Host, FunctionKey.EmployeeTransfer_All, FunctionKey.EmployeeTransfer_BU)]
    public async Task<IActionResult> GroupEmployeeListCanTransfer(GroupEmployeeViewModel model)
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
            employees = FilterEmployeeByPermission(employees).ToList();
        }

        return Json(employees);
    }

    private IEnumerable<GroupEmployeeModel> FilterEmployeeByPermission(
    IEnumerable<GroupEmployeeModel> employees
)
    {
        if (session.HasPermissionOnFunction(FunctionKey.EmployeeTransfer_All, Policy.Host))
        {
            return employees;
        }

        List<int> empIds = [];

        if (session.HasPermissionOnFunction(FunctionKey.EmployeeTransfer_BU, Policy.Host))
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

            var currentEmployee = employees?.FirstOrDefault(e => e.EmpId == EmployeeSession.EmpId);
            if (currentEmployee != null && !string.IsNullOrWhiteSpace(currentEmployee.UnitId))
            {
                empIds.AddRange(
                    employees
                        .Where(e => e.UnitId == currentEmployee.UnitId)
                        .Select(e => e.EmpId)
                        .ToList()
                );
            }

        }
        var distinctEmpIds = empIds.Distinct().ToList();

        return employees.Where(e => distinctEmpIds.Contains(e.EmpId));
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.EmployeeTransfer_All, FunctionKey.EmployeeTransfer_BU)]
    public async Task<IActionResult> TransferEmployee([FromBody] EmployeeTransfer model)
    {
        var result = await employeeService.CheckExistAsync(model.Code, model.NickName);
        if (!result.IsSuccessful)
        {
            return Json(result);
        }

        var newEmpId = await employeeService.TransferEmployeeAsync(model);
        var param = EmployeeHelper.MappingEditEmployeeParam(model, $"{(int)RoleSystem.Member}");
        param.EmployeeId = newEmpId;
        param.Enabled = true;

        var commandResult = await employeeService.EditAsync(
            EmployeeSession.EmpId,
            param,
            model.TargetCompanyId
        );
        if (commandResult.IsSuccessful)
        {
            await employeeService.UpdateUserInfoAsync(model.UserId, param);
            await UpdateUserSiteAndRoles(model, param);
        }

        return Json(commandResult);
    }

    
    private async Task UpdateUserSiteAndRoles(EmployeeTransfer model, EditEmployeeParam param)
    {
        var companySites = await companyService.GetAllCompanySiteAsync();
        var targetSite = companySites.FirstOrDefault(c => c.CompanyId == model.TargetCompanyId);
        if (targetSite == null)
        {
            return;
        }

        var oldSite = companySites.FirstOrDefault(c => c.CompanyId == model.TransferOutCompanyId);
        if (oldSite != null) // Deactivate old site
        {
            await userService.UpdateUserSiteAsync(new UpdateUserSiteParam
            {
                UserId = model.UserId,
                SiteId = oldSite.SiteId,
                IsActive = false
            });
        }

        await userService.UpdateUserSiteAsync(new UpdateUserSiteParam
        {
            UserId = model.UserId,
            SiteId = targetSite.SiteId,
            IsActive = true
        });
        await userService.AssignRoleAsync(new AssignRoleParam
        {
            SiteId = targetSite.SiteId,
            UserId = model.UserId,
            RoleIds = param.Roles
        });
    }

    [HttpGet]
    [HrmPermission(Policy.Host, FunctionKey.EmployeeTransfer_BU, FunctionKey.EmployeeTransfer_All)]
    public async Task<IActionResult> GetInfoEmployeeTransfer(int id, short transferInCompanyId, short transferOutCompanyId)
    {
        var employee = await employeeService.GetEmployeeTransferByIdAsync(id, transferOutCompanyId);

        if (employee != null)
        {
            employee.Coutries = await commonDataService.GetAllCountry();
            employee.LengthOfServiceByGroupDate = employeeService.CalculateLengthOfService(
                employee.JoinedDate,
                employee.MilitaryFromDate,
                employee.MilitaryToDate
            ).LengthOfServiceText;
            employee.JobLevels = (await jobLevelService.GetJobLevel()).ToList();
            employee.ManagementLevels = (await managementLevelService.GetManagementLevel()).ToList();
            employee.SubCompanies = (await companyService.GetSubCompanies(transferInCompanyId)).ToList();
            employee.JobTitles = (await commonDataService.GetLookupDataByCodeAsync(Constants.JobTitle, transferInCompanyId)).ToList();
            var settings = await commonDataService.GetSystemSettingAsync(transferOutCompanyId);
            employee.EnableSickLeaveBalance = settings.EnableSickLeaveBalance;
        }

        return Json(employee);
    }

    
    [HrmPermission(Policy.Read, FunctionKey.EmployeeTransfer)]
    public async Task<IActionResult> ExportEmployeeTransferList()
    {
        var criteria = new GetEmployeeTransferCriteria
        {
            CurrentEmpId = session.Get<EmployeeSession>(SessionKey.Employee).EmpId,
            IsViewAll = session.HasPermissionOnFunction(FunctionKey.EmployeeTransfer_All, Policy.Read)
        };
        var employees = await employeeService.GetEmployeeTransferListAsync(criteria);
        var bytes = await excelService.ExportGroupEmployeesTransferAsync(employees);
        var fileName = $"EmployeesTransfer_{DateTimeOffset.Now.DateTime.ToClientTimeZone():yyyyMMdd}.xlsx";

        return File(bytes, MimeType.XLS, fileName);
    }
}

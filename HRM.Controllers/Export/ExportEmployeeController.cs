using System;
using System.Linq;
using System.Threading.Tasks;
using GO.UserControl.Client.Core.Session;
using HRM.Core;
using HRM.Core.Constants;
using HRM.Core.Services;
using HRM.Criteria.Employee;
using HRM.Criteria.Group;
using HRM.Criteria.Report;
using HRM.Models.Group;
using HRM.Services.ExportExcel.Employee;
using HRM.Services.Implement;
using HRM.Services.Interfaces;
using HRM.ViewModels.Employee;
using HRM.ViewModels.EmployeeGroup;
using HRM.ViewModels.Report;
using Microsoft.AspNetCore.Mvc;

namespace HRM.Controllers.Export;

public class ExportEmployeeController(
    IUserControlSession userControlSession,
    IEmployeeService employeeService,
    IExportExcelEmployeeInfoService excelService,
    IBaseService baseService) : BaseExportController(userControlSession)
{
    public async Task<IActionResult> EmployeeBasicInfo(UserManagementViewModel model)
    {
        var criteria = new GetAllEmployeeCriteria
        {
            Query = model.Keyword,
            TeamIds = model.TeamIds,
            EnableUser = model.IsEnable,
            IsDeleteUser = model.IsDelete,
            TitleId = model.JobTitleId,
            IsLocalUser = model.IsLocal,
            CompanyId = CurrentCompanyId
        };

        var employees = await employeeService.GetEmployeeAsync(criteria);
        var bytes = await excelService.ExportEmployeeBasicInfo(employees.ToList());
        var fileName = $"EmployeeBasicInfo_{DateTimeOffset.Now.DateTime.ToClientTimeZone():yyyyMMdd}.xls";

        return File(bytes, MimeType.XLS, fileName);
    }

    public async Task<IActionResult> EmployeeDetailsInfo(UserManagementViewModel model)
    {
        var criteria = new GetEmployeeFullProfileCriteria
        {
            Query = model.Keyword,
            TeamIds = model.TeamIds,
            EnableUser = model.IsEnable,
            IsDeleteUser = model.IsDelete,
            TitleId = model.JobTitleId,
            IsLocalUser = model.IsLocal
        };

        var profiles = await employeeService.GetEmployeeFullProfile(criteria);
        var bytes = await excelService.ExportEmployeeFullProfile(profiles);
        var fileName = $"EmployeeFullInfo_{DateTimeOffset.Now.DateTime.ToClientTimeZone():yyyyMMdd}.xls";

        return File(bytes, MimeType.XLS, fileName);
    }

    public async Task<IActionResult> ExportGroupEmployeeList(GroupEmployeeViewModel model)
    {
        var criteria = new GetEmployeesInGroupScopeCriteria
        {
            Query = model.Keyword,
            TeamIds = model.TeamIds,
            EnableUser = model.IsEnable,
            IsDeleteUser = model.IsDelete,
            TitleId = model.JobTitleId,
            CompanyIds = model.CompanyIds
        };

        var profiles = (await baseService.GetDataAsync<GroupEmployeeModel>(criteria)).ToList();
        var bytes = await excelService.ExportGroupEmployeesAsync(profiles);
        var fileName = $"GroupEmployeeList_{DateTimeOffset.Now.DateTime.ToClientTimeZone():yyyyMMdd}.xlsx";

        return File(bytes, MimeType.XLS, fileName);
    }
}
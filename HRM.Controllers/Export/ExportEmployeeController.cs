using System;
using System.Linq;
using System.Threading.Tasks;
using GO.UserControl.Client.Core.Session;
using HRM.Criteria.Employee;
using HRM.Services.ExportExcel.Employee;
using HRM.Services.Interfaces;
using HRM.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HRM.Controllers.Export;

public class ExportEmployeeController(
    IUserControlSession userControlSession,
    IEmployeeService employeeService,
    IExportExcelEmployeeInfoService excelService)
    : BaseExportController(userControlSession)
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
            IsLocalUser = model.IsLocal
        };

        var employees = await employeeService.GetEmployeeAsync(criteria);
        var bytes = await excelService.ExportEmployeeBasicInfo(employees.ToList());
        var fileName = "EmployeeBasicInfo_" + DateTimeOffset.Now.ToString("yyyyMMdd");

        return File(bytes, "application/xlsx", $"{fileName}.xlsx");
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
        var fileName = "EmployeeFullInfo_" + DateTimeOffset.Now.ToString("yyyyMMdd");
        
        return File(bytes, "application/xlsx", $"{fileName}.xlsx");
    }
}
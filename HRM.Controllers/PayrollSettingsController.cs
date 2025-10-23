using System.Threading.Tasks;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Session;
using HRM.Core.Constants;
using HRM.Core.Filters;
using HRM.Core.Services;
using HRM.Criteria.PayrollManagement;
using Microsoft.AspNetCore.Mvc;

namespace HRM.Controllers;

public class PayrollSettingsController(
    IUserControlSession session,
    IBaseService baseService) : BaseController(session)
{
    [HrmPermission(Policy.Read, FunctionKey.PayrollManagement)]
    public async Task<IActionResult> Index()
    {
        var hasPermission = session.HasPermissionOnFunction(FunctionKey.SalaryStructureConfiguration, Policy.Host);
        return View(hasPermission);
    }

    //[HrmPermission(Policy.Read, FunctionKey.PayrollManagement)]
    //[HttpGet]
    //public async Task<IActionResult> GetPayrollSettings()
    //{
    //    var criteria = new GetPayrollItemSettingsCriteria
    //    {
    //        CompanyId = CurrentCompanyId
    //    };

    //    var result = await payrollSettingService.GetPayrollItemSettingsAsync(criteria);

    //    return Ok(result);
    //}

    [HttpDelete]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        var criteria = new DeleteAllowanceItemCriteria
        {
            AllowanceItemSettingId = id
        };

        var result = await baseService.ExecuteNonQueryAsync(criteria);

        return Ok(result);
    }
}
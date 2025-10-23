using System.Globalization;
using System.Threading.Tasks;
using Fanex.Data;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Session;
using HRM.Core.Constants;
using HRM.Core.Filters;
using HRM.Core.Services;
using HRM.Criteria.PayrollManagement;
using HRM.Models.PayrollManagement;
using HRM.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HRM.Controllers;

public class PayrollSettingsController(
    IUserControlSession session,
    IBaseService baseService,
    IPayrollSettingService payrollSettingService) : BaseController(session)
{
    [HrmPermission(Policy.Read, FunctionKey.PayrollSetting)]
    public async Task<IActionResult> Index()
    {
        var hasPermission = session.HasPermissionOnFunction(FunctionKey.PayrollSetting, Policy.Host);
        return View(hasPermission);
    }

    [HttpGet]
    [HrmPermission(Policy.Read, FunctionKey.PayrollSetting)]
    public async Task<IActionResult> GetPayrollSettings()
    {
        var criteria = new GetPayrollItemSettingsCriteria
        {
            CompanyId = CurrentCompanyId
        };
            
        var result = await payrollSettingService.GetPayrollItemSettings(criteria);

        return Ok(result);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.PayrollSetting)]
    public async Task<IActionResult> Create([FromBody] PayrollAllowanceCreateRequest request)
    {
        var criteria = new CreateAllowanceItemCriteria
        {
            PayrollItemSettingId = request.PayrollItemSettingId,
            Amount = request.Amount,
            IsActive = request.IsActive,
            IsPercent = request.IsPercent,
            ItemName = request.ItemName,
            ModifiedBy = EmployeeSession.EmpId
        };
        criteria.Amount = Cryptography.EncryptPassword(request.Amount.ToString(CultureInfo.InvariantCulture));

        var result = await baseService.ExecuteNonQueryAsync(criteria);

        return Ok(result);
    }

    [HttpPut]
    [HrmPermission(Policy.Host, FunctionKey.PayrollSetting)]
    public async Task<IActionResult> Edit([FromBody] PayrollAllowanceEditRequest request)
    {
        var criteria = new EditAllowanceItemCriteria
        {
            AllowanceItemSettingId = request.AllowanceItemSettingId,
            Amount = request.Amount,
            IsActive = request.IsActive,
            IsPercent = request.IsPercent,
            ItemName = request.ItemName,
            ModifiedBy = EmployeeSession.EmpId
        };
        criteria.Amount = Cryptography.EncryptPassword(request.Amount.ToString(CultureInfo.InvariantCulture));

        var result = await baseService.ExecuteNonQueryAsync(criteria);

        return Ok(result);
    }

    [HttpDelete]
    [HrmPermission(Policy.Host, FunctionKey.PayrollSetting)]
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
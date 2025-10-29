using System.Globalization;
using System.Threading.Tasks;
using Fanex.Data;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Session;
using HRM.Core.Constants;
using HRM.Core.Enums;
using HRM.Core.Filters;
using HRM.Criteria.EmployeePayrollSetting;
using HRM.Criteria.PayrollManagement;
using HRM.Models.PayrollManagement;
using HRM.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HRM.Controllers;

public class PayrollSettingsController(
    IUserControlSession session,
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

    [HttpGet]
    [HrmPermission(Policy.Read, FunctionKey.PayrollSetting)]
    public async Task<IActionResult> GetPayrollSettingTypes()
    {
        var criteria = new GetPayrollSettingTypeCriteria
        {
            CompanyId = CurrentCompanyId,
            PayrollSettingCategory = PayrollSettingType.Allowance
        };

        var result = await payrollSettingService.GetPayrollItemSettingTypes(criteria);

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

        var result = await payrollSettingService.CreateAllowanceItem(criteria);

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

        var result = await payrollSettingService.EditAllowanceItem(criteria);

        return Ok(result);
    }

    [HttpDelete]
    [HrmPermission(Policy.Host, FunctionKey.PayrollSetting)]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        var criteria = new DeleteAllowanceItemCriteria
        {
            AllowanceItemSettingId = id,
            ModifiedBy = EmployeeSession.EmpId
        };

        var result = await payrollSettingService.DeleteAllowanceItem(criteria);

        return Ok(result);
    }
}
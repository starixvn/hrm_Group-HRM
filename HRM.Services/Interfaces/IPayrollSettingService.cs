using System.Collections.Generic;
using System.Threading.Tasks;
using HRM.Core.Models;
using HRM.Criteria.PayrollManagement;
using HRM.Models.PayrollManagement;

namespace HRM.Services.Interfaces;

public interface IPayrollSettingService
{
    Task<IEnumerable<PayrollSetting>> GetPayrollItemSettings(GetPayrollItemSettingsCriteria criteria);
    Task<CommandResult> CreateAllowanceItem(CreateAllowanceItemCriteria criteria);
    Task<CommandResult> EditAllowanceItem(EditAllowanceItemCriteria criteria);
    Task<CommandResult> DeleteAllowanceItem(DeleteAllowanceItemCriteria criteria);
    Task<IEnumerable<PayrollSetting>> GetPayrollItemSettingsAsync(GetPayrollItemSettingsCriteria criteria);
    Task<IEnumerable<PayrollAllowanceItem>> GetAllAllowanceItemSettingsAsync(int companyId);
}


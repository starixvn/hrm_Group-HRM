using System.Collections.Generic;
using System.Threading.Tasks;
using HRM.Criteria.PayrollManagement;
using HRM.Models.PayrollManagement;

namespace HRM.Services.Interfaces;

public interface IPayrollSettingService
{
    Task<IEnumerable<PayrollSetting>> GetPayrollItemSettings(GetPayrollItemSettingsCriteria criteria);
    Task<IEnumerable<PayrollSetting>> GetPayrollItemSettingsAsync(GetPayrollItemSettingsCriteria criteria);
    Task<IEnumerable<PayrollAllowanceItem>> GetAllAllowanceItemSettingsAsync(int companyId);
}


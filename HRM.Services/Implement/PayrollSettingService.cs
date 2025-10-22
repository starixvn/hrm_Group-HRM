using System.Collections.Generic;
using System.Threading.Tasks;
using HRM.Core.Services;
using HRM.Criteria.PayrollManagement;
using HRM.Models.PayrollManagement;
using HRM.Repositories.Interfaces;
using HRM.Services.Interfaces;

namespace HRM.Services.Implement;

public class PayrollSettingService(IPayrollSettingRepository payrollSettingRepository, IBaseService baseService) : IPayrollSettingService
{
    public async Task<IEnumerable<PayrollSetting>> GetPayrollItemSettingsAsync(GetPayrollItemSettingsCriteria criteria)
    {
        return await payrollSettingRepository.GetPayrollItemSettings(criteria);
    }

    public async Task<IEnumerable<PayrollAllowanceItem>> GetAllAllowanceItemSettingsAsync(int companyId)
    {
        var criteria = new GetAllowanceItemSettingCriteria
        {
            CompanyId = companyId
        };

        return await baseService.GetDataAsync<PayrollAllowanceItem>(criteria);
    }
}


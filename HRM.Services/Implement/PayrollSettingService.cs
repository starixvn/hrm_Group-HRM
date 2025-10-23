using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Fanex.Data;
using HRM.Core.Services;
using HRM.Criteria.PayrollManagement;
using HRM.Models.PayrollManagement;
using HRM.Services.Interfaces;

namespace HRM.Services.Implement;

public class PayrollSettingService(IPayrollManagementService payrollManagementService, IBaseService baseService) : IPayrollSettingService
{
    public async Task<IEnumerable<PayrollSetting>> GetPayrollItemSettings(GetPayrollItemSettingsCriteria criteria)
    {
        using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        using var multi = await db.QueryMultipleAsync(criteria);
        var flatRows = multi.Read<PayrollSetting, PayrollAllowanceItem, PayrollSetting>(
        (setting, item) =>
        {
            if (item != null)
            {
                setting.PayrollAllowanceItems.Add(item);
            }

            return setting;
        },
            splitOn: "AllowanceItemSettingId"
        ).ToList();

        var grouped = flatRows
            .GroupBy(s => s.PayrollItemSettingId)
            .Select(g =>
            {
                var setting = g.First();
                setting.PayrollAllowanceItems = g
                    .SelectMany(x => x.PayrollAllowanceItems)
                    .DistinctBy(x => x.AllowanceItemSettingId)
                    .Select(item =>
                    {
                        item.Amount = payrollManagementService
                            .DecryptSalary(item.Amount)
                            .ToString(CultureInfo.InvariantCulture);
                        return item;
                    })
                    .ToList();
                return setting;
            })
            .ToList();

        return grouped;
    }

    public async Task<IEnumerable<PayrollSetting>> GetPayrollItemSettingsAsync(GetPayrollItemSettingsCriteria criteria)
    {
        return await GetPayrollItemSettings(criteria);
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


using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using HRM.Core.Models;
using HRM.Core.Services;
using HRM.Criteria.EmployeePayrollSetting;
using HRM.Criteria.PayrollManagement;
using HRM.Models;
using HRM.Models.PayrollManagement;
using HRM.Services.Helper;
using HRM.Services.Interfaces;

namespace HRM.Services.Implement;

public class PayrollSettingService(
    IBaseService baseService,
    IObjectDbFactory objectDbFactory) : IPayrollSettingService
{
    public async Task<IEnumerable<PayrollSetting>> GetPayrollItemSettings(GetPayrollItemSettingsCriteria criteria)
    {
        using var db = objectDbFactory.CreateInstance(criteria.GetSettingKey());
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
                        item.Amount = PayrollHelper.DecryptSalary(item.Amount)
                            .ToString(CultureInfo.InvariantCulture);
                        return item;
                    })
                    .ToList();
                return setting;
            })
            .ToList();

        return grouped;
    }

    public async Task<IEnumerable<PayrollItemSettingType>> GetPayrollItemSettingTypes(GetPayrollSettingTypeCriteria criteria)
    {
        return await baseService.GetDataAsync<PayrollItemSettingType>(criteria);
    }

    public async Task<CommandResult> CreateAllowanceItem(CreateAllowanceItemCriteria criteria)
    {
        var result = await baseService.ExecuteNonQueryAsync(criteria);

        return result;
    }

    public async Task<CommandResult> EditAllowanceItem(EditAllowanceItemCriteria criteria)
    {
        var result = await baseService.ExecuteNonQueryAsync(criteria);

        return result;
    }

    public async Task<CommandResult> DeleteAllowanceItem(DeleteAllowanceItemCriteria criteria)
    {
        var result = await baseService.ExecuteNonQueryAsync(criteria);

        return result;
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


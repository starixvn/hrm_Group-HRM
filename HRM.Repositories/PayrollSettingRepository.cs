using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fanex.Data;
using HRM.Models.PayrollManagement;
using HRM.Repositories.Interfaces;
using HRM.Criteria.PayrollManagement;

namespace HRM.Repositories
{
    public class PayrollSettingRepository : IPayrollSettingRepository
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

            return flatRows;
        }
    }
}

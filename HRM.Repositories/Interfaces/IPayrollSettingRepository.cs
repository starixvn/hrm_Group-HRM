using System.Collections.Generic;
using System.Threading.Tasks;
using HRM.Criteria.PayrollManagement;
using HRM.Models.PayrollManagement;

namespace HRM.Repositories.Interfaces
{
    public interface IPayrollSettingRepository
    {
        Task<IEnumerable<PayrollSetting>> GetPayrollItemSettings(GetPayrollItemSettingsCriteria criteria);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

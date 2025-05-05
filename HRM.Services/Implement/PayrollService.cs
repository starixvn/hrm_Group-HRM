using System.Collections.Generic;
using System.Threading.Tasks;
using HRM.Core.Services;
using HRM.Criteria.EmployeePayroll;
using HRM.Models.Payroll;
using HRM.Services.Interfaces;

namespace HRM.Services.Implement;

public class PayrollService(IBaseService baseService) : IPayrollService
{
    public async Task<IEnumerable<EmployeePayroll>> GetEmployeePayrollAsync(GetEmployeePayrollCriteria criteria)
    {
        return await baseService.GetDataAsync<EmployeePayroll>(criteria);
    }
}
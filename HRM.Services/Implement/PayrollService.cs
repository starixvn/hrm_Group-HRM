using System.Collections.Generic;
using System.Threading.Tasks;
using HRM.Core.Models;
using HRM.Core.Services;
using HRM.Criteria.EmployeePayroll;
using HRM.Criteria.PayrollManagement;
using HRM.Models.Payroll;
using HRM.Services.Interfaces;

namespace HRM.Services.Implement;

public class PayrollService(IBaseService baseService) : IPayrollService
{
    public async Task<IEnumerable<EmployeePayroll>> GetUploadEmployeePayrollAsync(GetEmployeePayrollCriteria criteria)
    {
        return await baseService.GetDataAsync<EmployeePayroll>(criteria);
    }

    public async Task<IEnumerable<UploadHistory>> GetUploadHistoryAsync(short companyId)
    {
        return await baseService.GetDataAsync<UploadHistory>(new GetSalaryUploadHistoryCriteria { CompanyId = companyId });
    }

    public async Task<CommandResult> ConfirmUploadPayrollAsync(ConfirmUploadPayrollCriteria criteria)
    {
        return await baseService.ExecuteNonQueryAsync(criteria);
    }
}
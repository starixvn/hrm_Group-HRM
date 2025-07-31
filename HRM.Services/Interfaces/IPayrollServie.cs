using System.Collections.Generic;
using System.Threading.Tasks;
using HRM.Core.Models;
using HRM.Criteria.EmployeePayroll;
using HRM.Criteria.PayrollManagement;
using HRM.Models.Payroll;

namespace HRM.Services.Interfaces;

public interface IPayrollService
{
    Task<IEnumerable<EmployeePayroll>> GetUploadEmployeePayrollAsync(GetEmployeePayrollCriteria criteria);
    Task<IEnumerable<UploadHistory>> GetUploadHistoryAsync(short companyId);
    Task<CommandResult> ConfirmUploadPayrollAsync(ConfirmUploadPayrollCriteria criteria);
}
using System.Collections.Generic;
using System.Threading.Tasks;
using HRM.Criteria.EmployeePayroll;
using HRM.Models.Payroll;

namespace HRM.Services.Interfaces;

public interface IPayrollService
{
    Task<IEnumerable<EmployeePayroll>> GetEmployeePayrollAsync(GetEmployeePayrollCriteria criteria);
}
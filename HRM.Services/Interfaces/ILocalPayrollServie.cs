using System.Collections.Generic;
using System.Threading.Tasks;
using HRM.Models.PayrollManagement;

namespace HRM.Services.Interfaces;

public interface ILocalPayrollService
{
    Task<Dictionary<int, decimal>> CalculateMonthlySalary(IEnumerable<int> empIds, bool isLocal);
    Task<Dictionary<int, PayrollDetail>> CalculateMonthlySalaryDetail(IEnumerable<int> empIds, bool isLocal);
}
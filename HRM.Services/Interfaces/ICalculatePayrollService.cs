using System.Collections.Generic;
using System.Threading.Tasks;
using HRM.Models.PayrollManagement;

namespace HRM.Services.Interfaces;

public interface ICalculatePayrollService
{
    Task<Dictionary<int, PayrollDetail>> CalculateLocalMonthlySalary(IEnumerable<int> empIds, Dictionary<string, decimal> exchangeRates);

    Task<Dictionary<int, PayrollDetail>> CalculateExpatMonthlySalary(IEnumerable<int> empIds, Dictionary<string, decimal> exchangeRates);
}
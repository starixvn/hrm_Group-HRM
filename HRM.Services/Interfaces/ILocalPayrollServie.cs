using System.Collections.Generic;
using System.Threading.Tasks;

namespace HRM.Services.Interfaces;

public interface ILocalPayrollService
{
    Task<Dictionary<int, decimal>> CalculateMonthlySalary(IEnumerable<int> empIds);
}
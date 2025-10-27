using System.Collections.Generic;
using System.Threading.Tasks;

namespace HRM.Services.Interfaces;

public interface IExpatPayrollService
{
    Task<Dictionary<int, decimal>> CalculateMonthlySalary(IEnumerable<int> empIds);
}


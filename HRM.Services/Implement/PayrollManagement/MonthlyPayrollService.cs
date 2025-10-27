using HRM.Models.Group;
using HRM.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRM.Services.Implement.PayrollManagement;

public class MonthlyPayrollService(ILocalPayrollService localPayrollService, IExpatPayrollService expatPayrollService) : IMonthlyPayrollService
{
    public async Task CalculateMonthlySalary(IEnumerable<GroupEmployeeModel> employees)
    {
        var localIds = employees.Where(e => e.IsLocal).Select(x => x.EmpId).Distinct().ToList();
        var expatIds = employees.Where(e => !e.IsLocal).Select(x => x.EmpId).Distinct().ToList();

        var tasks = new List<Task<Dictionary<int, decimal>>>();
        if (localIds.Count > 0)
        {
            tasks.Add(localPayrollService.CalculateMonthlySalary(localIds));
        }

        if (expatIds.Count > 0)
        {
            tasks.Add(expatPayrollService.CalculateMonthlySalary(expatIds));
        }

        var results = await Task.WhenAll(tasks);

        var payrolls = results.SelectMany(r => r).ToDictionary(x => x.Key, x => x.Value);
        foreach (var employee in employees)
        {
            employee.Salary = payrolls.GetValueOrDefault(employee.EmpId);
        }
    }
}


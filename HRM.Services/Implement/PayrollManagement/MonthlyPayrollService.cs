using HRM.Models.Group;
using HRM.Models.PayrollManagement;
using HRM.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRM.Core;
using HRM.Services.Helper;

namespace HRM.Services.Implement.PayrollManagement;

public class MonthlyPayrollService(ICalculatePayrollService payrollService, IExchangeRateService exchangeRateService) : IMonthlyPayrollService
{
    public async Task CalculateMonthlySalary(IEnumerable<GroupEmployeeModel> employees)
    {
        var exchangeRates = await exchangeRateService.GetEffectiveExchangeRatesDictAsync();
        var roundingRules = await exchangeRateService.GetRoundingRulesDictAsync();
        var localIds = employees.Where(e => e.IsLocal).Select(x => x.EmpId).Distinct().ToList();
        var expatIds = employees.Where(e => !e.IsLocal).Select(x => x.EmpId).Distinct().ToList();

        var tasks = new List<Task<Dictionary<int, PayrollDetail>>>();
        if (localIds.Count > 0)
        {
            tasks.Add(payrollService.CalculateLocalMonthlySalary(localIds, exchangeRates));
        }

        if (expatIds.Count > 0)
        {
            tasks.Add(payrollService.CalculateExpatMonthlySalary(expatIds, exchangeRates));
        }

        var results = await Task.WhenAll(tasks);

        var payrolls = results.SelectMany(r => r).ToDictionary(x => x.Key, x => x.Value);
        foreach (var employee in employees)
        {
            var detail = payrolls.GetValueOrDefault(employee.EmpId);
            if (detail != null)
            {
                employee.Salary = detail.TotalDisplay = CurrencyConverter.RoundUSDAmount(detail.Total, roundingRules);
                employee.BasicSalary = detail.BasicSalaryDisplay;
                employee.JobLevelAddOn = detail.JobLevelAddOnDisplay;
                employee.ManagementAllowance = detail.ManagementAllowanceDisplay;
                employee.OverseasAllowance = detail.OverseasAllowanceDisplay;
                employee.SkillSetAddOn = detail.SkillSetAddOnDisplay;
                employee.Others = detail.OthersDisplay;
            }
        }
    }
}


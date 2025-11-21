using System;
using HRM.Core.Services;
using HRM.Criteria.Group;
using HRM.Models.PayrollManagement;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fanex.Data;
using HRM.Services.Helper;
using HRM.Services.Interfaces;

namespace HRM.Services.Implement.PayrollManagement;

public class CalculatePayrollService(IBaseService baseService): ICalculatePayrollService
{
    public Task<Dictionary<int, PayrollDetail>> CalculateLocalMonthlySalary(IEnumerable<int> empIds, Dictionary<string, decimal> exchangeRates) 
        => CalculateMonthlySalaryDetail(empIds, exchangeRates, true);

    public Task<Dictionary<int, PayrollDetail>> CalculateExpatMonthlySalary(IEnumerable<int> empIds, Dictionary<string, decimal> exchangeRates)
        => CalculateMonthlySalaryDetail(empIds, exchangeRates, false);

    public async Task<Dictionary<int, PayrollDetail>> CalculateMonthlySalaryDetail(IEnumerable<int> empIds, Dictionary<string, decimal> exchangeRates, bool isLocal)
    {
        var (payrolls, allowancesByEmp, decryptionCache) = await FetchPayrollData(empIds, isLocal);

        return payrolls.ToDictionary(
            p => p.EmpId,
            p => BuildPayrollDetail(p, allowancesByEmp[p.EmpId], decryptionCache, exchangeRates)
        );
    }

    private async Task<(IEnumerable<Payroll> payrolls, ILookup<int, LocalPayrollAllowance> allowancesByEmp, Dictionary<string, decimal> decryptionCache)> 
        FetchPayrollData(IEnumerable<int> empIds, bool isLocal)
    {
        var criteria = new GetPayrollInfoCriteria
        {
            EmpIds = string.Join(",", empIds),
            IsLocal = isLocal
        };
        var (payrolls, payrollAllowances) = await baseService.FetchMultipleAsync<Payroll, LocalPayrollAllowance>(criteria);

        var allowancesByEmp = payrollAllowances.ToLookup(x => x.EmpId);
        var decryptionCache = BatchDecryptSalaries(payrolls, payrollAllowances);

        return (payrolls, allowancesByEmp, decryptionCache);
    }

    private static PayrollDetail BuildPayrollDetail(Payroll payroll, IEnumerable<LocalPayrollAllowance> allowances, Dictionary<string, decimal> cache, Dictionary<string, decimal> exchangeRates)
    {
        var detail = new PayrollDetail
        {
            BasicSalary = DecryptedAndExchangeRate(cache, payroll.EncryptBasicSalary, payroll.CurrencyCode, exchangeRates),
            JobLevelAddOn = DecryptedAndExchangeRate(cache, payroll.EncryptJobLevelAddOn, payroll.CurrencyCode, exchangeRates),
            Others = DecryptedAndExchangeRate(cache, payroll.EncryptSpecialSubsidy, payroll.SpecialSubsidyCurrencyCode, exchangeRates)
        };

        foreach (var a in allowances)
        {
            var amount = DecryptedAndExchangeRate(cache, a.EncryptAmount, a.CurrencyCode, exchangeRates);
            switch (a.Type?.Trim().ToLowerInvariant())
            {
                case "management allowance":
                    detail.ManagementAllowance += amount; 
                    break;
                case "overseas allowance":
                    detail.OverseasAllowance += amount;
                    break;
                case "skill set add-on":
                    detail.SkillSetAddOn += amount;
                    break;
                default:
                    detail.Others += amount; 
                    break;
            }
        }

        return detail;
    }

    private static Dictionary<string, decimal> BatchDecryptSalaries(IEnumerable<Payroll> payrolls, IEnumerable<LocalPayrollAllowance> allowances)
    {
        var cache = new ConcurrentDictionary<string, decimal>();
        var encryptedValues = payrolls
            .SelectMany(p => new[]
            {
                p.EncryptBasicSalary,
                p.EncryptJobLevelAddOn,
                p.EncryptSpecialSubsidy
            })
            .Concat(allowances.Select(a => a.EncryptAmount))
            .Distinct()
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .ToHashSet();

        Parallel.ForEach(encryptedValues, encryptedValue =>
        {
            var decryptedValue = Cryptography.DecryptPassword(encryptedValue);
            cache[encryptedValue] = decimal.Parse(decryptedValue);
        });
        
        return new Dictionary<string, decimal>(cache);
    }

    private static decimal DecryptedAndExchangeRate(Dictionary<string, decimal> cache, string encryptedValue, string currencyCode, Dictionary<string, decimal> exchangeRates)
    {
        var decryptValue = string.IsNullOrEmpty(encryptedValue) ? 0 : cache.GetValueOrDefault(encryptedValue, 0);

        return CurrencyConverter.ConvertToUsd(decryptValue, currencyCode, exchangeRates);
    }
}
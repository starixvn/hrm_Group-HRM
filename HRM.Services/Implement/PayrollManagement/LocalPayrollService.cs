using HRM.Core.Services;
using HRM.Criteria.Group;
using HRM.Models.PayrollManagement;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fanex.Data;
using HRM.Services.Interfaces;

namespace HRM.Services.Implement.PayrollManagement;

public class LocalPayrollService(IBaseService baseService): ILocalPayrollService
{
    public async Task<Dictionary<int, decimal>> CalculateMonthlySalary(IEnumerable<int> empIds)
    {
        var criteria = new GetPayrollInfoCriteria
        {
            EmpIds = string.Join(",", empIds),
        };

        var (payrolls, payrollAllowances) = await baseService.FetchMultipleAsync<Payroll, LocalPayrollAllowance>(criteria);
        var allowancesByEmpId = payrollAllowances.ToLookup(x => x.EmpId);
        var decryptionCache = BatchDecryptSalaries(payrolls, payrollAllowances);

        var result = new Dictionary<int, decimal>();
        foreach (var payroll in payrolls)
        {
            var total = GetDecryptedValue(decryptionCache, payroll.EncryptBasicSalary)
                            + GetDecryptedValue(decryptionCache, payroll.EncryptJobLevelAddOn)
                            + GetDecryptedValue(decryptionCache, payroll.EncryptSpecialSubsidy);

            var empAllowances = allowancesByEmpId[payroll.EmpId];
            foreach (var allowance in empAllowances)
            {
                total += GetDecryptedValue(decryptionCache, allowance.EncryptAmount);
            }

            result[payroll.EmpId] = total;
        }

        return result;
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

    private static decimal GetDecryptedValue(Dictionary<string, decimal> cache, string encryptedValue) => string.IsNullOrEmpty(encryptedValue) ? 0 : cache.GetValueOrDefault(encryptedValue, 0);
}
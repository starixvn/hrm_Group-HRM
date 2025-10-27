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

public class ExpatPayrollService(IBaseService baseService): IExpatPayrollService
{
    public async Task<Dictionary<int, decimal>> CalculateMonthlySalary(IEnumerable<int> empIds)
    {
        var criteria = new GetPayrollInfoCriteria // TODO: Create a specific criteria for Expat later
        {
            EmpIds = string.Join(",", empIds),
        };

        var (payrolls, _) = await baseService.FetchMultipleAsync<Payroll, LocalPayrollAllowance>(criteria);
        var decryptionCache = BatchDecryptSalaries(payrolls);

        var result = new Dictionary<int, decimal>();
        foreach (var payroll in payrolls)
        {
            var total = GetDecryptedValue(decryptionCache, payroll.EncryptExpatSalary);
            result[payroll.EmpId] = total;
        }

        return result;
    }

    private static Dictionary<string, decimal> BatchDecryptSalaries(IEnumerable<Payroll> payrolls)
    {
        var cache = new ConcurrentDictionary<string, decimal>();
        var encryptedValues = payrolls
            .SelectMany(p => new[]
            {
                p.EncryptExpatSalary,
            })
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


using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fanex.Data;
using HRM.Core.Models;
using HRM.Core.Services;
using HRM.Criteria.SalaryStructure;
using HRM.Models.SalaryStructure;
using HRM.Services.Interfaces;

namespace HRM.Services.Implement;

public class SalaryStructureService(IBaseService baseService) : ISalaryStructureService
{
    public async Task<IEnumerable<SalaryStructure>> GetSalaryStructureAsync(int CompanyId)
    {
        var criteria = new GetSalaryStructureCriteria
        {
            CompanyId = CompanyId
        };
        var (salaryStructures, details) = await baseService.FetchMultipleAsync<SalaryStructure, SalaryStructureDetail>(criteria);
        foreach (var item in details)
        {
            item.LocalSalary = !string.IsNullOrWhiteSpace(item.LocalSalary)
                ? Cryptography.DecryptPassword(item.LocalSalary)
                : null;

            item.ExpatSalary = !string.IsNullOrWhiteSpace(item.ExpatSalary)
                ? Cryptography.DecryptPassword(item.ExpatSalary)
                : null;
        }

        foreach (var structure in salaryStructures)
        {
            structure.Details = details.Where(x => x.SalaryStructureId == structure.SalaryStructureId);
        }

        return salaryStructures;
    }

    public async Task<SalaryStructure> GetByIdAsync(int Id)
    {
        var criteria = new GetSalaryStructureByIdCriteria
        {
            SalaryStructureId = Id
        };
        var (salaryStructures, details) = await baseService.FetchMultipleAsync<SalaryStructure, SalaryStructureDetail>(criteria);
        var salaryStructure = salaryStructures.FirstOrDefault();
        if (salaryStructure != null)
        {
            foreach (var item in details)
            {
                item.LocalSalary = !string.IsNullOrWhiteSpace(item.LocalSalary)
                    ? Cryptography.DecryptPassword(item.LocalSalary)
                    : null;

                item.ExpatSalary = !string.IsNullOrWhiteSpace(item.ExpatSalary)
                    ? Cryptography.DecryptPassword(item.ExpatSalary)
                    : null;
            }

            salaryStructure.Details = details;
        }

        return salaryStructure;
    }

    public async Task<CommandResult> DeleteAsync(int actorId, int id)
    {
        var criteria = new DeleteSalaryStructureCriteria
        {
            SalaryStructureId = id,
            DeletedBy = actorId
        };
        return await baseService.ExecuteNonQueryAsync(criteria);
    }
}
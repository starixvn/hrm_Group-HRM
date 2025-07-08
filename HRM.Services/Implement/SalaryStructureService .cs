namespace HRM.Services.Implement;

using System.Collections.Generic;
using System.Threading.Tasks;
using Criteria.SalaryStructure;
using HRM.Core.Models;
using HRM.Core.Services;
using Interfaces;
using Models;

public class SalaryStructureService(IBaseService baseService) : ISalaryStructureService
{
    public async Task<IEnumerable<SalaryStructure>> GetSalaryStructureAsync(int CompanyId)
    {
        var criteria = new GetSalaryStructureCriteria
        {
            CompanyId = CompanyId
        };

        return await baseService.GetDataAsync<SalaryStructure>(criteria);
    }

    public async Task<SalaryStructure> GetByIdAsync(int Id)
    {
        var criteria = new GetSalaryStructureByIdCriteria
        {
            SalaryStructureId = Id
        };
        return await baseService.GetDataEntityAsync<SalaryStructure>(criteria);
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
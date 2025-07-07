namespace HRM.Services.Implement;

using System.Collections.Generic;
using System.Threading.Tasks;
using Criteria.SalaryStructure;
using HRM.Core.Models;
using HRM.Core.Services;
using Interfaces;
using System.Linq;
using Models;

public class SalaryStructureService(IBaseService baseService) : ISalaryStructureService
{
    public async Task<IList<SalaryStructure>> GetSalaryStructureAsync(int CompanyId)
    {
        var criteria = new GetSalaryStructureCriteria
        {
            CompanyId = CompanyId
        };

        var result = await baseService.GetDataAsync<SalaryStructure>(criteria);

        return result.ToList();
    }

    public async Task<SalaryStructure> GetByIdAsync(int Id)
    {
        var criteria = new GetSalaryStructureByIdCriteria
        {
            SalaryStructureId = Id
        };
        var result = await baseService.GetDataAsync<SalaryStructure>(criteria);
        return result.FirstOrDefault();
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
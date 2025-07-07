namespace HRM.Services.Implement;

using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Criteria.SalaryStructure;
using HRM.Core.Models;
using HRM.Core.Services;
using Interfaces;
using Models;

public class SalaryStructureService : ISalaryStructureService
{
    private readonly IBaseService baseService;

    public SalaryStructureService(IBaseService baseService)
    {
        this.baseService = baseService;
    }

    public async Task<CommandResult> DeleteAsync(int actorId, int id)
    {
        var criteria = new DeleteSalaryStructureCriteria
        {
            SalaryStructureId = id,
            ModifiedBy = actorId
        };
        return await baseService.ExecuteNonQueryAsync(criteria);
    }

    public async Task<IList<SalaryStructure>> GetSalaryStructureAsync()
    {
        var criteria = new GetSalaryStructureCriteria();

        var result = await baseService.GetDataAsync<SalaryStructure>(criteria);
        return result.ToList();
    }

    public async Task<SalaryStructure> GetByIdAsync(int Id)
    {
        var criteria = new GetSalaryStructureByIdCriteria
        {
            Id = Id
        };

        var result = await baseService.GetDataAsync<SalaryStructure>(criteria);
        return result.FirstOrDefault();
    }
}

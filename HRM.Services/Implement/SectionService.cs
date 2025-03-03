namespace HRM.Services.Implement;

using System.Collections.Generic;
using HRM.Core.Models;
using HRM.Core.Services;
using Criteria.Section;
using Models;
using Interfaces;
using System.Threading.Tasks;
using HRM.Repositories.Interfaces;

public class SectionService(
    ISectionRepository sectionRepository,
    IBaseService baseService)
    : ISectionService
{
    public Section GetById(int id)
    {
        var criteria = new GetSectionByIdCriteria
        {
            SectionId = id
        };
        return sectionRepository.GetById(criteria);
    }

    public async Task<Section> GetByIdAsync(int id)
    {
        var criteria = new GetSectionByIdCriteria
        {
            SectionId = id
        };
        return await sectionRepository.GetByIdAsync(criteria);
    }

    public IList<Section> GetSection(bool? isActive = true)
    {
        var criteria = new GetSectionCriteria
        {
            IsActive = isActive
        };
        var sections = sectionRepository.GetSection(criteria);

        return sections ?? new List<Section>();
    }

    public async Task<IList<Section>> GetSectionAsync(bool? isActive = true)
    {
        var criteria = new GetSectionCriteria
        {
            IsActive = isActive
        };
        var sections = await sectionRepository.GetSectionAsync(criteria);

        return sections ?? new List<Section>();
    }

    public CommandResult Delete(int actorId, int id)
    {
        var criteria = new DeleteSectionCriteria
        {
            sectionId = id,
            ModifiedBy = actorId
        };
        return baseService.ExecuteNonQuery(criteria);
    }

    public async Task<CommandResult> DeleteAsync(int actorId, int id)
    {
        var criteria = new DeleteSectionCriteria
        {
            sectionId = id,
            ModifiedBy = actorId
        };
        return await baseService.ExecuteNonQueryAsync(criteria);
    }
}
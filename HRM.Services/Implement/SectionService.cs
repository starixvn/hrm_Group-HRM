using HRM.Models.Section;

namespace HRM.Services.Implement;

using System.Collections.Generic;
using System.Threading.Tasks;
using Criteria.Section;
using HRM.Core.Models;
using HRM.Core.Services;
using HRM.Repositories.Interfaces;
using Interfaces;
using Core.Constants;
using GO.UserControl.Client.Core.Session;

public class SectionService(
    ISectionRepository sectionRepository,
    IBaseService baseService,
    IUserControlSession userControlSession) : ISectionService
{
    public async Task<Section> GetByIdAsync(int id)
    {
        var companyId = userControlSession.Get<short>(SessionKey.CurrentCompanyId);

        var criteria = new GetSectionByIdCriteria
        {
            SectionId = id,
            CompanyId = companyId
        };
        return await sectionRepository.GetByIdAsync(criteria);
    }

    public async Task<IList<Section>> GetSectionAsync(bool? isActive = true)
    {
        var companyId = userControlSession.Get<short>(SessionKey.CurrentCompanyId);
        var criteria = new GetSectionCriteria
        {
            IsActive = isActive,
            CompanyId = companyId
        };
        var sections = await sectionRepository.GetSectionAsync(criteria);

        return sections ?? new List<Section>();
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
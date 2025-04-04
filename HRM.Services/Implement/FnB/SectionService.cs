using System.Collections.Generic;
using System.Threading.Tasks;
using HRM.Core.Services;
using HRM.Criteria.Fnb;
using HRM.Models.FnB;
using HRM.Services.Interfaces.FnB;

namespace HRM.Services.Implement.FnB;

public class SectionService(IBaseService baseService) : ISectionService
{
    public async Task<IEnumerable<SectionModel>> GetSectionsByLastModifiedDate(GetSectionByLastModifiedDateCriteria criteria)
    {
        return await baseService.GetDataAsync<SectionModel>(criteria);
    }
}
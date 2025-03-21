using System.Collections.Generic;
using System.Threading.Tasks;
using HRM.Criteria.Fnb;
using HRM.Models.FnB;

namespace HRM.Services.Interfaces.FnB;

public interface ISectionService
{
    Task<IEnumerable<SectionModel>> GetSectionsByLastModifiedDate(GetSectionByLastModifiedDateCriteria criteria);
}
using HRM.Models.Section;

namespace HRM.Repositories.Interfaces;

using System.Collections.Generic;
using Fanex.Data.Repository;
using Criteria.Section;
using System.Threading.Tasks;

public interface ISectionRepository
{
    Task<Section> GetByIdAsync(ICriteria criteria);

    Task<IList<Section>> GetSectionAsync(GetSectionCriteria criteria);
}
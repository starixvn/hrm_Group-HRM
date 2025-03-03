namespace HRM.Repositories.Interfaces;

using System.Collections.Generic;
using Fanex.Data.Repository;
using Criteria.Section;
using Models;
using System.Threading.Tasks;

public interface ISectionRepository
{
    Section GetById(ICriteria criteria);

    Task<Section> GetByIdAsync(ICriteria criteria);

    IList<Section> GetSection(GetSectionCriteria criteria);

    Task<IList<Section>> GetSectionAsync(GetSectionCriteria criteria);
}
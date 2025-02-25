namespace HRM.Repositories
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using HRM.Criteria.Section;
    using HRM.Models;

    public interface ISectionRepository
    {
        Section GetById(ICriteria criteria);

        IList<Section> GetSection(GetSectionCriteria criteria);
    }
}
namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Criteria.Section;
    using Fanex.NexHR.Models;

    public interface ISectionRepository
    {
        Section GetById(ICriteria criteria);

        IList<Section> GetSection(GetSectionCriteria criteria);
    }
}
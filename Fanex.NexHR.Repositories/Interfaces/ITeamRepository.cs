namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;

    public interface ITeamRepository
    {
        Team GetById(ICriteria criteria);

        IList<Team> GetAll(ICriteria criteria);
    }
}
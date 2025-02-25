namespace HRM.Repositories
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using HRM.Models;

    public interface ITeamRepository
    {
        Team GetById(ICriteria criteria);

        IList<Team> GetAll(ICriteria criteria);
    }
}
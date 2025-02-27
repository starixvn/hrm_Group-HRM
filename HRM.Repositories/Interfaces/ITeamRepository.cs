namespace HRM.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using HRM.Models;

    public interface ITeamRepository
    {
        Team GetById(ICriteria criteria);

        Task<IEnumerable<Team>> GetAllAsync(ICriteria criteria);
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using Fanex.Data.Repository;
using HRM.Models;

namespace HRM.Repositories.Interfaces;

public interface ITeamRepository
{
    Task<Team> GetById(ICriteria criteria);

    Task<IEnumerable<Team>> GetAllAsync(ICriteria criteria);
}
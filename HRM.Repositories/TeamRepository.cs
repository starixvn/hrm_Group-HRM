namespace HRM.Repositories;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fanex.Data;
using Fanex.Data.Repository;
using Models;
using Interfaces;

public class TeamRepository : ITeamRepository
{
    public async Task<Team> GetById(ICriteria criteria)
    {
        using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        var teams = await db.QueryAsync<Team, Department, Team>(
            (t, d) =>
            {
                t.Department = d;
                return t;
            },
            splitOn: "DepartmentId",
            parameterSet: criteria);

        return teams.FirstOrDefault();
    }

    public async Task<IEnumerable<Team>> GetAllAsync(ICriteria criteria)
    {
        using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        return await db.QueryAsync<Team, Department, Team>(
            (t, d) =>
            {
                t.Department = d;
                return t;
            },
            splitOn: "DepartmentId",
            parameterSet: criteria);
    }
}
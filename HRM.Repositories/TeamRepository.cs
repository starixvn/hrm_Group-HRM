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
        using var multi = await db.QueryMultipleAsync(criteria);
        var team = multi.Read<Team, Department, Team>(
            (t, d) =>
            {
                t.Department = d;
                return t;
            },
            "DepartmentId").FirstOrDefault();

        return team;
    }

    public async Task<IEnumerable<Team>> GetAllAsync(ICriteria criteria)
    {
        using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        using var multi = await db.QueryMultipleAsync(criteria);
        var teams = multi.Read<Team, Department, Team>(
            (t, d) =>
            {
                t.Department = d;
                return t;
            },
            "DepartmentId"
        ).ToList();

        return teams;
    }
}
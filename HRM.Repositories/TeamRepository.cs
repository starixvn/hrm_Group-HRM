namespace HRM.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using HRM.Models;
    using HRM.Models.Group;

    public class TeamRepository : ITeamRepository
    {
        public Team GetById(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = db.QueryMultiple(criteria))
                {
                    var team = multi.Read<Team, Section, Team>(
                            (t, sec) =>
                            {
                                t.Section = sec;
                                return t;
                            },
                             "SectionId").FirstOrDefault();

                    if (team != null)
                    {
                        team.Managers = multi.Read<TeamManager>();
                    }

                    return team;
                }
            }
        }

        public IList<Team> GetAll(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (var multi = db.QueryMultiple(criteria))
                {
                    var teams = multi.Read<Team, Section , Team>(
                            (t, sec) =>
                            {
                                t.Section = sec;
                                return t;
                            },
                             "SectionId"
                            ).ToList();
                    var employees = multi.Read<TeamManager>();

                    foreach (var team in teams)
                    {
                        team.Managers = employees.Where(e => e.TeamId == team.Id);
                    }

                    return teams;
                }
            }
        }
    }
}
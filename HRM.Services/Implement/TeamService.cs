namespace HRM.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using HRM.Core.Models;
    using HRM.Core.Services;
    using HRM.Core.XMLExtensions;
    using HRM.Criteria.Group;
    using HRM.Criteria.Supervisor;
    using HRM.Group;
    using HRM.Models;
    using HRM.Models.Group;
    using HRM.Models.Supervisor;
    using HRM.Repositories;

    public class TeamService(
        ITeamRepository teamRepository,
        IBaseService baseService) : ITeamService
    {
        public Team GetById(int id)
        {
            var criteria = new GetGroupByIdCriteria
            {
                TeamId = id
            };
            return teamRepository.GetById(criteria);
        }

        public IList<Team> GetAll(bool isDeleted = false, string keyword = null)
        {
            var criteria = new GetAllGroupCriteria
            {
                IsDeleted = isDeleted,
                Keyword = keyword
            };
            IList<Team> teams = teamRepository.GetAll(criteria);

            return teams ?? new List<Team>();
        }

        public CommandResult Delete(int actorId, int id)
        {
            var criteria = new DeleteTeamCriteria
            {
                TeamId = id,
                ModifiedBy = actorId
            };
            return baseService.ExecuteNonQuery(criteria);
        }

        public async Task<IEnumerable<Team>> GetBySupervisorAsync(int supervisorId)
        {
            var criteria = new GetGroupBySupervisorCriteria
            {
                SupervisorId = supervisorId
            };
            return await baseService.GetDataAsync<Team>(criteria);
        }

        public IList<SupervisorModel> GetTeamSupervisorByRoleId(int teamId, short roleId)
        {
            var criteria = new GetTeamSupervisorByRoleIdCriteria
            {
                TeamId = teamId,
                Role = roleId
            };
            return baseService.GetData<SupervisorModel>(criteria).ToList();
        }

        public IEnumerable<TeamSupervisorModel> GetSupervisorByTeamId(int id)
        {
            var criteria = new GetSupervisorByTeamIdCriteria
            {
                TeamId = id
            };
            return baseService.GetData<TeamSupervisorModel>(criteria).ToList();
        }

        public IEnumerable<Supervisor> GetDisciplineManagerByTeamId(int teamId)
        {
            var criteria = new GetDisciplineManagerByTeamIdCriteria
            {
                TeamId = teamId
            };
            return baseService.GetData<Supervisor>(criteria).ToList();
        }

        public CommandResult EditTeamSupervisor(int actorId, int teamId, short roleId, IEnumerable<EditTeamSupervisor> supervisors)
        {
            var criteria = new EditTeamSupervisorCriteria
            {
                TeamId = teamId,
                ActorId = actorId,
                RoleId = roleId,
                SupervisorXml = supervisors.ToXML().ToString()
            };
            return baseService.ExecuteNonQuery(criteria);
        }

        public IEnumerable<DisciplineRule> GetDisciplineRuleByTeam(int teamId)
        {
            var criteria = new GetDisciplineRuleByTeamCriteria { TeamId = teamId };
            return baseService.GetData<DisciplineRule>(criteria);
        }

        public CommandResult EditDisciplineRule(int actorId, int teamId, IEnumerable<DisciplineRule> disciplineRules)
        {
            var criteria = new EditDisciplineRuleCriteria
            {
                TeamId = teamId,
                ActorId = actorId,
                DisciplineRuleXml = disciplineRules.ToXML().ToString()
            };
            return baseService.ExecuteNonQuery(criteria);
        }
    }
}
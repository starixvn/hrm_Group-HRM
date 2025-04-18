using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GO.UserControl.Client.Core.Session;
using HRM.Core.Constants;
using HRM.Core.Models;
using HRM.Core.Services;
using HRM.Core.XMLExtensions;
using HRM.Criteria.Group;
using HRM.Criteria.Supervisor;
using HRM.Models;
using HRM.Models.Group;
using HRM.Models.Supervisor;
using HRM.Repositories.Interfaces;
using HRM.Services.Interfaces;

namespace HRM.Services.Implement;

public class TeamService(
    ITeamRepository teamRepository,
    IBaseService baseService,
    IUserControlSession userControlSession) : ITeamService
{
    public async Task<Team> GetById(int id)
    {
        var companyId = userControlSession.Get<short>(SessionKey.CurrentCompanyId);
        var criteria = new GetGroupByIdCriteria
        {
            TeamId = id,
            CompanyId = companyId
        };
        return await teamRepository.GetById(criteria);
    }

    public async Task<int> GetNextGroupTeamIdAsync()
    {
        var criteria = new GetNextGroupTeamIdCriteria();
        return await baseService.GetDataEntityAsync<int>(criteria);
    }

    public async Task<IEnumerable<Team>> GetAllAsync(bool isDeleted = false, string keyword = null)
    {
        var companyId = userControlSession.Get<short>(SessionKey.CurrentCompanyId);
        var criteria = new GetAllGroupCriteria
        {
            IsDeleted = isDeleted,
            Keyword = keyword,
            CompanyId = companyId
        };
        var teams = await teamRepository.GetAllAsync(criteria);

        return teams ?? new List<Team>();
    }

    public async Task<CommandResult> Delete(int actorId, int id)
    {
        var criteria = new DeleteTeamCriteria
        {
            TeamId = id,
            ModifiedBy = actorId
        };
        return await baseService.ExecuteNonQueryAsync(criteria);
    }

    public async Task<IEnumerable<Team>> GetBySupervisorAsync(int supervisorId)
    {
        var criteria = new GetGroupBySupervisorCriteria
        {
            SupervisorId = supervisorId
        };
        return await baseService.GetDataAsync<Team>(criteria);
    }

    public async Task<IList<SupervisorModel>> GetTeamSupervisorByRoleId(int teamId, short roleId)
    {
        var criteria = new GetTeamSupervisorByRoleIdCriteria
        {
            TeamId = teamId,
            Role = roleId
        };
        return (await baseService.GetDataAsync<SupervisorModel>(criteria)).ToList();
    }

    public async Task<IEnumerable<TeamSupervisorModel>> GetSupervisorByTeamId(int id)
    {
        var criteria = new GetSupervisorByTeamIdCriteria
        {
            TeamId = id
        };
        return (await baseService.GetDataAsync<TeamSupervisorModel>(criteria)).ToList();
    }

    public async Task<IEnumerable<Supervisor>> GetDisciplineManagerByTeamId(int teamId)
    {
        var criteria = new GetDisciplineManagerByTeamIdCriteria
        {
            TeamId = teamId
        };
        return (await baseService.GetDataAsync<Supervisor>(criteria)).ToList();
    }

    public async Task<CommandResult> EditTeamSupervisor(int actorId, int teamId, short roleId, IEnumerable<EditTeamSupervisor> supervisors)
    {
        var criteria = new EditTeamSupervisorCriteria
        {
            TeamId = teamId,
            ActorId = actorId,
            RoleId = roleId,
            SupervisorXml = supervisors.ToXML().ToString()
        };
        return await baseService.ExecuteNonQueryAsync(criteria);
    }

    public async Task<IEnumerable<DisciplineRule>> GetDisciplineRuleByTeam(int teamId)
    {
        var criteria = new GetDisciplineRuleByTeamCriteria { TeamId = teamId };
        return await baseService.GetDataAsync<DisciplineRule>(criteria);
    }

    public async Task<CommandResult> EditDisciplineRule(int actorId, int teamId, IEnumerable<DisciplineRule> disciplineRules)
    {
        var criteria = new EditDisciplineRuleCriteria
        {
            TeamId = teamId,
            ActorId = actorId,
            DisciplineRuleXml = disciplineRules.ToXML().ToString()
        };
        return await baseService.ExecuteNonQueryAsync(criteria);
    }
}
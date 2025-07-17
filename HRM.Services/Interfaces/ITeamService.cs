using System.Collections.Generic;
using System.Threading.Tasks;
using HRM.Core.Models;
using HRM.Models;
using HRM.Models.Group;
using HRM.Models.Supervisor;

namespace HRM.Services.Interfaces;

public interface ITeamService
{
    Task<Team> GetById(int id);

    Task<int> GetNextGroupTeamIdAsync();

    Task<IEnumerable<Team>> GetAllAsync(bool isDeleted = false, string keyword = null, bool isGroup = false);

    Task<IEnumerable<GroupTeamModel>> GetAllTeamByUnitScopeAsync(string teamIds = null);

    Task<GroupTeamModel> GetGroupTeamInUnitScopeById(int id);

    Task<int> GetGroupIdByGroupTeamId(int groupTeamId);

    Task<CommandResult> Delete(int actorId, int id);

    Task<IEnumerable<Team>> GetBySupervisorAsync(int supervisorId);

    Task<IList<SupervisorModel>> GetTeamSupervisorByRoleId(int teamId, short roleId);

    Task<IEnumerable<TeamSupervisorModel>> GetSupervisorByTeamId(int id, bool isGroupSpace);

    Task<IEnumerable<Supervisor>> GetDisciplineManagerByTeamId(int teamId);

    Task<CommandResult> EditTeamSupervisor(int actorId, int teamId, short roleId, IEnumerable<EditTeamSupervisor> supervisors);

    Task<IEnumerable<DisciplineRule>> GetDisciplineRuleByTeam(int teamId);

    Task<CommandResult> EditDisciplineRule(int actorId, int teamId, IEnumerable<DisciplineRule> disciplineRules);

    Task<byte[]> GenerateGroupTeamExcel(IEnumerable<GroupTeamModel> teams);

    Task<CommandResult> InsertTeamSupervisorByPIC(int actorId, int teamId,
        IEnumerable<EditTeamSupervisor> supervisors);
}
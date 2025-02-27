namespace HRM.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRM.Core.Models;
    using HRM.Models;
    using HRM.Models.Group;
    using HRM.Models.Supervisor;

    public interface ITeamService
    {
        Team GetById(int id);

        Task<IEnumerable<Team>> GetAllAsync(bool isDeleted = false, string keyword = null);

        CommandResult Delete(int actorId, int id);

        Task<IEnumerable<Team>> GetBySupervisorAsync(int supervisorId);

        IList<SupervisorModel> GetTeamSupervisorByRoleId(int teamId, short roleId);

        IEnumerable<TeamSupervisorModel> GetSupervisorByTeamId(int id);

        IEnumerable<Supervisor> GetDisciplineManagerByTeamId(int teamId);

        CommandResult EditTeamSupervisor(int actorId, int teamId, short roleId, IEnumerable<EditTeamSupervisor> supervisors);

        IEnumerable<DisciplineRule> GetDisciplineRuleByTeam(int teamId);

        CommandResult EditDisciplineRule(int actorId, int teamId, IEnumerable<DisciplineRule> disciplineRules);
    }
}
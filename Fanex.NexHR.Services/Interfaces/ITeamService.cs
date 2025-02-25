namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Group;
    using Fanex.NexHR.Models.Supervisor;

    public interface ITeamService
    {
        Team GetById(int id);

        IList<Team> GetAll(bool isDeleted = false, string keyword = null);

        CommandResult Delete(int actorId, int id);

        IList<Team> GetBySupervisor(int supervisorId);

        IList<SupervisorModel> GetTeamSupervisorByRoleId(int teamId, short roleId);

        IEnumerable<TeamSupervisorModel> GetSupervisorByTeamId(int id);

        IEnumerable<Supervisor> GetDisciplineManagerByTeamId(int teamId);

        CommandResult EditTeamSupervisor(int actorId, int teamId, short roleId, IEnumerable<EditTeamSupervisor> supervisors);

        IEnumerable<DisciplineRule> GetDisciplineRuleByTeam(int teamId);

        CommandResult EditDisciplineRule(int actorId, int teamId, IEnumerable<DisciplineRule> disciplineRules);
    }
}
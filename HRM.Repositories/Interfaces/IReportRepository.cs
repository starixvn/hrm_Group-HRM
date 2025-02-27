namespace HRM.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using HRM.Models.Supervisor;
    using HRM.ViewModels;

    public partial interface IReportRepository
    {
        Task<IEnumerable<EmployeeAttendanceModel>> GetEmployeeAttendanceReportAsync(ICriteria criteria);

        IList<SupervisorReportModel> GetSupervisorReport(ICriteria criteria);

        IList<TeamSupervisorReportModel> GetTeamSupervisor(ICriteria criteria);
    }
}
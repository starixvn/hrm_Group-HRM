namespace HRM.Repositories
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using HRM.Models.Supervisor;
    using HRM.ViewModels;

    public partial interface IReportRepository
    {
        IList<EmployeeAttendanceModel> GetEmployeeAttendanceReport(ICriteria criteria);

        IList<SupervisorReportModel> GetSupervisorReport(ICriteria criteria);

        IList<TeamSupervisorReportModel> GetTeamSupervisor(ICriteria criteria);
    }
}
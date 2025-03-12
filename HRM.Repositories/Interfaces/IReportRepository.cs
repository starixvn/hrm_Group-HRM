using System.Collections.Generic;
using System.Threading.Tasks;
using Fanex.Data.Repository;
using HRM.Models.Supervisor;
using HRM.ViewModels.Report;

namespace HRM.Repositories.Interfaces;

public interface IReportRepository
{
    Task<IEnumerable<EmployeeAttendanceModel>> GetEmployeeAttendanceReportAsync(ICriteria criteria);

    Task<IList<SupervisorReportModel>> GetSupervisorReport(ICriteria criteria);

    Task<IList<TeamSupervisorReportModel>> GetTeamSupervisor(ICriteria criteria);
}
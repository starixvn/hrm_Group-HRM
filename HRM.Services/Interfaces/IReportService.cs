using System.Collections.Generic;
using System.Threading.Tasks;
using HRM.Criteria.AnnualLeave;
using HRM.Criteria.Report;
using HRM.Criteria.Supervisor;
using HRM.Criteria.Time;
using HRM.Models;
using HRM.Models.Report;
using HRM.Models.Supervisor;
using HRM.ViewModels.Report;

namespace HRM.Services.Interfaces;

public interface IReportService
{
    Task<IEnumerable<EmployeeAttendanceModel>> GetEmployeeAttendanceReportAsync(GetEmployeeAttendanceCriteria criteria);

    Task<IEnumerable<LeaveSummary>> GetAnnualLeaveBalanceReportAsync(GetAnnualLeaveBalanceCriteria criteria);

    Task<IEnumerable<LeaveSummary>> GetUnpaidLeaveAsync(GetUnpaidLeaveCriteria criteria, string period);

    Task<IEnumerable<TimeRecordModel>> GetTimeRecordAsync(GetTimeRecordCriteria criteria);

    Task<IList<SupervisorReportModel>> GetSupervisorReport(GetSupervisorReportCriteria criteria);

    Task<IList<TeamSupervisorReportModel>> GetTeamSupervisor(GetTeamSupervisorCriteria criteria);

    Task<IList<WeekendLeaveSummary>> GetWeekendLeaveSummaryAsync(GetWeekendLeaveSummaryCriteria criteria);

    Task<IList<WeekendLeaveDetail>> GetWeekendLeaveDetailAsync(GetWeekendLeaveDetailCriteria criteria);

    Task<bool> GetIsPrivilegeCreateCheckInOutAsync(GetPrivilegeCreateCheckInOutRecordCriteria criteria);
}
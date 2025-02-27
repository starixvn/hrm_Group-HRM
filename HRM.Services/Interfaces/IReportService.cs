namespace HRM.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRM.Criteria;
    using HRM.Criteria.AnnualLeave;
    using HRM.Criteria.Report;
    using HRM.Criteria.Supervisor;
    using HRM.Criteria.Time;
    using HRM.Models;
    using HRM.Models.Report;
    using HRM.Models.Supervisor;
    using HRM.ViewModels;

    public interface IReportService
    {
        Task<IEnumerable<EmployeeAttendanceModel>> GetEmployeeAttendanceReportAsync(GetEmployeeAttendanceCriteria criteria);

        Task<IEnumerable<LeaveSummary>> GetAnnualLeaveBalanceReportAsync(GetAnnualLeaveBalanceCriteria criteria);

        Task<IEnumerable<LeaveSummary>> GetUnpaidLeaveAsync(GetUnpaidLeaveCriteria criteria);

        IList<TimeRecordModel> GetTimeRecords(GetTimeRecordCriteria criteria);

        IList<SupervisorReportModel> GetSupervisorReport(GetSupervisorReportCriteria criteria);

        IList<TeamSupervisorReportModel> GetTeamSupervisor(GetTeamSupervisorCriteria criteria);

        IList<WeekendLeaveSummary> GetWeekendLeaveSummary(GetWeekendLeaveSummaryCriteria criteria);

        IList<WeekendLeaveDetail> GetWeekendLeaveDetail(GetWeekendLeaveDetailCriteria criteria);

        bool GetIsPrivilegeCreateCheckInOut(GetPrivilegeCreateCheckInOutRecordCriteria criteria);
    }
}
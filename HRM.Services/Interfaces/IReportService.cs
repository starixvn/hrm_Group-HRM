namespace HRM.Services
{
    using System.Collections.Generic;
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
        IList<EmployeeAttendanceModel> GetEmployeeAttendanceReport(GetEmployeeAttendanceCriteria criteria);

        IList<LeaveSummary> GetAnnualLeaveBalanceReport(GetAnnualLeaveBalanceCriteria criteria);

        IList<LeaveSummary> GetUnpaidLeave(GetUnpaidLeaveCriteria criteria);

        IList<TimeRecordModel> GetTimeRecords(GetTimeRecordCriteria criteria);

        IList<SupervisorReportModel> GetSupervisorReport(GetSupervisorReportCriteria criteria);

        IList<TeamSupervisorReportModel> GetTeamSupervisor(GetTeamSupervisorCriteria criteria);

        IList<WeekendLeaveSummary> GetWeekendLeaveSummary(GetWeekendLeaveSummaryCriteria criteria);

        IList<WeekendLeaveDetail> GetWeekendLeaveDetail(GetWeekendLeaveDetailCriteria criteria);

        bool GetIsPrivilegeCreateCheckInOut(GetPrivilegeCreateCheckInOutRecordCriteria criteria);
    }
}
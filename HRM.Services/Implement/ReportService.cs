namespace HRM.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using HRM.Core.Services;
    using HRM.Criteria;
    using HRM.Criteria.AnnualLeave;
    using HRM.Criteria.Report;
    using HRM.Criteria.Supervisor;
    using HRM.Criteria.Time;
    using HRM.Models;
    using HRM.Models.Report;
    using HRM.Models.Supervisor;
    using HRM.Repositories;
    using HRM.ViewModels;

    public class ReportService(
        IReportRepository reportRepository,
        IBaseService baseService) : IReportService
    {
        public async Task<IEnumerable<EmployeeAttendanceModel>> GetEmployeeAttendanceReportAsync(GetEmployeeAttendanceCriteria criteria)
        {
            IEnumerable<EmployeeAttendanceModel> employeeAttendance = null;
            if (criteria != null && criteria.IsValid())
            {
                employeeAttendance = await reportRepository.GetEmployeeAttendanceReportAsync(criteria);
            }

            return employeeAttendance;
        }

        public async Task<IEnumerable<LeaveSummary>> GetAnnualLeaveBalanceReportAsync(GetAnnualLeaveBalanceCriteria criteria)
        {
            return await baseService.GetDataAsync<LeaveSummary>(criteria);
        }

        public async Task<IEnumerable<LeaveSummary>> GetUnpaidLeaveAsync(GetUnpaidLeaveCriteria criteria)
        {
            return await baseService.GetDataAsync<LeaveSummary>(criteria);
        }

        public IList<TimeRecordModel> GetTimeRecords(GetTimeRecordCriteria criteria)
        {
            return baseService.GetData<TimeRecordModel>(criteria).ToList();
        }

        public IList<SupervisorReportModel> GetSupervisorReport(GetSupervisorReportCriteria criteria)
        {
            IList<SupervisorReportModel> supervisors = null;
            if (criteria != null && criteria.IsValid())
            {
                supervisors = reportRepository.GetSupervisorReport(criteria);
            }

            return supervisors;
        }

        public IList<TeamSupervisorReportModel> GetTeamSupervisor(GetTeamSupervisorCriteria criteria)
        {
            IList<TeamSupervisorReportModel> supervisors = null;
            if (criteria != null && criteria.IsValid())
            {
                supervisors = reportRepository.GetTeamSupervisor(criteria);
            }

            return supervisors;
        }

        public IList<WeekendLeaveSummary> GetWeekendLeaveSummary(GetWeekendLeaveSummaryCriteria criteria)
        {
            return baseService.GetData<WeekendLeaveSummary>(criteria).ToList();
        }

        public IList<WeekendLeaveDetail> GetWeekendLeaveDetail(GetWeekendLeaveDetailCriteria criteria)
        {
            return baseService.GetData<WeekendLeaveDetail>(criteria).ToList();
        }

        public bool GetIsPrivilegeCreateCheckInOut(GetPrivilegeCreateCheckInOutRecordCriteria criteria)
        {
            return baseService.GetDataEntity<bool>(criteria);
        }
    }
}
namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Criteria;
    using Fanex.NexHR.Criteria.AnnualLeave;
    using Fanex.NexHR.Criteria.Report;
    using Fanex.NexHR.Criteria.Supervisor;
    using Fanex.NexHR.Criteria.Time;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Report;
    using Fanex.NexHR.Models.Supervisor;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.ViewModels;

    public class ReportService : IReportService
    {
        private readonly IReportRepository reportRepository;
        private readonly IBaseService baseService;

        public ReportService(
            IReportRepository reportRepository,
            IBaseService baseService)
        {
            this.reportRepository = reportRepository;
            this.baseService = baseService;
        }

        public IList<EmployeeAttendanceModel> GetEmployeeAttendanceReport(GetEmployeeAttendanceCriteria criteria)
        {
            IList<EmployeeAttendanceModel> employeeAttendance = null;
            if (criteria != null && criteria.IsValid())
            {
                employeeAttendance = reportRepository.GetEmployeeAttendanceReport(criteria);
            }

            return employeeAttendance;
        }

        public IList<LeaveSummary> GetAnnualLeaveBalanceReport(GetAnnualLeaveBalanceCriteria criteria)
        {
            return baseService.GetData<LeaveSummary>(criteria).ToList();
        }

        public IList<LeaveSummary> GetUnpaidLeave(GetUnpaidLeaveCriteria criteria)
        {
            return baseService.GetData<LeaveSummary>(criteria).ToList();
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
namespace HRM.Services
{
    using System.Collections.Generic;
    using System.Linq;
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
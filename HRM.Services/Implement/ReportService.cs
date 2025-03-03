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
using HRM.Services.Interfaces;
using HRM.ViewModels;

namespace HRM.Services.Implement
{
    public class ReportService(
        Repositories.Interfaces.IReportRepository reportRepository,
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

        public async Task<IList<SupervisorReportModel>> GetSupervisorReport(GetSupervisorReportCriteria criteria)
        {
            IList<SupervisorReportModel> supervisors = null;
            if (criteria != null && criteria.IsValid())
            {
                supervisors = await reportRepository.GetSupervisorReport(criteria);
            }

            return supervisors;
        }

        public async Task<IList<TeamSupervisorReportModel>> GetTeamSupervisor(GetTeamSupervisorCriteria criteria)
        {
            IList<TeamSupervisorReportModel> supervisors = null;
            if (criteria != null && criteria.IsValid())
            {
                supervisors = await reportRepository.GetTeamSupervisor(criteria);
            }

            return supervisors;
        }

        public async Task<IList<WeekendLeaveSummary>> GetWeekendLeaveSummaryAsync(GetWeekendLeaveSummaryCriteria criteria)
        {
            return (await baseService.GetDataAsync<WeekendLeaveSummary>(criteria)).ToList();
        }

        public async Task<IList<WeekendLeaveDetail>> GetWeekendLeaveDetailAsync(GetWeekendLeaveDetailCriteria criteria)
        {
            return (await baseService.GetDataAsync<WeekendLeaveDetail>(criteria)).ToList();
        }

        public async Task<bool> GetIsPrivilegeCreateCheckInOutAsync(GetPrivilegeCreateCheckInOutRecordCriteria criteria)
        {
            return await baseService.GetDataEntityAsync<bool>(criteria);
        }
    }
}
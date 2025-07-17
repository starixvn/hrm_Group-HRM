using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRM.Core;
using HRM.Core.Services;
using HRM.Criteria.AnnualLeave;
using HRM.Criteria.Report;
using HRM.Criteria.Supervisor;
using HRM.Criteria.Time;
using HRM.Models;
using HRM.Models.Report;
using HRM.Models.Supervisor;
using HRM.Repositories.Interfaces;
using HRM.Services.Interfaces;
using HRM.ViewModels.Report;

namespace HRM.Services.Implement;

public class ReportService(
    IReportRepository reportRepository,
    IBaseService baseService) : IReportService
{
    public async Task<IEnumerable<EmployeeAttendanceModel>> GetEmployeeAttendanceReportAsync(GetEmployeeAttendanceCriteria criteria)
    {
        IEnumerable<EmployeeAttendanceModel> attendances = null;
        if (criteria != null && criteria.IsValid())
        {
            attendances = await reportRepository.GetEmployeeAttendanceReportAsync(criteria);

            foreach (var attendance in attendances)
            {
                foreach (var request in attendance.RequestList)
                {
                    request.CreatedDate = request.CreatedDate.ToClientTimeZone();
                    request.FromTime = request.FromTime?.ToClientTimeZone() ?? request.FromTime;
                    request.ToTime = request.ToTime?.ToClientTimeZone() ?? request.ToTime;
                }
            }
        }

        return attendances;
    }

    public async Task<IEnumerable<LeaveSummary>> GetAnnualLeaveBalanceReportAsync(GetAnnualLeaveBalanceCriteria criteria)
    {
        return await baseService.GetDataAsync<LeaveSummary>(criteria);
    }

    public async Task<IEnumerable<LeaveSummaryForJoinedDate>> GetAnnualLeaveBalanceForJoinedDateReportAsync(GetAnnualLeaveBalanceForJoinedDateCriteria criteria)
    {
        return await baseService.GetDataAsync<LeaveSummaryForJoinedDate>(criteria);
    }

    public async Task<IEnumerable<LeaveSummary>> GetUnpaidLeaveAsync(GetUnpaidLeaveCriteria criteria, string period)
    {
        var leaveSummaries = await baseService.GetDataAsync<LeaveSummary>(criteria);
        return period switch
        {
            "1" => leaveSummaries.Where(HasUnpaidLeaveInFirstHalf).ToList(),
            "2" => leaveSummaries.Where(HasUnpaidLeaveInSecondHalf).ToList(),
            _ => leaveSummaries
        };
    }

    private static bool HasUnpaidLeaveInFirstHalf(LeaveSummary x)
    {
        return Array.Exists([x.Jan, x.Feb, x.Mar, x.Apr, x.May, x.Jun], month => month > 0);
    }

    private static bool HasUnpaidLeaveInSecondHalf(LeaveSummary x)
    {
        return Array.Exists([x.Jul, x.Aug, x.Sep, x.Oct, x.Nov, x.Dec], month => month > 0);
    }

    public async Task<IEnumerable<TimeRecordModel>> GetTimeRecordAsync(GetTimeRecordCriteria criteria)
    {
        return await baseService.GetDataAsync<TimeRecordModel>(criteria);
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

    public async Task<IEnumerable<TeamSupervisorReportModel>> GetTeamSupervisorAsync(GetTeamSupervisorCriteria criteria)
    {
        IEnumerable<TeamSupervisorReportModel> supervisors = null;
        if (criteria != null && criteria.IsValid())
        {
            supervisors = await reportRepository.GetTeamSupervisorAsync(criteria);
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
        var currentDate = DateTimeOffset.Now.DateTime.ToClientTimeZone().Date;
        criteria.CurrentDate = currentDate;
        return await baseService.GetDataEntityAsync<bool>(criteria);
    }
}
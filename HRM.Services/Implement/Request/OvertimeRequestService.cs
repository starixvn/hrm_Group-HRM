using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GO.UserControl.Client.Core.Session;
using HRM.Core;
using HRM.Core.Constants;
using HRM.Core.Enums;
using HRM.Core.Models;
using HRM.Core.Services;
using HRM.Core.XMLExtensions;
using HRM.Criteria.Overtime;
using HRM.Models.Overtime;
using HRM.Models.Supervisor;
using HRM.Repositories.Interfaces;
using HRM.Services.Interfaces;

namespace HRM.Services.Implement.Request;

public class OvertimeRequestService(
    IOvertimeRequestRepository overtimeRepository,
    IBaseService baseService,
    IEmailService emailService,
    ILetstalkService letStalkService,
    IUserControlSession userControlSession) : IOvertimeRequestService
{
    private const byte FromDay = 21;
    private const byte ToDay = 20;

    public async Task<IEnumerable<MonthlyOvertimeRequest>> GetMonthlyRequestByEmployeeId(int employeeId, int year)
    {
        var today = DateTimeOffset.Now;
        var fromDate = new DateTimeOffset(year, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var toDate = today;

        if (today.Year != year)
        {
            toDate = new DateTimeOffset(year, Constants.December, 1, 0, 0, 0, TimeSpan.Zero);
        }

        if (today.Day > ToDay)
        {
            toDate = toDate.AddMonths(1);
        }

        if (toDate.Year > year)
        {
            toDate = new DateTimeOffset(year, Constants.December, 1, 0, 0, 0, TimeSpan.Zero);
        }

        var companyId = userControlSession.Get<short>(SessionKey.CurrentCompanyId);

        var criteria = new GetMonthlyOvertimeRequestByEmployeeIdCriteria
        {
            EmployeeId = employeeId,
            FromDate = fromDate.Date,
            ToDate = toDate.Date,
            CompanyId = companyId
        };
        var monthlyRequests = await overtimeRepository.GetMonthlyRequestByEmployeeId(criteria);

        return monthlyRequests;
    }

    public async Task<MonthlyOvertimeRequest> GetMonthlyRequestDetailsByEmployeeId(int employeeId, DateTime month)
    {
        var (fromDate, toDate) = GetDateRange(month.FirstDayOfMonth());
        var companyId = userControlSession.Get<short>(SessionKey.CurrentCompanyId);
        var criteria = new GetMonthlyOvertimeRequestDetailsByEmployeeIdCriteria
        {
            EmployeeId = employeeId,
            Month = month.FirstDayOfMonth(),
            FromDate = fromDate,
            ToDate = toDate,
            CompanyId = companyId
        };
        var monthlyRequest = await overtimeRepository.GetMonthlyRequestDetailsByEmployeeId(criteria);

        return monthlyRequest ?? new MonthlyOvertimeRequest();
    }

    public async Task<IEnumerable<MonthlyOvertimeRequest>> GetMemberMonthlyRequestAsync(GetMonthlyOvertimeRequestParam param)
    {
        var criteria = new GetMemberMonthlyOvertimeRequestCriteria
        {
            ActorId = param.ActorId,
            Month = param.Month,
            StatusIds = param.StatusIds,
            TeamIds = param.TeamIds,
            EmployeeId = param.EmployeeId,
            IsProcessor = param.IsProcessor,
            OvertimeType = param.OvertimeType,
            IsLocal = param.IsLocal,
            CompanyId = param.CompanyId
        };
        return await overtimeRepository.GetMemberMonthlyRequestAsync(criteria);
    }

    public async Task<MonthlyOvertimeRequest> GetMonthlyRequestDetailsById(long requestId)
    {
        var companyId = userControlSession.Get<short>(SessionKey.CurrentCompanyId);
        var criteria = new GetMonthlyOvertimeRequestDetailsByIdCriteria
        {
            RequestId = requestId,
            CompanyId = companyId
        };
        var monthlyRequest = await overtimeRepository.GetMonthlyRequestDetailsByEmployeeId(criteria);

        return monthlyRequest ?? new MonthlyOvertimeRequest();
    }

    public async Task SendMailToReminderClaimOvertime(string subject, string content)
    {
        var companyId = userControlSession.Get<short>(SessionKey.CurrentCompanyId);
        var criteria = new GetEmployeeToRemindClaimOvertimeCriteria { CompanyId = companyId };
        var employees = (await baseService.GetDataAsync<SupervisorNameModel>(criteria)).ToList();

        if (employees.Count > 0)
        {
            var emailList = employees.Where(t => !string.IsNullOrWhiteSpace(t.Email)).Select(t => t.Email).ToList();
            var letStalkList = employees.Where(t => !string.IsNullOrWhiteSpace(t.LetsTalk)).Select(t => t.LetsTalk).ToList();
            var emails = SplitList(emailList, 100);
            var letStalks = SplitList(letStalkList, 100);

            foreach (var list in emails)
            {
                var mailTo = string.Join(";", list);
                await emailService.SendEmailAsync(mailTo, string.Empty, subject, content);
            }

            foreach (var letStalk in letStalks)
            {
                var letStalkJoin = string.Join(";", letStalk);
                await Task.Run(() => letStalkService.SendNotificationAsync(letStalkJoin, subject, content));
            }
        }
    }

    public async Task<bool> CheckInvalidOvertimes(string requestIds)
    {
        var isInvalid = false;
        var criteria = new GetOvertimeDetailsByRequestIdsCriteria { RequestIds = requestIds };
        var overtimes = (await baseService.GetDataAsync<DailyOvertimeRequest>(criteria)).ToList();
        if (overtimes.Count > 0)
        {
            var index = 0;
            while (!isInvalid && index < overtimes.Count)
            {
                isInvalid = CheckInvalidOvertimes(overtimes[index]);
                index++;
            }
        }

        return isInvalid;
    }

    public async Task<CommandResult> EditMonthlyRequest(int actorId, EditMonthlyOvertimeRequest model)
    {
        var criteria = new EditMonthlyOvertimeRequestCriteria
        {
            ActorId = actorId,
            EmployeeId = model.EmployeeId,
            Month = model.Month.FirstDayOfMonth(),
            VerifierId = model.VerifierId,
            ApproverId = model.ApproverId,
            DailyDetailsXml = model.DailyOvertimes.ToXML().ToString()
        };
        return await baseService.ExecuteNonQueryAsync(criteria);
    }

    public async Task<CommandResult> EditMonthlyRequestStatus(int actorId, string requestIds, RequestCommand command, byte roleId)
    {
        var criteria = new EditMonthlyOvertimeRequestStatusCriteria
        {
            RequestCommand = command,
            RequestIds = requestIds,
            ActorId = actorId,
            RoleId = roleId,
            CompanyId = userControlSession.Get<short>(SessionKey.CurrentCompanyId)
        };
        return await baseService.ExecuteNonQueryAsync(criteria);
    }

    private static bool CheckInvalidOvertimes(DailyOvertimeRequest overtime)
    {
        var isInvalid = false;
        if ((!overtime.IsHoliday && !overtime.IsOffDay) || overtime.IsKeepInView)
        {
            isInvalid |= IsInvalidKeepInView(overtime);
        }

        if (overtime.IsOffDay)
        {
            isInvalid |= IsInvalidOffDay(overtime);
        }

        if (overtime.IsHoliday)
        {
            isInvalid |= IsInvalidHoliday(overtime);
        }

        return isInvalid;
    }

    private static bool IsInvalidKeepInView(DailyOvertimeRequest overtime)
    {
        return !overtime.OffHours.Equals(0)
               || !overtime.PublicHours.Equals(0)
               || overtime.ActualWorkingHours < overtime.TotalShiftHours + overtime.NormalHours
               || overtime.ActualWorkingHours < overtime.TotalShiftHours + overtime.SpecialHours;
    }

    private static bool IsInvalidOffDay(DailyOvertimeRequest overtime)
    {
        return !overtime.NormalHours.Equals(0)
               || !overtime.PublicHours.Equals(0)
               || overtime.ActualWorkingHours < overtime.OffHours;
    }

    private static bool IsInvalidHoliday(DailyOvertimeRequest overtime)
    {
        return !overtime.NormalHours.Equals(0)
               || !overtime.OffHours.Equals(0)
               || overtime.ActualWorkingHours < overtime.PublicHours;
    }

    private static List<List<string>> SplitList(List<string> locations, int size)
    {
        var emails = new List<List<string>>();
        for (var i = 0; i < locations.Count; i += size)
        {
            emails.Add(locations.GetRange(i, Math.Min(size, locations.Count - i)));
        }

        return emails;
    }

    private static ValueTuple<DateTime, DateTime> GetDateRange(DateTime month)
    {
        var newRuleApplyFromMonth = new DateTimeOffset(2020, 5, 1, 0, 0, 0, TimeSpan.Zero);
        var lastMonth = month.AddMonths(-1);
        var fromDate = new DateTimeOffset(lastMonth.Year, lastMonth.Month, 10, 0, 0, 0, TimeSpan.Zero);
        var toDate = new DateTimeOffset(month.Year, month.Month, 9, 0, 0, 0, TimeSpan.Zero);

        if (month == newRuleApplyFromMonth)
        {
            toDate = new DateTimeOffset(month.Year, month.Month, ToDay, 0, 0, 0, TimeSpan.Zero);
        }

        if (month > newRuleApplyFromMonth)
        {
            fromDate = new DateTimeOffset(lastMonth.Year, lastMonth.Month, FromDay, 0, 0, 0, TimeSpan.Zero);
            toDate = new DateTimeOffset(month.Year, month.Month, ToDay, 0, 0, 0, TimeSpan.Zero);
        }

        return (fromDate.Date, toDate.Date);
    }
}
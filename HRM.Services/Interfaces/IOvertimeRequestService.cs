using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HRM.Core.Enums;
using HRM.Core.Models;
using HRM.Models.Overtime;

namespace HRM.Services.Interfaces;

public interface IOvertimeRequestService
{
    Task<IEnumerable<MonthlyOvertimeRequest>> GetMonthlyRequestByEmployeeId(int employeeId, int year);

    Task<MonthlyOvertimeRequest> GetMonthlyRequestDetailsByEmployeeId(int employeeId, DateTime month);

    Task<IEnumerable<MonthlyOvertimeRequest>> GetMemberMonthlyRequest(GetMonthlyOvertimeRequestParam param);

    Task<MonthlyOvertimeRequest> GetMonthlyRequestDetailsById(long requestId);

    Task SendMailToReminderClaimOvertime(string subject, string content);

    Task<bool> CheckInvalidOvertimes(string requestIds);

    Task<CommandResult> EditMonthlyRequest(int actorId, EditMonthlyOvertimeRequest model);

    Task<CommandResult> EditMonthlyRequestStatus(int actorId, string requestIds, RequestCommand command, byte roleId);
}
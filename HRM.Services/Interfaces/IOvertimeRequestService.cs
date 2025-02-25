namespace HRM.Services
{
    using System;
    using System.Collections.Generic;
    using HRM.Core.Enums;
    using HRM.Core.Models;
    using HRM.Models.Overtime;

    public interface IOvertimeRequestService
    {
        IEnumerable<MonthlyOvertimeRequest> GetMonthlyRequestByEmployeeId(int employeeId, int year);

        MonthlyOvertimeRequest GetMonthlyRequestDetailsByEmployeeId(int employeeId, DateTime month);

        IEnumerable<MonthlyOvertimeRequest> GetMemberMonthlyRequest(GetMonthlyOvertimeRequestParam param);

        MonthlyOvertimeRequest GetMonthlyRequestDetailsById(long requestId);

        void SendMailToReminderClaimOvertime(string subject, string content);

        bool CheckInvalidOvertimes(string requestIds);

        CommandResult EditMonthlyRequest(int actorId, EditMonthlyOvertimeRequest model);

        CommandResult EditMonthlyRequestStatus(int actorId, string requestIds, RequestCommand command, byte roleId);
    }
}
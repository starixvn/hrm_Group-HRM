namespace HRM.Services
{
    using System;
    using HRM.Core.Enums;
    using HRM.Models;

    public interface IRequestMailService
    {
        EmailModel GetMailToMailCCAndLetstalk(long requestId, int action);

        byte GetActionProcessRequest(byte requestStatusId, RequestCommand command);

        string GetCommandMessage(RequestCommand command);

        string GetStringDateForLeaveRequest(DateTime requestedDate, DateTime fromDate, DateTime toDate, decimal totalDays);
    }
}
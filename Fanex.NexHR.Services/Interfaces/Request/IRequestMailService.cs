namespace Fanex.NexHR.Services
{
    using System;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Models;

    public interface IRequestMailService
    {
        EmailModel GetMailToMailCCAndLetstalk(long requestId, int action);

        byte GetActionProcessRequest(byte requestStatusId, RequestCommand command);

        string GetCommandMessage(RequestCommand command);

        string GetStringDateForLeaveRequest(DateTime requestedDate, DateTime fromDate, DateTime toDate, decimal totalDays);
    }
}
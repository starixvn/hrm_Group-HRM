using System;
using System.Threading.Tasks;
using HRM.Core.Enums;
using HRM.Models;

namespace HRM.Services.Interfaces.Request;

public interface IRequestMailService
{
    Task<EmailModel> GetMailToMailCcAndLetStalk(long requestId, int action);

    byte GetActionProcessRequest(byte requestStatusId, RequestCommand command);

    string GetCommandMessage(RequestCommand command);

    string GetStringDateForLeaveRequest(DateTime requestedDate, DateTime fromDate, DateTime toDate, decimal totalDays);

    Task<MailConfigModel> GetEmployeeMailConfig(int empId);
}
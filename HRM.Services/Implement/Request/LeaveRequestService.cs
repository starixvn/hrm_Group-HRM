using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GO.UserControl.Client.Core.Session;
using HRM.Core;
using HRM.Core.Constants;
using HRM.Core.Enums;
using HRM.Core.Models;
using HRM.Core.Services;
using HRM.Criteria.Request;
using HRM.Criteria.Request.Leave;
using HRM.Models;
using HRM.Models.Request;
using HRM.Models.Request.Leave;
using HRM.Repositories.Interfaces;
using HRM.Resources;
using HRM.Services.Interfaces;
using HRM.Services.Interfaces.Request;
using Microsoft.AspNetCore.Hosting;
using Fanex.Logging;

namespace HRM.Services.Implement.Request;

public class LeaveRequestService(
    IWebHostEnvironment env,
    ILeaveRequestRepository leaveRequestRepository,
    IRequestMailService requestMailService,
    IBaseService baseService,
    IEmailService emailService,
    ILetstalkService letStalkService,
    ICompanyService companyService,
    ILogger logger,
    IUserControlSession userControlSession) : ILeaveRequestService
{
    private const string EmailTemplatePathForRequest = "EmailTemplates/Leave.Request.txt";
    private const string EmailTemplatePathForCancel = "EmailTemplates/Leave.Cancel.txt";
    private const string EmailTemplatePathForApproval = "EmailTemplates/Leave.Approval.txt";
    private const string LetsTalkTemplatePathForRequest = "LetsTalkTemplate/Leave.Request.txt";
    private const string LetsTalkTemplatePathForCancel = "LetsTalkTemplate/Leave.Cancel.txt";
    private const string LetsTalkTemplatePathForApproval = "LetsTalkTemplate/Leave.Approval.txt";

    public async Task<IList<LeaveRequest>> GetByDateAsync(GetLeaveRequestByDateCriteria criteria)
    {
        IList<LeaveRequest> requests = null;
        if (criteria != null && criteria.IsValid())
        {
            requests = await leaveRequestRepository.GetByDateAsync(criteria);
        }

        return requests ?? [];
    }

    public async Task<LeaveRequest> GetByIdAsync(long requestId)
    {
        var companyId = userControlSession.Get<short>(SessionKey.CurrentCompanyId);
        if (companyId == 0)
        {
            var company = await companyService.GetCompanySiteByDomainAsync();
            companyId = company.CompanyId;
        }
        var criteria = new GetLeaveRequestByIdCriteria
        {
            RequestId = requestId,
            CompanyId = companyId
        };
        return await leaveRequestRepository.GetByIdAsync(criteria);
    }

    public async Task<IList<LeaveRequestDetail>> GetLeaveRequestDetailByIdAsync(long requestId)
    {
        var criteria = new GetLeaveRequestDetailByIdCriteria
        {
            RequestId = requestId
        };
        return (await baseService.GetDataAsync<LeaveRequestDetail>(criteria)).ToList();
    }

    public async Task<IEnumerable<LeaveRequest>> GetHistoryByIdAsync(long requestId)
    {
        var companyId = userControlSession.Get<short>(SessionKey.CurrentCompanyId);
        var criteria = new GetLeaveRequestHistoryByIdCriteria
        {
            RequestId = requestId,
            CompanyId = companyId
        };
        return await leaveRequestRepository.GetHistoryByIdAsync(criteria);
    }

    public async Task<IEnumerable<DateTime>> GetDisabledDateAsync(GetDisabledDateParam param)
    {
        var companyId = userControlSession.Get<short>(SessionKey.CurrentCompanyId);
        var criteria = new GetDisabledDateCriteria
        {
            LeaveTypeId = param.LeaveTypeId,
            EmployeeId = param.EmployeeId,
            FromDate = param.FromDate,
            ToDate = param.ToDate,
            RequestId = param.RequestId,
            CompanyId = companyId
        };
        return await baseService.GetDataAsync<DateTime>(criteria);
    }

    public async Task<IEnumerable<Models.ScheduleArrange.DailySchedule>> GetEmployeeCalendarAsync(int employeeId, DateTime fromDate, DateTime toDate)
    {
        var companyId = userControlSession.Get<short>(SessionKey.CurrentCompanyId);
        var criteria = new GetEmployeeCalendarCriteria
        {
            EmployeeId = employeeId,
            FromDate = fromDate,
            ToDate = toDate,
            CompanyId = companyId
        };
        return await leaveRequestRepository.GetEmployeeCalendarAsync(criteria);
    }

    public async Task<EmployeeLeaveBalance> GetLeaveBalance(int userId, long? requestId, int? year)
    {
        var criteria = new GetLeaveBalanceCriteria
        {
            EmpId = userId,
            RequestId = requestId,
            EffectedDay = year.HasValue
                ? new DateTimeOffset(year.Value, 1, 1, 0, 0, 0, TimeSpan.Zero).Date
                : DateTimeOffset.Now.Date.ToClientTimeZone()
        };
        return await GetLeaveBalanceAsync(criteria);
    }

    public async Task<EmployeeLeaveBalance> GetLeaveBalanceAsync(GetLeaveBalanceCriteria criteria)
    {
        var leaveBalance = await baseService.GetDataEntityAsync<EmployeeLeaveBalance>(criteria);
        leaveBalance.KeepInViewByHourBalance = Math.Round(leaveBalance.KeepInViewByHourBalance / Constants.TotalMinuteInHour, 1);
        return leaveBalance;
    }

    public async Task<CommandResult> DeleteAsync(long requestId)
    {
        var companyId = userControlSession.Get<short>(SessionKey.CurrentCompanyId);
        var criteria = new DeleteLeaveRequestCriteria
        {
            RequestId = requestId,
            CompanyId = companyId
        };
        return await baseService.ExecuteNonQueryAsync(criteria);
    }

    public void SendMailAndSendLetstalkCreateRequest(InsertLeaveRequestCriteria criteria, string domain, short companyId)
    {
        Task.Run(() => SendMailForInsertSuccessfully(criteria, domain, companyId));
    }

    public void SendMailEditRequest(UpdateLeaveRequestCriteria criteria, string domain, short companyId)
    {
        Task.Run(() => SendMailForUpdateSuccessfully(criteria, domain, companyId));
    }

    private async Task SendMailForInsertSuccessfully(InsertLeaveRequestCriteria criteria, string domain, short companyId)
    {
        try
        {
            var email = await requestMailService.GetMailToMailCcAndLetStalk(criteria.RequestId, (int)SettingsSendMail.New, (int)RequestType.Leave);
            email.Subject = Messages.LeaveEmailSubject + " " + criteria.EmployeeName;
            await SendMailForSuccessfully(criteria, email, domain, companyId);
        }
        catch (Exception e)
        {
            await logger.InfoAsync($"Send Email + Letstalk insert fail: {e.Message});");
        }
    }

    private async Task SendMailForUpdateSuccessfully(UpdateLeaveRequestCriteria criteria, string domain, short companyId)
    {
        try
        {
            var email = await requestMailService.GetMailToMailCcAndLetStalk(criteria.RequestId, (int)SettingsSendMail.Edit, (int)RequestType.Leave);
            email.Subject = Messages.UpdatedLeaveEmailSubject + " " + criteria.EmployeeName;
            await SendMailForSuccessfully(criteria, email, domain, companyId);
        }
        catch (Exception e)
        {
            await logger.InfoAsync($"Send Email + Letstalk fail: {e.Message});");
        }
    }

    public async Task SendMailDeleteRequest(long requestId)
    {
        var request = await GetByIdAsync(requestId);
        if (request != null)
        {
            var email = await requestMailService.GetMailToMailCcAndLetStalk(requestId, (byte)SettingsSendMail.Cancel, (int)RequestType.Leave);
            var fromDate = request.TotalDays == Constants.HalfDayOffValue ? GetFromTimeHalfDay(request.LeaveDetail) : request.FromDate;
            var toDate = request.TotalDays == Constants.HalfDayOffValue ? GetToTimeHalfDay(request.LeaveDetail) : request.ToDate;
            var forDate = requestMailService.GetStringDateForLeaveRequest(request.FromDate, fromDate, toDate, request.TotalDays);

            email.Subject = string.Format(Messages.MessageLeaveRequestCanceled, request.Requester.Nickname);
            email.Body = Utilities.ReadTextFile(GetTemplatePath(EmailTemplatePathForCancel));
            email.Body = string.Format(
                email.Body,
                email.SendToNickName,
                request.Requester.DisplayName,
                forDate,
                request.LeaveType.Name,
                request.Remark);

            await emailService.SendEmailAsync(
                email.SendTo,
                email.SendCC,
                email.Subject,
                email.Body);

            var content = Utilities.ReadTextFile(GetTemplatePath(LetsTalkTemplatePathForCancel));
            content = string.Format(
                content,
                request.Requester.DisplayName,
                forDate,
                request.LeaveType.Name,
                request.Remark
                );

            await SendLetsTalkNotifications(email, content);
        }
    }

    public async Task SendMailProcessRequest(long requestId, EmployeeSession actor, RequestCommand command)
    {
        var request = await GetByIdAsync(requestId);
        if (request != null)
        {
            var action = requestMailService.GetActionProcessRequest(request.Status.Id, command);
            var commandMessage = requestMailService.GetCommandMessage(command);
            var fromDate = request.TotalDays == Constants.HalfDayOffValue ? GetFromTimeHalfDay(request.LeaveDetail) : request.FromDate;
            var toDate = request.TotalDays == Constants.HalfDayOffValue ? GetToTimeHalfDay(request.LeaveDetail) : request.ToDate;
            var forDate = requestMailService.GetStringDateForLeaveRequest(request.FromDate, fromDate, toDate, request.TotalDays);

            var email = await requestMailService.GetMailToMailCcAndLetStalk(requestId, action, (int)RequestType.Leave);
            email.Subject = string.Format(Messages.ApprovalNotificationLeaveSubject,
                request.Requester.Nickname,
                commandMessage,
                actor.Nickname);
            email.Body = Utilities.ReadTextFile(GetTemplatePath(EmailTemplatePathForApproval));
            email.Body = string.Format(
                email.Body,
                email.SendToNickName,
                request.Requester.DisplayName,
                commandMessage,
                actor.DisplayName,
                request.RequestTypeName,
                forDate,
                request.LeaveType.Name,
                request.Remark);

            await emailService.SendEmailAsync(
                email.SendTo,
                email.SendCC,
                email.Subject,
                email.Body,
                companyId: request.Requester.CompanyId
                );

            var content = Utilities.ReadTextFile(GetTemplatePath(LetsTalkTemplatePathForApproval));
            content = string.Format(
                content,
                commandMessage,
                actor.DisplayName,
                forDate,
                request.LeaveType.Name,
                request.Remark);
            await Task.Run(() => letStalkService.SendNotification(email.Owner.LetsTalkId, content));
        }
    }

    public DateTime GetFromTimeHalfDay(IList<LeaveRequestDetail> leaveDetails)
    {
        var fromTime = DateTimeOffset.Now;
        var leaveHalfDay = leaveDetails.FirstOrDefault(t => t.WorkType == (byte)WorkType.Leave && t.IsHalfDay);
        if (leaveHalfDay != null)
        {
            fromTime = leaveHalfDay.FromTime;
        }

        return fromTime.DateTime;
    }

    public DateTime GetToTimeHalfDay(IList<LeaveRequestDetail> leaveDetails)
    {
        var toTime = DateTimeOffset.Now;
        var leaveHalfDay = leaveDetails.FirstOrDefault(t => t.WorkType == (byte)WorkType.Leave && t.IsHalfDay);
        if (leaveHalfDay != null)
        {
            toTime = leaveHalfDay.ToTime;
        }

        return toTime.DateTime;
    }

    private string GetTemplatePath(string templatePath)
    {
        return Path.Combine(env.ContentRootPath, templatePath);
    }

    private async Task SendMailForSuccessfully(BaseLeaveRequestCriteria criteria, EmailModel email, string domain, short companyId)
    {
        email.Subject += criteria.TotalDays > 1
            ? " from " + criteria.FromDate.ToDateMonthYearFormatedDate() + " to " + criteria.ToDate.ToDateMonthYearFormatedDate()
            : " on " + criteria.FromDate.ToDateMonthYearFormatedDate();
        var fromDate = criteria.TotalDays == Constants.HalfDayOffValue ? GetFromTimeHalfDay(criteria.RequestDetail) : criteria.FromDate;
        var toDate = criteria.TotalDays == Constants.HalfDayOffValue ? GetToTimeHalfDay(criteria.RequestDetail) : criteria.ToDate;
        var forDate = requestMailService.GetStringDateForLeaveRequest(criteria.FromDate, fromDate, toDate, criteria.TotalDays);

        email.Body = Utilities.ReadTextFile(GetTemplatePath(EmailTemplatePathForRequest));
        email.Body = string.Format(
            email.Body,
            email.SendToNickName,
            criteria.EmployeeName,
            forDate,
            criteria.LeaveTypeName,
            criteria.Remark,
            domain);
        if (!string.IsNullOrWhiteSpace(email.SendTo))
        {
            await emailService.SendEmailAsync(
                email.SendTo,
                email.SendCC,
                email.Subject,
                email.Body,
                companyId);
        }

        var content = Utilities.ReadTextFile(GetTemplatePath(LetsTalkTemplatePathForRequest));
        content = string.Format(
            content,
            email.Subject,
            email.RequesterCode,
            criteria.EmployeeName,
            criteria.LeaveTypeName,
            forDate,
            email.PICDisplayName,
            criteria.Remark);

        await SendLetsTalkNotifications(email, content);
    }

    private async Task SendLetsTalkNotifications(EmailModel email, string content)
    {
        if (email.Approver != null && !string.IsNullOrWhiteSpace(email.Approver.LetsTalkId))
        {
            await Task.Run(() => letStalkService.SendNotification(email.Approver.LetsTalkId, content, email.Approver.ReplyMarkup));
        }
        if (email.Verifier != null && !string.IsNullOrWhiteSpace(email.Verifier.LetsTalkId))
        {
            await Task.Run(() => letStalkService.SendNotification(email.Verifier.LetsTalkId, content, email.Verifier.ReplyMarkup));
        }
    }
}